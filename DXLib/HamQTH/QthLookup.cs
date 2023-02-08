namespace DXLib.HamQTH
{
    public abstract class QthLookup
    {
        public HamQTHResult? GetGeo(string callsign)
        {
            Task<HamQTHResult?> t = Task.Run<HamQTHResult?>(async () => await this.GetGeoAsync(callsign));
            return t.Result;
        }
        public abstract Task<HamQTHResult?> GetGeoAsync(string callsign);
        public QthLookup? Lower;
    }
}