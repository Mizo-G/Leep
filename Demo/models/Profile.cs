using System.Text.Json.Serialization;
namespace Demo.Models
{

    public class Profile : ICosmosResource
    {
        //TODO => Add Validation tags. 
        //  and required declarations.  
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [JsonPropertyName("userId")]
        public string ProfileId { get; set; } = "";
        [JsonPropertyName("userName")]
        public string? ProfileName { get; set; }
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = "";
        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = "";
        [JsonPropertyName("profileImage")]
        public string? ProfileImage { get; set; }
        [JsonPropertyName("interests")]
        public List<string>? Interests { get; set; } = new List<string>();
        [JsonPropertyName("jobTitle")]
        public string JobTitle { get; set; } = "";
        [JsonPropertyName("sector")]
        public string? Sector { get; set; }
        [JsonPropertyName("subSector")]
        public string? SubSector { get; set; }
        [JsonPropertyName("yearsOfExperience")]
        public int YearsOfExperience { get; set; } = 0;
        [JsonPropertyName("country")]
        public string Country { get; set; } = "";
        [JsonPropertyName("governorate")]
        public string Governorate { get; set; } = "";
        [JsonPropertyName("aboutMe")]
        public string? AboutMe { get; set; }
        [JsonPropertyName("introVideo")]
        public string? introVideo { get; set; }
        [JsonPropertyName("previousExperiences")]
        public List<string>? PreviousExperiences { get; set; }
        [JsonPropertyName("experiencedIn")]
        public List<string>? ExperiencedIn { get; set; }
        [JsonPropertyName("needHelpWith")]
        public List<string>? NeedHelpWith { get; set; }
        [JsonPropertyName("contactInfo")]
        public ContactInfo? ContactInfo { get; set; }
        [JsonPropertyName("keepSocialsPrivate")]
        public bool keepSocialsPrivate { get; set; } = false;
        [JsonPropertyName("keepPersonalInfoPrivate")]
        public bool keepPersonalInfoPrivate { get; set; } = true;
        [JsonPropertyName("emailVerified")]
        public bool EmailVerified { get; set; } = false;
        [JsonPropertyName("hash")]
        public string? Hash { get; set; }
        [JsonPropertyName("completionPercentage")]
        public int CompletionPercentage { get; set; }
        [JsonPropertyName("createdDate")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [JsonPropertyName("docType")]
        public string DocType { get; set; } = "user";
        [JsonPropertyName("email")]
        public string Email { get; set; } = "";

        public Profile()
        {
            ProfileId = Id;
        }

        public (bool, string) IsEssentialInfoFilled()
        {
            // Unnecessary method, marking the fields as required,
            // is enough to guarantee Essential Info is Filled.
            // Add Validation messages as attributes on top of 
            // every field and return them to the controller.
            // Or make them nullable and call this method before
            // adding any: new Profile();
            var user = this;

            if (String.IsNullOrWhiteSpace(user.FirstName)) return (false, $"{nameof(user.FirstName)} is required, but was not provided");
            if (String.IsNullOrWhiteSpace(user.LastName)) return (false, $"{nameof(user.LastName)} is required, but was not provided");
            if (String.IsNullOrWhiteSpace(user.Email)) return (false, $"An email needs to be provided and verified before Profile can be added");
            if (String.IsNullOrWhiteSpace(user.Country)) return (false, $"{nameof(user.Country)} is required, but was not provided");
            if (String.IsNullOrWhiteSpace(user.Governorate)) return (false, $"{nameof(user.Governorate)} is required, but was not provided");
            if (String.IsNullOrWhiteSpace(user.JobTitle)) return (false, $"{nameof(user.JobTitle)} is required, but was not provided");
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
