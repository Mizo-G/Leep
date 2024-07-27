using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using User = Demo.Models.User;

public record SignInRequest(string Email, string Password);
public record RegisterPasswordRequest(string Id, string Password);

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly CosmosDB<User> _db;
    private readonly CosmosDB<Otp> _dbOtp;
    private readonly AuthService _authService;

    public AuthController(CosmosContainerFactory containerFactory)
    {
        _db = new CosmosDB<User>(containerFactory, "Leep.User");
        _dbOtp = new CosmosDB<Otp>(containerFactory, "Leep.User");

        _authService = new AuthService(_db, _dbOtp);
    }


    [HttpPost("signin")]
    public async Task<IActionResult> SignInWithEmail([FromBody] SignInRequest request)
    {
        try
        {
            var (email, password) = request;
            var (user, reasonIfFailed) = await _authService.LoginInWithEmail(email, password);
            if (user == null) return Unauthorized(reasonIfFailed);
            return Ok(user);
        }
        catch (CosmosException ce)
        {
            Console.WriteLine(ce.Message);
            return BadRequest($"A database exception occured while deleting data {ce.Message}");
        }
        catch (ArgumentNullException e)
        {
            return BadRequest($"A non nullable value was found to be null - {e.Message}");
        }
        catch (Exception e)
        {
            return BadRequest($"An exception has occured - {e.Message}");
        }
    }

    [HttpPost("signin/linkedin")]
    public IActionResult SignInWithSocialMedia()
    {
        throw new NotImplementedException();
    }

    [HttpPost("register-unverified-user")]
    public async Task<IActionResult> RegisterUnverified([FromBody] string email)
    {
        try
        {
            var (status, codeOrErr) = await _authService.RegisterUnverifiedUser(email);
            if (!status) return BadRequest($"Failed to register user. Reason: {codeOrErr}");
            //return code to the client for testing 
            return Ok(codeOrErr);
        }
        catch (CosmosException ce)
        {
            Console.WriteLine(ce.Message);
            return BadRequest($"A database exception occured while registering user {ce.Message}");
        }
        catch (ArgumentNullException e)
        {
            return BadRequest($"A non nullable value was found to be null - {e.Message}");
        }
        catch (Exception e)
        {
            return BadRequest($"An exception has occured - {e.Message}");
        }
    }

    [HttpPost("resendotp/{userId}")]
    public async Task<IActionResult> ResendOtp(string userId, [FromBody] string email)
    {
        try
        {
            var (status, code) = await _authService.ResendOtp(userId, email);
            if (!status) return BadRequest($"Failed to resend otp to email: {email}");
            //return code to client for testing
            return Ok(code);
        }
        catch (CosmosException ce)
        {
            Console.WriteLine(ce.Message);
            return BadRequest($"A database exception occured while resending otp {ce.Message}");
        }
        catch (ArgumentNullException e)
        {
            return BadRequest($"A non nullable value was found to be null - {e.Message}");
        }
        catch (Exception e)
        {
            return BadRequest($"An exception has occured - {e.Message}");
        }
    }

    [HttpPost("register-user-password")]
    public async Task<IActionResult> RegisterPassword([FromBody] RegisterPasswordRequest request)
    {
        try
        {
            var (id, password) = request;
            var status = await _authService.RegisterUserPassword(id, password);
            if (!status) return BadRequest("Failed to register user");
            return Ok();
        }
        catch (CosmosException ce)
        {
            Console.WriteLine(ce.Message);
            return BadRequest($"A database exception occured while registering password {ce.Message}");
        }
        catch (ArgumentNullException e)
        {
            return BadRequest($"A non nullable value was found to be null - {e.Message}");
        }
        catch (Exception e)
        {
            return BadRequest($"An exception has occured - {e.Message}");
        }
    }

    [HttpPost("verify/{userId}")]
    public async Task<IActionResult> VerifyEmail(string userId, [FromBody] string code)
    {
        try
        {
            bool status;
            status = await _authService.VerifyOtp(userId, code);
            if (!status) return Forbid("Failed to verify otp");
            status = await _authService.VerifyEmail(userId);
            if (!status) return BadRequest("Failed to verify email");
            return Ok();
        }
        catch (CosmosException ce)
        {
            Console.WriteLine(ce.Message);
            return BadRequest($"A database exception occured while verifying data {ce.Message}");
        }
        catch (ArgumentNullException e)
        {
            return BadRequest($"A non nullable value was found to be null - {e.Message}");
        }
        catch (Exception e)
        {
            return BadRequest($"An exception has occured - {e.Message}");
        }
    }
}
