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
                query: "SELECT * FROM c WHERE c.docType = 'user'"
            );
            var result = await _db.QueryItems(query);
            if (result == null) { return BadRequest("Results are null"); }
            return Ok(result);
        }
        catch (CosmosException ce)
        {
            return NotFound($"A database exception occured while retrieving data: {ce.Message}");
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

    [HttpGet("{id}/{pk}")]
    public async Task<IActionResult> GetById(string id, string pk)
    {
        try
        {
            var item = await _db.ReadItem(id, pk);
            if (String.IsNullOrWhiteSpace(item.Id)) return NotFound($"Couldn't find User with id {id} and partition key {pk}");
            return Ok(item);
        }
        catch (CosmosException ce)
        {
            return NotFound($"A database exception occured while retrieving data - {ce.Message}");
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

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] User item)
    {
        try
        {
            bool status;
            (status, var err) = item.IsEssentialInfoFilled();
            if (!status) return BadRequest($"{err}. Please provied all required inforamation.");
            item.CompletionPercentage = item.CalculateCompletionPercentage();
            (status, var result) = await _db.CreateItem(item, item.UserId);
            if (!status) return BadRequest("Failed to Create Item");
            return Ok(result?.Id);
        }
        catch (CosmosException ce)
        {
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

    [HttpPut("{id}/{partitionKey}")]
    public async Task<IActionResult> Update(string id, string partitionKey, [FromBody] User item)
    {
        try
        {
            bool status;
            (status, var err) = item.IsEssentialInfoFilled();
            if (!status) return BadRequest($"{err}. Please provied all required inforamation.");
            item.CompletionPercentage = item.CalculateCompletionPercentage();
            (status, var result) = await _db.UpdateItem(id, partitionKey, item);
            if (!status) return BadRequest("Failed to Update Item");
            return Ok(result?.Id);
        }
        catch (CosmosException ce)
        {
            return NotFound($"A database exception occured while updating data {ce.Message}");
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

    [HttpDelete("{id}/{partitionKey}")]
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
            return NotFound($"A database exception occured while deleting data {ce.Message}");
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
