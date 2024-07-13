using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

public record SignInRequest(string Email, string Password);

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
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
            var (status, err) = await _authService.RegisterUnverifiedUser(email);
            if (!status) return BadRequest($"Failed to register user. Reason: {err}");
            return Ok();
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

    [HttpPost("resendotp/{id}")]
    public async Task<IActionResult> ResendOtp(string id, [FromBody] string email)
    {
        try
        {
            var status = await _authService.ResendOtp(id, email);
            if (!status) return BadRequest("Failed to register user");
            return Ok();
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

    [HttpPost("register-user-password")]
    public async Task<IActionResult> RegisterPassword([FromBody] SignInRequest request)
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

    [HttpPost("verify/{id}")]
    public async Task<IActionResult> VerifyEmail(string id, [FromBody] string code)
    {
        try
        {
            bool status;
            status = await _authService.VerifyOtp(id, code);
            if (!status) return Forbid("Failed to verify otp");
            status = await _authService.VerifyEmail(id);
            if (!status) return BadRequest("Failed to verify email");
            return Ok();
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
}
