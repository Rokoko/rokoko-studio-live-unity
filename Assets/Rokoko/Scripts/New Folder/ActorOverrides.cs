using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rokoko
{
    public class ActorOverrides : MonoBehaviour
    {
        public List<ActorOverride> actorOverrides = new List<ActorOverride>();

        public Actor GetActorOverride(string profileName)
        {
            for (int i = 0; i < actorOverrides.Count; i++)
            {
                if (profileName.ToLower() == actorOverrides[i].profileName.ToLower())
                    return actorOverrides[i].actor;
            }
            return null;
        }


        [System.Serializable]
        public class ActorOverride
        {
            public string profileName;
            public Actor actor;
        }
    }
}