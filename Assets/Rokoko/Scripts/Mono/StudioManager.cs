using Rokoko;
using Rokoko.Core;
using Rokoko.Helper;
using Rokoko.Inputs;
using Rokoko.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudioManager : MonoBehaviour
{
    private const string ACTOR_DEMO_IDLE_NAME = "ActorIdle";

    private static StudioManager instance;

    [Header("Network")]
    [Tooltip("ReceivePort must match Studio Live Stream port settings")]
    public int receivePort = 14043;

    [Header("Default Inputs - Used when no overrides found")]
    [Tooltip("Actor Prefab to create actors when no overrides found")]
    public Actor actorPrefab;
    [Tooltip("Prop Prefab to create props when no overrides found")]
    public Prop propPrefab;

    [Header("UI")]
    public UIHierarchyManager uiManager;

    [Header("Input Overrides - Automatically updated")]
    public List<Actor> actorOverrides = new List<Actor>();
    public List<Prop> propOverrides = new List<Prop>();

    [Header("Extra Behiavours")]
    public bool autoGenerateInputsWhenNoOverridesFound = true;
    public bool showDefaultActorWhenNoData = true;

    private StudioReceiver studioReceiver;
    private PrefabInstancer<string, Actor> actors;
    private PrefabInstancer<string, Prop> props;

    private List<Action> actionsOnMainThread = new List<Action>();

    #region MonoBehaviour

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Start is called before the first frame update
    private IEnumerator Start()
    {
        studioReceiver = new StudioReceiver();
        studioReceiver.receivePortNumber = receivePort;
        studioReceiver.Initialize();
        studioReceiver.StartListening();
        studioReceiver.onStudioDataReceived += StudioReceiver_onStudioDataReceived;

        actors = new PrefabInstancer<string, Actor>(actorPrefab, this.transform);
        props = new PrefabInstancer<string, Prop>(propPrefab, this.transform);

        yield return null;

        if (actorOverrides.Count == 0)
        {
            Debug.Log("No custom characters found. Will generate scene from default ones");
        }
    }

    private void Update()
    {
        // Run all actions inside Unity's main thread
        lock (actionsOnMainThread)
        {
            for (int i = 0; i < actionsOnMainThread.Count; i++)
            {
                actionsOnMainThread[i]();
            }
            actionsOnMainThread.Clear();
        }
    }

    private void OnDestroy()
    {
        studioReceiver.Dispose();
    }

    #endregion

    /// <summary>
    /// Store actions to run on Unity's main thread
    /// </summary>
    private void RunOnMainThread(Action action)
    {
        lock (actionsOnMainThread)
            actionsOnMainThread.Add(action);
    }

    private void StudioReceiver_onStudioDataReceived(object sender, LiveFrame_v4 e)
    {
        // Process in Unity thread
        RunOnMainThread(() =>
        {
            ProcessLiveFrame(e);
        });
    }

    /// <summary>
    /// Main process logic of live data
    /// </summary>
    private void ProcessLiveFrame(LiveFrame_v4 frame)
    {
        // Update each actor from live data
        for (int i = 0; i < frame.scene.actors.Length; i++)
        {
            ActorFrame actorFrame = frame.scene.actors[i];

            List<Actor> actorOverrides = GetActorOverride(actorFrame.name);
            // Update custom actors if any
            if (actorOverrides.Count > 0)
            {
                for (int a = 0; a < actorOverrides.Count; a++)
                {
                    actorOverrides[a].UpdateActor(actorFrame);
                }
            }
            // Update default actor
            else if (autoGenerateInputsWhenNoOverridesFound)
            {
                actors[actorFrame.name].UpdateActor(actorFrame);
            }
        }

        // Update each prop from live data
        for (int i = 0; i < frame.scene.props.Length; i++)
        {
            PropFrame propFrame = frame.scene.props[i];

            List<Prop> propOverrides = GetPropOverride(propFrame.name);
            // Update custom props if any
            if (propOverrides.Count > 0)
            {
                for (int a = 0; a < propOverrides.Count; a++)
                {
                    propOverrides[a].UpdateProp(propFrame);
                }
            }
            // Update default prop
            else if (autoGenerateInputsWhenNoOverridesFound)
            {
                props[propFrame.name].UpdateProp(propFrame);
            }
        }

        // Remove all default Actors that doesn't exist in data 
        ClearUnusedDefaultActors(frame);

        // Show default character
        UpdateDefaultActorWhenIdle();

        // Update Hierarchy UI
        uiManager?.UpdateHierarchy(frame);
    }

    /// <summary>
    /// Show default T pose character when not playback data 
    /// </summary>
    private void UpdateDefaultActorWhenIdle()
    {
        if (!showDefaultActorWhenNoData) return;

        // Crate default actor
        if (actors.Count == 0 && props.Count == 0)
        {
            actors[ACTOR_DEMO_IDLE_NAME].CreateIdle(ACTOR_DEMO_IDLE_NAME);
        }
        // No need to update
        else if (actors.Count == 1 && actors.ContainsKey(ACTOR_DEMO_IDLE_NAME))
        {

        }
        // Remove default actor when playback data available
        else
        {
            actors.Remove(ACTOR_DEMO_IDLE_NAME);
        }
    }

    /// <summary>
    /// Remove all default Actors that doesn't exist in data 
    /// </summary>
    private void ClearUnusedDefaultActors(LiveFrame_v4 frame)
    {
        foreach (Actor actor in new List<Actor>((IEnumerable<Actor>)actors.Values))
        {
            // Don't remove idle demo
            if (actor.profileName == ACTOR_DEMO_IDLE_NAME) continue;

            if (!frame.HasProfile(actor.profileName))
                actors.Remove(actor.profileName);
        }

        foreach (Prop prop in new List<Prop>((IEnumerable<Prop>)props.Values))
        {
            if (!frame.HasProp(prop.propName))
                props.Remove(prop.propName);
        }
    }

    public List<Actor> GetActorOverride(string profileName)
    {
        List<Actor> overrides = new List<Actor>();
        for (int i = 0; i < actorOverrides.Count; i++)
        {
            if (profileName.ToLower() == actorOverrides[i].profileName.ToLower())
                overrides.Add(actorOverrides[i]);
        }
        return overrides;
    }

    public List<Prop> GetPropOverride(string profileName)
    {
        List<Prop> overrides = new List<Prop>();
        for (int i = 0; i < propOverrides.Count; i++)
        {
            if (profileName.ToLower() == propOverrides[i].propName.ToLower())
                overrides.Add(propOverrides[i]);
        }
        return overrides;
    }

    public static void AddActorOverride(Actor actor)
    {
        if (instance == null) return;
        if (instance.actorOverrides.Contains(actor)) return;
        instance.actorOverrides.Add(actor);
    }

    public static void AddPropOverride(Prop prop)
    {
        if (instance == null) return;
        if (instance.propOverrides.Contains(prop)) return;
        instance.propOverrides.Add(prop);
    }
}
