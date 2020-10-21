using Rokoko.Threading;
using Studio.Scripts.Live.Serializers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudioManager : MonoBehaviour
{
    private StudioReceiver studioReceiver;
    public Actor actorPrefab;
    public Prop propPrefab;

    private Dictionary<string, Actor> actors = new Dictionary<string, Actor>();
    private PrefabPool<Actor> actorPool;

    private Dictionary<string, Prop> props = new Dictionary<string, Prop>();
    private PrefabPool<Prop> propPool;

    #region MonoBehaviour

    // Start is called before the first frame update
    void Start()
    {
        studioReceiver = new StudioReceiver();
        studioReceiver.Initialize();
        studioReceiver.StartListening();
        studioReceiver.onStudioDataReceived += StudioReceiver_onStudioDataReceived;

        actorPool = new PrefabPool<Actor>(actorPrefab, this.transform);
        propPool = new PrefabPool<Prop>(propPrefab, this.transform);
    }

    private void OnDestroy()
    {
        studioReceiver.Dispose();
    }

    #endregion

    private void StudioReceiver_onStudioDataReceived(object sender, LiveFrame_v4 e)
    {
        // Process in Unity thread
        AsyncThread.RunOnMainThread(() =>
        {
            ProcessLiveFrame(e);
        });
    }

    private void ProcessLiveFrame(LiveFrame_v4 frame)
    {
        for (int i = 0; i < frame.scene.actors.Length; i++)
        {
            ActorFrame actorFrame = frame.scene.actors[i];
            string actorName = actorFrame.name;
            if (!actors.ContainsKey(actorName))
                actors.Add(actorName, actorPool.Dequeue());
            actors[actorName].UpdateActor(actorFrame);
        }

        if (frame.scene.props.Length > 0)
        {
            propPrefab.transform.position = frame.scene.props[0].position.ToVector3();
            propPrefab.transform.rotation = frame.scene.props[0].rotation.ToQuaternion();
        }

        for (int i = 0; i < frame.scene.props.Length; i++)
        {
            PropFrame propFrame = frame.scene.props[i];
            string propName = propFrame.name;
            if (!props.ContainsKey(propName))
                props.Add(propName, propPool.Dequeue());
            props[propName].UpdateProp(propFrame);
        }
    }
}
