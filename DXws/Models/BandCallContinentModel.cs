using System.Text.Json.Serialization;

namespace DXws.Models
{
    public class BandModel
    {
        [JsonPropertyOrder(0)]
        public string Band { get; set; } = string.Empty;
        [JsonPropertyName("calls")]
        public List<CallModel>? InnerCall { get; set; }
    }
    public class CallModel
    {
        [JsonPropertyOrder(0)]
        public string Call { get; set; } = string.Empty;
        [JsonPropertyName("continents")]
        public List<ContinentModel>? InnerContinent { get; set; }
    }
    public class ContinentModel
    {
        public string Continent { get; set; } = string.Empty;
        public int Count { get; set; } = 0;
    }
}
