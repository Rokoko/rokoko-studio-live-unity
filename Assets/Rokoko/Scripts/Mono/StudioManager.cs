using Rokoko;
using Rokoko.Core;
using Rokoko.CommandAPI;
using Rokoko.Helper;
using Rokoko.Inputs;
using Rokoko.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rokoko
{
    public class StudioManager : MonoBehaviour
    {
        private const string ACTOR_DEMO_IDLE_NAME = "ActorIdle";

        private static StudioManager instance;

        [Header("Network")]
        [Tooltip("ReceivePort must match Studio Live Stream port settings")]
        public int receivePort = 14043;

        [Tooltip("Use LZ4 compression stream")]
        public bool useLZ4Compression = true;

        [Tooltip("Log the stream frame information")]
        public bool receiverVerbose = false;

        [Header("Default Inputs - Used when no overrides found (Optional)")]
        [Tooltip("Actor Prefab to create actors when no overrides found")]
        public Actor actorPrefab;
        [Tooltip("Character Prefab to create characters when no overrides found")]
        public Character characterPrefab;
        [Tooltip("Prop Prefab to create props when no overrides found")]
        public Prop propPrefab;

        [Header("UI (Optional)")]
        public UIHierarchyManager uiManager;

        [Header("Command API (Optional)")]
        public StudioCommandAPI CommandAPI;
        public bool AutoSendTrackerCommands;

        [Header("Input Overrides - Automatically updated")]
        public List<Actor> actorOverrides = new List<Actor>();
        public List<Character> characterOverrides = new List<Character>();
        public List<Prop> propOverrides = new List<Prop>();

        [Header("Extra Behiavours")]
        public bool autoGenerateInputsWhenNoOverridesFound = false;
        public bool showDefaultActorWhenNoData = false;

        private StudioReceiver studioReceiver;
        private PrefabInstancer<string, Actor> actors;
        private PrefabInstancer<string, Character> characters;
        private PrefabInstancer<string, Prop> props;

        private object actionsOnMainThread = new object();
        private List<LiveFrame_v4> packetsToProcess = new List<LiveFrame_v4>();

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
            studioReceiver.useLZ4Compression = useLZ4Compression;
            studioReceiver.verbose = receiverVerbose;
            studioReceiver.Initialize();
            studioReceiver.StartListening();
            studioReceiver.onStudioDataReceived += StudioReceiver_onStudioDataReceived;

            if (actorPrefab != null)
                actors = new PrefabInstancer<string, Actor>(actorPrefab, this.transform);
            if (characterPrefab != null)
                characters = new PrefabInstancer<string, Character>(characterPrefab, this.transform);
            if (propPrefab != null)
                props = new PrefabInstancer<string, Prop>(propPrefab, this.transform);

            yield return null;

            if (actorOverrides.Count == 0 && characterOverrides.Count == 0)
            {
                Debug.Log("No custom characters found. Will generate scene from default ones");
            }
        }

        private void Update()
        {
            // Run all actions inside Unity's main thread
            lock (actionsOnMainThread)
            {
                if (packetsToProcess.Count > 0)
                {
                    ProcessLiveFrame(packetsToProcess[packetsToProcess.Count-1]);    
                    packetsToProcess.Clear();
                }
            }
        }

        private void FixedUpdate()
        {
            if (AutoSendTrackerCommands && CommandAPI != null)
            {
                if (!CommandAPI.IsTrackerRequestInProgress)
                {
                    CommandAPI.Tracker();    
                }
            }
        }

        private void OnDestroy()
        {
            studioReceiver.Dispose();
        }

        #endregion

        private void StudioReceiver_onStudioDataReceived(object sender, LiveFrame_v4 e)
        {
            lock (actionsOnMainThread)
                packetsToProcess.Add(e);
        }

        /// <summary>
        /// Main process logic of live data
        /// </summary>
        private void ProcessLiveFrame(LiveFrame_v4 frame)
        {
            int numberOfActors = frame?.scene.actors?.Length ?? 0;
            int numberOfCharacters = frame?.scene.characters?.Length ?? 0;
            
            if (numberOfActors == 0 && numberOfCharacters == 0)
                return;

            // Update each actor from live data
            for (int i = 0; i < numberOfActors; i++)
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
                else if (autoGenerateInputsWhenNoOverridesFound && actors != null)
                {
                    actors[actorFrame.name].UpdateActor(actorFrame);
                }
            }

            // Update each character from live data
            for (int i = 0; i < numberOfCharacters; i++)
            {
                CharacterFrame charFrame = frame.scene.characters[i];

                List<Character> characterOverrides = GetCharacterOverride(charFrame.name);
                // Update custom characters if any
                if (characterOverrides.Count > 0)
                {
                    for (int a = 0; a < characterOverrides.Count; a++)
                    {
                        characterOverrides[a].UpdateCharacter(charFrame);
                    }
                }
                // Update default character
                else if (autoGenerateInputsWhenNoOverridesFound && characters != null)
                {
                    characters[charFrame.name].UpdateCharacter(charFrame);
                }
            }

            // Update each prop from live data
            if (frame.scene.props != null)
            {
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
                    else if (autoGenerateInputsWhenNoOverridesFound && props != null)
                    {
                        props[propFrame.name].UpdateProp(propFrame);
                    }
                }    
            }
            
            // Remove all default Actors that doesn't exist in data 
            ClearUnusedDefaultInputs(frame);

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
            if (actors == null || props == null) // || characters == null) 
                return;

            // Create default actor
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
        private void ClearUnusedDefaultInputs(LiveFrame_v4 frame)
        {
            if (actors != null)
            {
                foreach (Actor actor in new List<Actor>((IEnumerable<Actor>)actors.Values))
                {
                    // Don't remove idle demo
                    if (actor.profileName == ACTOR_DEMO_IDLE_NAME) continue;

                    if (!frame.HasProfile(actor.profileName))
                        actors.Remove(actor.profileName);
                }
            }

            if (characters != null)
            {
                foreach (Character character in new List<Character>((IEnumerable<Character>)characters.Values))
                {
                    // Don't remove idle demo
                    if (character.profileName == ACTOR_DEMO_IDLE_NAME) continue;

                    if (!frame.HasCharacter(character.profileName))
                        characters.Remove(character.profileName);
                }
            }

            if (props != null)
            {
                foreach (Prop prop in new List<Prop>((IEnumerable<Prop>)props.Values))
                {
                    if (!frame.HasProp(prop.propName))
                        props.Remove(prop.propName);
                }
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

        public List<Character> GetCharacterOverride(string profileName)
        {
            List<Character> overrides = new List<Character>();
            for (int i = 0; i < characterOverrides.Count; i++)
            {
                if (characterOverrides[i] == null)
                    continue;

                if (profileName.ToLower() == characterOverrides[i].profileName.ToLower())
                    overrides.Add(characterOverrides[i]);
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

        public static void AddCharacterOverride(Character character)
        {
            if (instance == null) return;
            if (instance.characterOverrides.Contains(character)) return;
            instance.characterOverrides.Add(character);
        }

        public static void AddPropOverride(Prop prop)
        {
            if (instance == null) return;
            if (instance.propOverrides.Contains(prop)) return;
            instance.propOverrides.Add(prop);
        }
    }
}