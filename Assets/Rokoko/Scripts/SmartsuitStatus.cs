using Rokoko.Smartsuit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rokoko.Smartsuit.Extra
{

    /// <summary>
    /// UI behavior that displays an actor's status.
    /// </summary>
    public class SmartsuitStatus : MonoBehaviour
    {
        /// <summary>
        /// Reference to a Smartsuit object.
        /// </summary>
        public Smartsuit smartsuit;

        /// <summary>
        /// Text prefab to use for status lines.
        /// </summary>
        public Text textPrefab;

        /// <summary>
        /// Container to place the status Texts.
        /// </summary>
        public Transform container;

        /// <summary>
        /// Maps a key string to a Text object.
        /// </summary>
        Dictionary<string, Text> dictToText = new Dictionary<string, Text>();

        /// <summary>
        /// Returns the Text that is mapped to the given key.
        /// If there is no mapping for the given key, a new text will be created,
        /// it will be mapped to the key and it will become child of the container.
        /// </summary>
        /// <param name="key">The key to get the text  for.</param>
        /// <returns>A Text object for the  given key.</returns>
        public Text GetText(string key)
        {
            if (!dictToText.ContainsKey(key))
            {
                Text newText = Instantiate(textPrefab);
                newText.gameObject.SetActive(true);
                newText.transform.SetParent(container);
                dictToText.Add(key, newText);
            }
            return dictToText[key];
        }

        /// <summary>
        /// Update text statuses.
        /// </summary>
        void Update()
        {
            GetText("suitname").text = "Smartsuit ID: " + smartsuit.HubID;
            GetText("profilename").text = "Profile name:" + (smartsuit.HasProfile ? smartsuit.dimensions.ProfileName : "Undefined");
            GetText("fps").text = "FPS:" + smartsuit.FPS.ToString();
            GetText("unicast").text = smartsuit.IsUnicast ? "Unicast" : "Broadcast";
        }
    }

}
