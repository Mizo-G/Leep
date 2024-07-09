using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using User = Demo.Models.User;

namespace Demo.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase 
{
   private readonly CosmosDB db;

    public UsersController(CosmosContainerFactory containerFactory)
    {
        db = new CosmosDB(containerFactory, "Leep.User");
    }


    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var query = new QueryDefinition(
                query: "SELECT * FROM c"
            );
            var result = await db.QueryItems<User>(query);
            if (result == null) { return BadRequest("Results are null"); }
            if (result.Count == 0) { return NoContent(); }
            return Ok(result);
        }
        catch (CosmosException ce)
        {
            Console.WriteLine(ce.Message);
            return BadRequest("A database exception occured while retrieving data");
        }
        catch (ArgumentNullException e)
        {
            return BadRequest($"A non nullable value was found to be null - {e.Message}");
        }
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        return Ok($"Hello from MyController with ID = {id}");
    }

    [HttpPost]
    public IActionResult Create(int id)
    {
        return Ok($"Hello from MyController with ID = {id}");
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id)
    {
        return Ok($"Hello from MyController with ID = {id}");
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        return Ok($"Hello from MyController with ID = {id}");
    }
}