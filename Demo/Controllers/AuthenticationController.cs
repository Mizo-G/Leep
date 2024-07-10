using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthenticationService _authService;

    public AuthController(AuthenticationService authService)
    {
        _authService = authService;
    }

    [HttpPost("signin")]
    public async Task<IActionResult> SignInWithEmail([FromBody] string email, [FromBody] string password)
    {
        try
        {
            var (user, reasonIfFailed) = await _authService.SignInWithEmail(email, password);
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
            var result = await _authService.RegisterUnverifiedUser(email);
            if (!result) return BadRequest("Failed to register user");
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
    public async Task<IActionResult> RegisterPassword([FromBody] string id, [FromBody] string password)
    {
        try
        {
            var result = await _authService.RegisterUserPassword(id, password);
            if (!result) return BadRequest("Failed to register user");
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

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyEmail([FromBody] string id)
    {
        try
        {
            var result = await _authService.VerifyEmail(id);
            if (!result) return BadRequest("Failed to verify email");
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

