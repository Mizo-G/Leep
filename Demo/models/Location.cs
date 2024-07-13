
using Newtonsoft.Json;

public class Location
{
    [JsonProperty("country")]
    public string Country  { get; set; }

    [JsonProperty("governorate")]
    public string Governorate  { get; set; }

    public Location(string country, string governorate)
    {
        Country = country;
        Governorate = governorate;
    }
}
