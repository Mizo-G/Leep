using System.Text.Json.Serialization;
namespace Demo.Models
{
    public enum UserRole
    {
        Admin,
        Member
    }

    public class User : ICosmosResource
    {
        //TODO => Add Validation tags. 
        //  and required declarations.  
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [JsonPropertyName("userId")]
        public string UserId { get; set; } = "";
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = "";
        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = "";
        [JsonPropertyName("yearsOfExperience")]
        public int YearsOfExperience { get; set; } = 0;
        [JsonPropertyName("country")]
        public string Country { get; set; } = "";
        [JsonPropertyName("governorate")]
        public string Governorate { get; set; } = "";
        [JsonPropertyName("jobTitle")]
        public string JobTitle { get; set; } = "";
        [JsonPropertyName("email")]
        public string Email { get; set; } = "";
        [JsonPropertyName("company")]
        public string Company { get; set; } = "";
        [JsonPropertyName("emailVerified")]
        public bool EmailVerified { get; set; } = false;
        [JsonPropertyName("hash")]
        public string? Hash { get; set; }
        [JsonPropertyName("createdDate")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [JsonPropertyName("docType")]
        public string DocType { get; set; } = "user";

        public User()
        {
            UserId = Id;
        }

        public void CopyEssentialData(User oldUser)
        {
            Hash = oldUser.Hash;
            DocType = oldUser.DocType;
            EmailVerified = oldUser.EmailVerified;
        }

        public (bool, string) IsEssentialInfoFilled()
        {
            var user = this;

            if (String.IsNullOrWhiteSpace(user.FirstName)) return (false, $"{nameof(user.FirstName)} is required, but was not provided");
            if (String.IsNullOrWhiteSpace(user.LastName)) return (false, $"{nameof(user.LastName)} is required, but was not provided");
            if (String.IsNullOrWhiteSpace(user.Email)) return (false, $"An email needs to be provided and verified before a User can be added");
            if (String.IsNullOrWhiteSpace(user.Country)) return (false, $"{nameof(user.Country)} is required, but was not provided");
            if (String.IsNullOrWhiteSpace(user.Governorate)) return (false, $"{nameof(user.Governorate)} is required, but was not provided");
            if (String.IsNullOrWhiteSpace(user.JobTitle)) return (false, $"{nameof(user.JobTitle)} is required, but was not provided");
            if (String.IsNullOrWhiteSpace(user.Company)) return (false, $"{nameof(user.JobTitle)} is required, but was not provided");
            if (!user.EmailVerified) return (false, $"Email needs to be verified.");

            return (true, "");
        }

        public int CalculateCompletionPercentage()
        {
            // Move to Personl Profile later?
            int toComplete = 7; //currently 7 required params

            // starts at one because YearsOfExprince 
            // gets initilized to zero which means
            // it is always valid
            int completed = 1;

            var user = this;

            if (!String.IsNullOrWhiteSpace(user.FirstName)) completed++;
            if (!String.IsNullOrWhiteSpace(user.LastName)) completed++;
            if (user.EmailVerified) completed++;
            if (!String.IsNullOrWhiteSpace(user.Country)) completed++;
            if (!String.IsNullOrWhiteSpace(user.Governorate)) completed++;
            if (!String.IsNullOrWhiteSpace(user.JobTitle)) completed++;

            return completed * 100 / toComplete;
        }

    }
}
