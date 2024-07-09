using Newtonsoft.Json;
namespace Demo.Models
{

public enum UserType {
    Professional,
    Social_Entrepreneur
}

public class User : ICosmosResource
{

    [JsonProperty("name")]
    public string Name { get; set; } = "";
    [JsonProperty("interests")]
    public List<string> Interests { get; set; } = new List<string>();

    public User()
    {
    }

    public User(string id, string name, List<string> interests)
    {
        Id = id;
        Name = name;
        Interests = interests;
    }

    static public User Copy(User u)
    {
        var newUser = new User
        {
            Name = u.Name
        };
        if (u.Interests.Count() > 0)
        {
            newUser.Interests = u.Interests.Select(i => i).ToList();
        }
        return newUser;
    }
}
}