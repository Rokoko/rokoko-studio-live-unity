using UnityEngine;
using UnityEngine.UI;

namespace Rokoko.CommandAPI
{
    public class StudioCommandAPI : StudioCommandAPIBase
    {
        [SerializeField] private Text responseText = null;

        public string ipAddress;
        protected override string IP => ipAddress;
        protected override RequestData GetRequestData() => new RequestData();

        private void Start()
        {
            SetStatusText("");
        }

        protected override void OnCommmandResponse(ResponseMessage response)
        {
            base.OnCommmandResponse(response);
            SetStatusText(response.description);
        }

        protected override void OnCommmandError(string error)
        {
            base.OnCommmandError(error);
            SetStatusText($"{error}\nPlease make sure Rokoko Studio is running and Command API is enabled (Menu->Settings->Command API->Enabled).\nCheck also the receiving port and API key in both Rokoko Studio and Unity plugin.");
        }

        private void SetStatusText(string text)
        {
            if (responseText == null) return;
            responseText.transform.parent.gameObject.SetActive(!string.IsNullOrEmpty(text));
            responseText.text = text;
            LayoutRebuilder.ForceRebuildLayoutImmediate(responseText.transform.parent as RectTransform);
        }
    }
}