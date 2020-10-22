using Rokoko.Serializers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rokoko.UI
{
    public class InputHierarchyRow : MonoBehaviour
    {
        public string profileName { get; private set; }

        [SerializeField] private Image profileImage = null;
        [SerializeField] private Text profileText = null;
        [SerializeField] private Image faceImage = null;
        [SerializeField] private Image suitImage = null;
        [SerializeField] private Image leftGloveImage = null;
        [SerializeField] private Image rightGloveImage = null;
        [SerializeField] private Color inactiveColor = Color.gray;

        public void UpdateRow(ActorFrame actorFrame)
        {
            profileName = actorFrame.name;
            profileImage.color = actorFrame.color.ToColor();
            profileText.text = actorFrame.name;
            faceImage.color = actorFrame.meta.hasFace ? Color.white : inactiveColor;
            suitImage.color = actorFrame.meta.hasBody ? Color.white : inactiveColor;
            leftGloveImage.color = actorFrame.meta.hasLeftGlove ? Color.white : inactiveColor;
            rightGloveImage.color = actorFrame.meta.hasRightGlove ? Color.white : inactiveColor;
        }
    }
}