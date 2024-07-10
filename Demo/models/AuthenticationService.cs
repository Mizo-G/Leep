
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Azure.Cosmos;
using User = Demo.Models.User;
public class AuthenticationService
{
    private readonly CosmosDB<User> _userDb;

    public AuthenticationService(CosmosContainerFactory containerFactory)
    {
        _userDb = new CosmosDB<User>(containerFactory, "Leep.Users");
    }

    public async Task<(User?, string)> SignInWithEmail(string email, string password)
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

    public void SignInWithLinkedIn()
    {
        throw new NotImplementedException();
    }

    public void RegisterWithLinkedIn()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> RegisterUnverifiedUser(string email)
    {
        var user = new User(email);
        var (status, _) = await _userDb.CreateItem(user);
        return status;
    }

    public async Task<bool> RegisterUserPassword(string userId, string password)
    {
        var user = await _userDb.ReadItem(userId);
        if (user == null) return false;

        user.Hash = HashPassword(password);
        
        var (status, _) = await _userDb.UpdateItem(user);
        return status;
    }

    public async Task<bool> VerifyEmail(string userId)
    {
        var user = await _userDb.ReadItem(userId);
        if (user == null) return false;

        user.EmailVerified = true;
        var (status, _) = await _userDb.UpdateItem(user);

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
