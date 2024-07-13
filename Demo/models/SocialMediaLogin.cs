using Newtonsoft.Json;

public class SocialMediaLogin
{
    [JsonProperty("provider")]
    public string Provider { get; set; } = "";

    [JsonProperty("providerUserId")]
    public string ProviderUserId { get; set; } = "";
}