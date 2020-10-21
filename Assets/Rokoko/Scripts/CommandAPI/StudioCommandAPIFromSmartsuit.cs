namespace Rokoko.CommandAPI
{
    /// <summary>
    /// This component provides access to Studio's Command API.
    /// </summary>
    public class StudioCommandAPIFromSmartsuit : StudioCommandAPIBase
    {
        public Smartsuit.Smartsuit smartsuit;
        protected override string IP => smartsuit.IpAddress();
        protected override RequestData GetRequestData() => new RequestData {smartsuit = smartsuit.HubID};
    }
}