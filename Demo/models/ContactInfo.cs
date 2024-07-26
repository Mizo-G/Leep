using System.Text.Json.Serialization;

public class ContactInfo
{
    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; set; }
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    [JsonPropertyName("linkedInProfile")]
    public string? LinkedInProfile { get; set; }
    [JsonPropertyName("twitterProfile")]
    public string? Twitter { get; set; }
    [JsonPropertyName("instagramProfile")]
    public string? instagramProfile { get; set; }
}
