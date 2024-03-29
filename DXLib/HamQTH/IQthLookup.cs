﻿namespace DXLib.HamQTH
{
    public interface IQthLookup
    {
/*        public HamQTHResult? GetGeo(string callsign)
        {
            Task<HamQTHResult?> t = Task.Run<HamQTHResult?>(async () => await this.GetGeoAsync(callsign));
            t.Wait();
            return t.Result;
        }*/
        public abstract Task<HamQTHResult?> GetGeoAsync(string callsign,bool resolveDeeper = true);

    }
}