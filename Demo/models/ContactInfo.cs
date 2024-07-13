using Newtonsoft.Json;

public class ContactInfo{
    [JsonProperty("phoneNumber")]
    public string? PhoneNumber { get; set; }
    [JsonProperty("email")]
    public string? Email { get; set; }
    [JsonProperty("linkedInProfile")] 
    public string? LinkedInProfile  { get; set; }
    [JsonProperty("twitterProfile")]
    public string? Twitter { get; set; }
    [JsonProperty("instagramProfile")]
    public string? instagramProfile  { get; set; }
}