using System.Security.Cryptography;
using Demo.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using User = Demo.Models.User;

public class Otp : ICosmosResource
{
    [JsonIgnore]
    public string Id { get; set; } = "";
    [JsonProperty("userId")]
    public string UserId { get; set; }
    public string PartitionKey => UserId;
    [JsonProperty("email")]
    public string Email { get; set; }
    [JsonProperty("code")]
    public string Code { get; set; }
    [JsonProperty("createdDate")]
    public DateTime CreatedDate { get; set; }
    public int ttl { get; set; }


    public Otp(string userId, string email, string code)
    {
        UserId = userId;
        Code = code;
        Email = email;
        CreatedDate = DateTime.UtcNow;
        ttl = 60 * 10; // 10 minutes, then cosmos auto deletes the record
    }
}

public class AuthenticationService
{
    private readonly CosmosDB<User> _userDb;
    private readonly CosmosDB<Otp> _otpDb;

    public AuthenticationService(CosmosContainerFactory containerFactory)
    {
        _userDb = new CosmosDB<User>(containerFactory, "Leep.Users");
        _otpDb = new CosmosDB<Otp>(containerFactory, "Leep.Otp");
    }

    public async Task<(User?, string)> LoginInWithEmail(string email, string password)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.email = @Email")
            .WithParameter("@Email", email);
        var users = await _userDb.QueryItems(query);

        if (users.Count == 0) return (null, $"Couldn't find User with Email {email}");
        if (users.Count > 1) return (null, @$"More than one User with Email {email}. 
                                            Time to Panic! Call an Admin!
                                            Or make sure Email is correct first.");

        var user = users.First();
        if (!user.EmailVerified) return (null, "Email not verified.");

        if (user.Hash is null) return (null, "User hash is null. Make Sure User has submitted Password");
        if (VerifyPassword(password, user.Hash)) return (user, "success");

        return (null, "Password is incorrect. Please try again.");
    }

    public void LoginInWithLinkedIn()
    {
        throw new NotImplementedException();
    }

    public void RegisterWithLinkedIn()
    {
        throw new NotImplementedException();
    }

    public async Task<(bool, string)> RegisterUnverifiedUser(string email)
    {
        bool status;
        var user = new User(email);
        (status, _) = await _userDb.CreateItem(user);
        if (!status) return (false, $"Couldn't find user with email {email}");
        status = await SendOtp(user.Id, email);
        if (!status) return (false, $"Couldn't send otp to email {email}");
        return (true, "");
    }

    private string GenerateOtp()
    {
        var random = new Random();
        return random.Next(100_000, 1_000_000).ToString();
    }

    private async Task<bool> SendOtp(string userId, string email)
    {
        bool status;
        var code = GenerateOtp();
        var otp = new Otp(userId, email, code);
        (status, _) = await _otpDb.CreateItem(otp);
        return status;
    }

    public async Task<bool> ResendOtp(string userId, string email)
    {
        bool status;
        var code = GenerateOtp();
        var otp = new Otp(userId, email, code);
        (status, _) = await _otpDb.UpsertItem(otp);
        return status;
    }

    public async Task<bool> VerifyOtp(string userId, string code)
    {
        var otp = await _otpDb.ReadItem(userId, userId);
        if (otp.Code == code) return true;
        return false;
    }

    public async Task<bool> RegisterUserPassword(string userId, string password)
    {
        var user = await _userDb.ReadItem(userId);
        if (user == null) return false;

        user.Hash = HashPassword(password);

        var (status, _) = await _userDb.UpdateItem(user.Id, user.PartitionKey, user);
        return status;
    }

    public async Task<bool> VerifyEmail(string userId)
    {
        var user = await _userDb.ReadItem(userId);
        if (user == null) return false;

        user.EmailVerified = true;
        var (status, _) = await _userDb.UpdateItem(user.Id, user.PartitionKey, user);

        return status;
    }

    private bool VerifyPassword(string password, string passwordHash)
    {
        // Extract the bytes
        byte[] hashBytes = Convert.FromBase64String(passwordHash);

        // Get the salt
        byte[] salt = new byte[16];
        Array.Copy(hashBytes, 0, salt, 0, 16);

        // Hash the password with the salt
        byte[] hash = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 32
        );
        // Compare the results
        for (int i = 0; i < 32; i++)
        {
            if (hashBytes[i + 16] != hash[i])
            {
                return false;
            }
        }

        return true;
    }

    private string HashPassword(string password)
    {
        // Generate a random salt
        byte[] salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // Hash the password with the salt
        byte[] hash = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 32
        );

        // Combine the salt and password bytes for storage
        byte[] hashBytes = new byte[48];
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 32);

        // Convert to Base64 string
        return Convert.ToBase64String(hashBytes);
    }

}
