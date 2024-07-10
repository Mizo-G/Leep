using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using User = Demo.Models.User;

namespace Demo.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly CosmosDB<User> _db;

    public UsersController(CosmosContainerFactory containerFactory)
    {
        _db = new CosmosDB<User>(containerFactory, "Leep.User");
    }


    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var query = new QueryDefinition(
                query: "SELECT * FROM c"
            );
            var result = await _db.QueryItems(query);
            if (result == null) { return BadRequest("Results are null"); }
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
        catch (Exception e)
        {
            return BadRequest($"An exception has occured - {e.Message}");
        }
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        return Ok($"Hello from MyController with ID = {id}");
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] User item)
    {
        try
        {
            var (status, result) = await _db.CreateItem(item);
            if (!status) return BadRequest("Failed to Create Item");
            return Ok(result?.Id);
        }
        catch (CosmosException ce)
        {
            Console.WriteLine(ce.Message);
            return BadRequest($"A database exception occured while creating data {ce.Message}");
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

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] User item)
    {
        try
        {
            var (status, result) = await _db.UpdateItem(item);
            if (!status) return BadRequest("Failed to Update Item");
            return Ok(result?.Id);
        }
        catch (CosmosException ce)
        {
            Console.WriteLine(ce.Message);
            return BadRequest($"A database exception occured while updating data {ce.Message}");
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, string partitionKey)
    {
        try
        {
            var status = await _db.DeleteItem(id, partitionKey);
            if (!status) return BadRequest("Failed to Delete Item");
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
