using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DXLib.HamQTH
{
    [XmlRootAttribute("HamQTH", Namespace = "https://www.hamqth.com")]
    public class HamQTHResult
    {
        [XmlElement("search")]
        public HamQTHSearchResult? SearchResult { get; set; }
    }
    public class HamQTHSearchResult
    {
        public string? callsign { get; set; }
        public string? nick { get; set; }
        public string? qth { get; set; }
        public string? country { get; set; }
        public string? adif { get; set; }
        public string? itu { get; set; }
        public string? CQ { get; set; }
        public string? grid { get; set; }
        public string? adr_name { get; set; }
        public string? adr_street1 { get; set; }
        public string? adr_street2 { get; set; }
        public string? adr_street3 { get; set; }
        public string? adr_city { get; set; }
        public string? adr_zip { get; set; }
        public string? adr_country { get; set; }
        public string? adr_adif { get; set; }
        public string? district { get; set; }
        public string? us_state { get; set; }
        public string? us_county { get; set; }
        public string? oblast { get; set; }
        public string? dok { get; set; }
        public string? iota { get; set; }
        public string? qsl_via { get; set; }
        public string? lotw { get; set; }
        public string? eqsl { get; set; }
        public string? qsl { get; set; }
        public string? qsldirect { get; set; }
        public string? email { get; set; }
        public string? jabber { get; set; }
        public string? icq { get; set; }
        public string? msn { get; set; }
        public string? skype { get; set; }
        public string? birth_year { get; set; }
        public string? lic_year { get; set; }
        public string? picture { get; set; }
        public string? latitude { get; set; }
        public string? longitude { get; set; }
        public string? continent { get; set; }
        public string? utc_offset { get; set; }
        public string? facebook { get; set; }
        public string? twitter { get; set; }
        public string? gplus { get; set; }
        public string? youtube { get; set; }
        public string? linkedin { get; set; }
        public string? flicker { get; set; }
        public string? vimeo { get; set; }

    }
}
