using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rokoko.Inputs
{
    public class ActorOverrides : MonoBehaviour
    {
        private static ActorOverrides instance;

        public List<Actor> actorOverrides = new List<Actor>();

        private void Awake()
        {
            if (instance == null)
                instance = this;
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

        public static void AddActorOverride(Actor actor)
        {
            if (instance == null) return;
            if (instance.actorOverrides.Contains(actor)) return;
            instance.actorOverrides.Add(actor);
        }
    }
}