namespace DXLib.HamQTH
{
    public abstract class QthLookup
    {
        public abstract Task<HamQTHResult?> GetGeo(string callsign);
        public QthLookup? Lower;
    }
}