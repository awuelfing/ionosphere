using DXLib.HamQTH;
using System.Diagnostics.Metrics;
using System.Runtime.CompilerServices;
using System;
using System.Xml;
using System.Xml.Serialization;
using static System.Net.WebRequestMethods;
using System.Text.Json;
using System.Web;

namespace HamQTHTesting
{
    internal class Program
    {
        static void Main(string[] args)
        {
            HamQTHGeo hamQTHGeo = new HamQTHGeo();
            Task<HamQTHResult> t = hamQTHGeo.GetGeo("W1AW");
            t.Wait();
            Console.WriteLine(t.Result.SearchResult.nick);
            
        }
    }
}