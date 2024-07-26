
using System.Text.Json.Serialization;

public class Location
{
    [JsonPropertyName("country")]
    public string Country { get; set; }

    [JsonPropertyName("governorate")]
    public string Governorate { get; set; }

    public Location(string country, string governorate)
    {
        Country = country;
        Governorate = governorate;
    }
}
