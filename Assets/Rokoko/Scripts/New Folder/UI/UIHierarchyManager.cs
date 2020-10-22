﻿using Rokoko.Serializers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rokoko.UI
{
    public class UIHierarchyManager : MonoBehaviour
    {
        [SerializeField] private InputHierarchyRow rowPrefab = null;
        [SerializeField] private Transform prefabContainer = null;

        private PrefabInstancer<string, InputHierarchyRow> rows;

        // Start is called before the first frame update
        void Start()
        {
            // Destroy children before PrefabInstancer creates the pool
            prefabContainer.DestroyChildren();

            rows = new PrefabInstancer<string, InputHierarchyRow>(rowPrefab, prefabContainer);
        }

        public void UpdateHierarchy(LiveFrame_v4 dataFrame)
        {
            // Check if UI needs rebuild
            bool forceLayoutUpdate = false;

            // Update each actor from live data
            for (int i = 0; i < dataFrame.scene.actors.Length; i++)
            {
                ActorFrame actorFrame = dataFrame.scene.actors[i];
                string profileName = dataFrame.scene.actors[i].name;
                
                // If profile doesn't exist, mark for rebuild
                if (forceLayoutUpdate == false && !rows.ContainsKey(profileName))
                    forceLayoutUpdate = true;

                rows[profileName].UpdateRow(actorFrame);
            }

            ClearUnusedInputRows(dataFrame);

            if (forceLayoutUpdate)
                LayoutRebuilder.ForceRebuildLayoutImmediate(prefabContainer as RectTransform);
        }

        public void RemoveRow(string profileName)
        {
            rows.Remove(profileName);
        }

        /// <summary>
        /// Remove all rows that no longer exists in live data
        /// </summary>
        private void ClearUnusedInputRows(LiveFrame_v4 frame)
        {
            foreach (InputHierarchyRow row in new List<InputHierarchyRow>((IEnumerable<InputHierarchyRow>)rows.Values))
            {
                if (!frame.HasProfile(row.profileName))
                    rows.Remove(row.profileName);
            }

        }
    }
}