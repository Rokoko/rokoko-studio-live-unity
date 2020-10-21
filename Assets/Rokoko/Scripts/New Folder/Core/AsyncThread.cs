using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Linq;

namespace Rokoko.Threading
{
    public class AsyncThread : MonoBehaviour
    {
        private struct DelayedQueueItem
        {
            public float Time;
            public Action Action;
        }

        #region Static

        /// <summary>
        /// Maximum number of active threads.
        /// </summary>
        public static readonly int maxThreads = 6;

        /// <summary>
        /// Handle data in MainThread with delay.
        /// </summary>
        public static void RunOnMainThread(Action action, float delay = 0)
        {
            if (delay != 0)
            {
                lock (Instance._delayed)
                {
                    Instance._delayed.Add(new DelayedQueueItem { Time = UnityEngine.Time.time + delay, Action = action });
                }
            }
            else
            {
                lock (Instance._actions)
                {
                    Instance._actions.Add(action);
                }
            }
        }

        /// <summary>
        /// Exercute code in a separate Thread.
        /// </summary>
        public static Thread RunOnNewThread(Action a)
        {
            Initialize();
            // Wait for Threads to complete
            while (numThreads >= maxThreads)
            {
                Thread.Sleep(1);
            }
            Interlocked.Increment(ref numThreads);
            ThreadPool.QueueUserWorkItem(RunAction, a);
            return null;
        }

        private static AsyncThread Instance
        {
            get
            {
                Initialize();
                return _instance;
            }
        }

        /// <summary>
        /// Active number of threads.
        /// </summary>
        private static int numThreads;

        /// <summary>
        /// Is Instance Initialized?
        /// </summary>
        private static bool initialized;

        /// <summary>
        /// Static instance.
        /// </summary>
        private static AsyncThread _instance;

        /// <summary>
        /// Initialize instance.
        /// </summary>
        private static void Initialize()
        {
            if (!initialized)
            {
                if (!Application.isPlaying)
                    return;
                initialized = true;
                _instance = new GameObject("AsyncThread").AddComponent<AsyncThread>();
                DontDestroyOnLoad(_instance);
            }
        }

        /// <summary>
        /// Run code in separate Thread.
        /// </summary>
        /// <param name="action"></param>
        private static void RunAction(object action)
        {
            try
            {
                ((Action)action)();
            }
            catch
            { }
            finally
            {
                Interlocked.Decrement(ref numThreads);
            }
        }

        #endregion

        private List<Action> _actions = new List<Action>();
        private List<Action> _currentActions = new List<Action>();
        private List<DelayedQueueItem> _delayed = new List<DelayedQueueItem>();
        private List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();

        // Initialize Instance
        void Awake()
        {
            _instance = this;
            initialized = true;
        }

        // Update is called once per frame
        void Update()
        {
            lock (_actions)
            {
                _currentActions.Clear();
                _currentActions.AddRange(_actions);
                _actions.Clear();
            }
            foreach (var a in _currentActions)
            {
                a();
            }
            lock (_delayed)
            {
                _currentDelayed.Clear();
                _currentDelayed.AddRange(_delayed.Where(d => d.Time <= Time.time));
                foreach (var item in _currentDelayed)
                    _delayed.Remove(item);
            }
            foreach (var delayed in _currentDelayed)
            {
                delayed.Action();
            }
        }

        void OnDisable()
        {
            if (_instance == this)
                _instance = null;
        }
    }
}