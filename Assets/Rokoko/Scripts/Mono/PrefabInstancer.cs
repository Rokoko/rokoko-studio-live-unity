using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rokoko
{
    public class PrefabInstancer<T, P> where P : MonoBehaviour
    {
        private PrefabPool<P> pool;
        private Dictionary<T, P> objects;

        public PrefabInstancer(P prefab, Transform container, int poolNumber = 0)
        {
            pool = new PrefabPool<P>(prefab, container, poolNumber);
            objects = new Dictionary<T, P>();
        }

        public P this[T key]
        {
            get
            {
                if (!objects.ContainsKey(key))
                    objects.Add(key, pool.Dequeue());
                return objects[key];
            }
        }

        public bool ContainsKey(T key) => objects.ContainsKey(key);

        public bool ContainsValue(P item) => objects.ContainsValue(item);

        public IEnumerable Keys => objects.Keys;

        public IEnumerable Values => objects.Values;

        public int Count => objects.Count;

        public void Remove(T key)
        {
            if (!ContainsKey(key)) return;
            P item = objects[key];
            objects.Remove(key);
            pool.Enqueue(item);
        }
    }
}