using System.Drawing;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Routing;
using Newtonsoft.Json;

namespace Demo.Models
{

public enum UserType {
    Professional,
    Social_Entrepreneur
}

public class Location {

}

public interface IFollowable {

}

public class Followed {
    public string ContentId { get; }
    public DateTime FollowingSince { get; }
    public bool Follow<T>(T content) where T : IFollowable : ICosmosResource
    {
        ContentId = content.Id;
        FollowingSince = DateTime.UtcNow;
    }
}

public class User : ICosmosResource : IFollowable
{
    [JsonProperty("userType")]
    public UserType UserType { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; } = "";
    [JsonProperty("profileImage")]
    public string ProfileImage { get; set; }
    [JsonProperty("interests")]
    public List<string>? Interests { get; set; } = new List<string>();
    [JsonProperty("jobTitle")]
    public string? JobTitle  { get; set; }
    [JsonProperty("sector")]
    public string? Sector { get; set; }
    [JsonProperty("subSector")]
    public strings? SubSector { get; set; }
    [JsonProperty("yearsOfExperience")]
    public int? levelOfExperience { get; set; }
    [JsonProperty("location")]
    public Location? Location { get; set; }
    [JsonProperty("aboutMe")]
    public string? AboutMe { get; set; }
    [JsonProperty("introVideo")]
    public string? introVideo { get; set; }
    [JsonProperty("previousExperiences")]
    public List<string>? PreviousExperiences { get; set; }
    [JsonProperty("experiencedIn")]
    public List<string>? ExperiencedIn { get; set; }
    [JsonProperty("needHelpWith")]
    public List<string>? NeedHelpWith { get; set; }
    [JsonProperty("following")]
    public List<Followed> Following { get; set; }
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