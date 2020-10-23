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

    [Header("Network")]
    public int receivePort = 14043;

    [Header("Default Actors")]
    public Actor actorPrefab;
    public Prop propPrefab;
    public bool showDefaultActorWhenNoData = true;

    [Header("UI")]
    public UIHierarchyManager uiManager;

    private ActorOverrides actorOverrides;
    private StudioReceiver studioReceiver;
    private PrefabInstancer<string, Actor> actors;
    private PrefabInstancer<string, Prop> props;

    private List<Action> actionsOnMainThread = new List<Action>();

    #region MonoBehaviour

    // Start is called before the first frame update
    private void Start()
    {
        studioReceiver = new StudioReceiver();
        studioReceiver.receivePortNumber = receivePort;
        studioReceiver.Initialize();
        studioReceiver.StartListening();
        studioReceiver.onStudioDataReceived += StudioReceiver_onStudioDataReceived;

        actors = new PrefabInstancer<string, Actor>(actorPrefab, this.transform);
        props = new PrefabInstancer<string, Prop>(propPrefab, this.transform);

        actorOverrides = this.GetComponent<ActorOverrides>();
        if (actorOverrides == null || actorOverrides.actorOverrides.Count == 0)
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

            Actor overrideActor = actorOverrides?.GetActorOverride(actorFrame.name);
            // Update custom actor if any
            if (overrideActor != null)
                overrideActor.UpdateActor(actorFrame);
            // Update default actor
            else
                actors[actorFrame.name].UpdateActor(actorFrame);
        }

        // Update each prop from live data
        for (int i = 0; i < frame.scene.props.Length; i++)
        {
            PropFrame propFrame = frame.scene.props[i];
            props[propFrame.name].UpdateProp(propFrame);
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

}
