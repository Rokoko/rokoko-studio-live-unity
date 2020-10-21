namespace Rokoko.CommandAPI
{
    public class StudioCommandAPI : StudioCommandAPIBase
    {
        public string ipAddress;
        protected override string IP => ipAddress;
        protected override RequestData GetRequestData() => new RequestData();
    }
}