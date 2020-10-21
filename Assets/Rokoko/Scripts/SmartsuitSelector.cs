using Rokoko.Smartsuit.Networking;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rokoko.Smartsuit.Extra
{
    /// <summary>
    /// This class demonstrates how to implement a basic ui panel to 
    /// select which Smartsuit to assign to a specific SmartsuitActor.
    /// </summary>
    public class SmartsuitSelector : MonoBehaviour
    {
        /// <summary>
        /// The smartsuit actor that the selector will apply the name.
        /// </summary>
        public SmartsuitActor actor;

        /// <summary>
        /// Template object for instaniating new options for smartsuits in the list.
        /// </summary>
        public GameObject template;

        /// <summary>
        /// List of all options.
        /// </summary>
        List<GameObject> allOptions = new List<GameObject>();

        /// <summary>
        /// Container that will hold the options.
        /// </summary>
        public Transform container;
        

        /// <summary>
        /// Update the options based on the currently live suits.
        /// </summary>
        void Update()
        {
            for (int i = allOptions.Count; i < SmartsuitReceiver.I.LiveSuits.Length; i++)
            {
                GameObject suitOption = Instantiate(template) as GameObject;
                suitOption.GetComponentInChildren<Text>().text = SmartsuitReceiver.I.LiveSuits[i].Suitname;
                suitOption.transform.SetParent(container);
                suitOption.SetActive(true);
                allOptions.Add(suitOption);
            }
            for (int i = 0; i < SmartsuitReceiver.I.LiveSuits.Length; i++)
            {
                if (i < allOptions.Count)
                {
                    if (actor.HubID == SmartsuitReceiver.I.LiveSuits[i].Suitname)
                    {
                        allOptions[i].GetComponent<Image>().color = new Color32(61, 189, 134, 255);
                    } else
                    {
                        allOptions[i].GetComponent<Image>().color = new Color32(73, 73, 73, 255);
                    }
                    allOptions[i].GetComponentInChildren<Text>().text =
                        $"{SmartsuitReceiver.I.LiveSuits[i].Suitname} - {SmartsuitReceiver.I.LiveSuits[i].Fps} fps";
                }
            }
        }

        /// <summary>
        /// Set a smartsuit name.
        /// This function is using a Text child of the caller and gets it's value that corresponds to the suitname.
        /// </summary>
        /// <param name="caller"></param>
        public void SetSmartsuit(GameObject caller)
        {
            actor.SetHubId(caller.GetComponentInChildren<Text>().text.Substring(0, 3));
        }
    }
}
