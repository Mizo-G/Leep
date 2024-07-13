using Newtonsoft.Json;

namespace Demo.Models
{
    public enum UserType
    {
        Professional,
        Social_Entrepreneur
    }
    public enum UserRole
    {
        Admin,
        Member
    }

    public class User : ICosmosResource
    {
        [JsonProperty("id")]
        public string Id { get; set; } = "";
        public string PartitionKey => Id;
        [JsonProperty("userType")]
        public UserType UserType { get; set; }
        [JsonProperty("userName")]
        public string? UserName { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; } = "";
        [JsonProperty("profileImage")]
        public string? ProfileImage { get; set; }
        [JsonProperty("interests")]
        public List<string>? Interests { get; set; } = new List<string>();
        [JsonProperty("jobTitle")]
        public string? JobTitle { get; set; }
        [JsonProperty("sector")]
        public string? Sector { get; set; }
        [JsonProperty("subSector")]
        public string? SubSector { get; set; }
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
        public List<Followed>? Following { get; set; }
        [JsonProperty("contactInfo")]
        public ContactInfo? ContactInfo { get; set; }
        [JsonProperty("keepSocialsPrivate")]
        public bool keepSocialsPrivate { get; set; } = false;
        [JsonProperty("keepPersonalInfoPrivate")]
        public bool keepPersonalInfoPrivate { get; set; } = true;
        [JsonProperty("emailVerified")]
        public bool EmailVerified { get; set; } = false;
        [JsonProperty("hash")]
        public string? Hash { get; set; }
        [JsonProperty("completionPercentage")]
        public int CompletionPercentage { get; set; }

        public User()
        {
        }

        public User(string email)
        {
            ContactInfo = new ContactInfo { Email = email };
        }

        public (bool, string) IsEssentialInfoFilled()
        {
            var user = this;
            if (String.IsNullOrWhiteSpace(user.Name)) return (false, $"{nameof(user.Name)} is required, but was not provided");
            if (String.IsNullOrWhiteSpace(user.UserName)) return (false, $"{nameof(user.UserName)} is required, but was not provided");
            if (!user.EmailVerified) return (false, $"Email needs to be verified.");
            if (user.ContactInfo is null) return (false, $"An email needs to be provided and verified before User can be added");
            if (String.IsNullOrWhiteSpace(user.ContactInfo.Email)) return (false, $"An email needs to be provided and verified before User can be added");

            return (true, "");
        }

        public int CalculateCompletionPercentage()
        {
            int toComplete = 10;
            int completed = 0;

            var user = this;
            if (!String.IsNullOrWhiteSpace(user.Name)) completed++;
            if (!String.IsNullOrWhiteSpace(user.UserName)) completed++;
            if (user.EmailVerified) completed++;
            if (!String.IsNullOrWhiteSpace(user.ContactInfo?.Email)) completed++;

            // TODO =>
            // add the rest of the feilds later

            return completed * 100 / toComplete;
        }

    }
}
