
using Newtonsoft.Json;

public class Location
{
    [JsonProperty("country")]
    public string Country  { get; set; }

    [JsonProperty("Governrate")]
    public string Governrate  { get; set; }

    public Location(string country, string governrate)
    {
        Country = country;
        Governrate = governrate;
    }
}
