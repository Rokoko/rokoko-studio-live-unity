using Rokoko.RemoteAPI;
using Rokoko.Serializers;
using Rokoko.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudioManager : MonoBehaviour
{
    private StudioReceiver studioReceiver;
    public Actor actorPrefab;
    public Prop propPrefab;

    private PrefabInstancer<string, Actor> actors;
    private PrefabInstancer<string, Prop> props;

    #region MonoBehaviour

    // Start is called before the first frame update
    void Start()
    {
        studioReceiver = new StudioReceiver();
        studioReceiver.Initialize();
        studioReceiver.StartListening();
        studioReceiver.onStudioDataReceived += StudioReceiver_onStudioDataReceived;

        actors = new PrefabInstancer<string, Actor>(actorPrefab, this.transform);
        props = new PrefabInstancer<string, Prop>(propPrefab, this.transform);
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
            actors[actorFrame.name].UpdateActor(actorFrame);
        }

        for (int i = 0; i < frame.scene.props.Length; i++)
        {
            PropFrame propFrame = frame.scene.props[i];
            props[propFrame.name].UpdateProp(propFrame);
        }
    }
}
