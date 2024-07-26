using System.Text.Json.Serialization;

public class SocialMediaLogin
{
    [JsonPropertyName("provider")]
    public string Provider { get; set; } = "";

    [JsonPropertyName("providerUserId")]
    public string ProviderUserId { get; set; } = "";
}
