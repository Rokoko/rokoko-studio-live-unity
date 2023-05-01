using Rokoko.Core;
using Rokoko.Helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rokoko.UI
{
    public class InputHierarchyRow : MonoBehaviour
    {
        public string profileName { get; private set; }

        [Header("Actor")]
        [SerializeField] private GameObject actorPanel = null;
        [SerializeField] private Image profileImage = null;
        [SerializeField] private Text profileText = null;
        [SerializeField] private Image faceImage = null;
        [SerializeField] private Image suitImage = null;
        [SerializeField] private Image leftGloveImage = null;
        [SerializeField] private Image rightGloveImage = null;
        [SerializeField] private Color inactiveColor = Color.gray;

        [Header("Prop")]
        [SerializeField] private GameObject propPanel = null;
        [SerializeField] private Image propImage = null;
        [SerializeField] private Text propText = null;

        public void UpdateRow(ActorFrame actorFrame)
        {
            actorPanel.SetActive(true);
            propPanel.SetActive(false);

            profileName = actorFrame.name;
            profileImage.color = actorFrame.color.ToColor();
            profileText.text = actorFrame.name;
            faceImage.color = actorFrame.meta.hasFace ? Color.white : inactiveColor;
            suitImage.color = actorFrame.meta.hasBody ? Color.white : inactiveColor;
            leftGloveImage.color = actorFrame.meta.hasLeftGlove ? Color.white : inactiveColor;
            rightGloveImage.color = actorFrame.meta.hasRightGlove ? Color.white : inactiveColor;
        }

        public void UpdateRow(CharacterFrame charFrame)
        {
            actorPanel.SetActive(false);
            propPanel.SetActive(true);

            profileName = charFrame.name;
            //propImage.color = propFrame.color.ToColor();
            propText.text = charFrame.name;
        }

        public void UpdateRow(PropFrame propFrame)
        {
            actorPanel.SetActive(false);
            propPanel.SetActive(true);

            profileName = propFrame.name;
            propImage.color = propFrame.color.ToColor();
            propText.text = propFrame.name;
        }
    }
}