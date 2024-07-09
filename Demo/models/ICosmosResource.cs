using Newtonsoft.Json;

namespace Demo.Models
{
public class ICosmosResource
{
    [JsonProperty("id")]
    public string Id { get; set; } = "";
}
}