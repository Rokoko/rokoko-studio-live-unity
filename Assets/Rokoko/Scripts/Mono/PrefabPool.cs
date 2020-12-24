using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rokoko
{
    public class PrefabPool<T> where T : MonoBehaviour
    {
        public int poolNumber = 3;
        public T prefab;
        public Transform container;

        private Queue<T> pool = new Queue<T>();

        public PrefabPool(T prefab, Transform container, int poolNumber = 3)
        {
            this.prefab = prefab;
            this.container = container;
            this.poolNumber = poolNumber;

            for (int i = 0; i < poolNumber; i++)
            {
                Enqueue(InstantiatePrefab());
            }
        }

        public T Dequeue()
        {
            if (pool.Count == 0)
                Enqueue(InstantiatePrefab());
            T instance = pool.Dequeue();
            instance.gameObject.SetActive(true);
            return instance;
        }

        public void Enqueue(T instance)
        {
            pool.Enqueue(instance);
            instance.gameObject.SetActive(false);
            instance.name = prefab.name;
        }

        private T InstantiatePrefab()
        {
            T instance = GameObject.Instantiate<T>(prefab);
            instance.transform.SetParent(container);
            instance.transform.position = Vector3.zero;
            instance.transform.rotation = Quaternion.identity;
            instance.name = prefab.name;
            return instance;
        }
    }
}