using System.Net;
using Demo.Models;
using Microsoft.Azure.Cosmos;

public class CosmosDB<T> where T : ICosmosResource
{
    private readonly Container _container;

    public CosmosDB(CosmosContainerFactory containerFactory, string containerName)
    {
        _container = containerFactory.GetContainer(containerName) ?? throw new ArgumentNullException(nameof(containerName), "Container is null");
    }

    public async Task<List<T>> QueryItems(QueryDefinition query, QueryRequestOptions? opts = null)
    {
        var items = new List<T>();
        opts ??= new QueryRequestOptions();

        using var feed = _container.GetItemQueryIterator<T>(query, requestOptions: opts);
        if (feed == null) throw new ArgumentNullException(nameof(feed), "Feed is null");

        while (feed.HasMoreResults)
        {
            var data = await feed.ReadNextAsync();
            if (data == null) throw new ArgumentNullException(nameof(data), "Data is null");
            if (data.Count < 1) break;

            foreach (var item in data)
            {
                if (item == null) throw new ArgumentNullException(nameof(item), "Data item is null");
                items.Add(item);
            }
        }
        return items;
    }

    public async Task<T> ReadItem(string id, string partitionKey = "")
    {
        if (String.IsNullOrWhiteSpace(partitionKey)) partitionKey = id;
        var result = await _container.ReadItemAsync<T>(id, new PartitionKey(partitionKey));
        if (result == null) throw new ArgumentNullException(nameof(result), "Read Item Result is null");
        return result.Resource;
    }

    public async Task<(bool, T?)> CreateItem(T item)
    {
        var result = await _container.CreateItemAsync(item, new PartitionKey(item.PartitionKey));
        if (result == null) throw new ArgumentNullException(nameof(result), "Create Result is null");
        if (result.Resource == null) throw new ArgumentNullException(nameof(result.Resource), "Result Source is null");
        var code = result.StatusCode;
        var resource = result.Resource;
        Console.WriteLine(code);
        Console.WriteLine(resource.Id);
        if (!String.IsNullOrWhiteSpace(resource.Id) && code == HttpStatusCode.Created) return (true, result.Resource);
        return (false, default(T));
    }

    public async Task<(bool, T?)> UpdateItem(string id, string partitionKey, T item)
    {
        item.Id = id;
        item.PartitionKey = partitionKey;
        var result = await _container.ReplaceItemAsync<T>(item, id, new PartitionKey(partitionKey));
        if (result == null) throw new ArgumentNullException(nameof(result), "Update Result is null");
        if (result.Resource == null) throw new ArgumentNullException(nameof(result.Resource), "Result Source is null");
        var code = result.StatusCode;
        var resource = result.Resource;
        if (!String.IsNullOrWhiteSpace(resource.Id) && (code == HttpStatusCode.OK)) return (true, resource);
        return (false, default(T));
    }

    public async Task<(bool, T?)> UpsertItem(T item)
    {
        var result = await _container.UpsertItemAsync<T>(item, new PartitionKey(item.PartitionKey));
        if (result == null) throw new ArgumentNullException(nameof(result), "Update Result is null");
        if (result.Resource == null) throw new ArgumentNullException(nameof(result.Resource), "Result Source is null");
        var code = result.StatusCode;
        var resource = result.Resource;
        Console.WriteLine(code);
        if (!String.IsNullOrWhiteSpace(resource.Id) && (code == HttpStatusCode.OK || code == HttpStatusCode.Created)) return (true, resource);
        return (false, default(T));
    }

    public async Task<bool> DeleteItem(string id, string partitionKey)
    {

        var result = await _container.DeleteItemAsync<T>(id, new PartitionKey(partitionKey));
        if (result == null) throw new ArgumentNullException(nameof(result), "Delete Result is null");
        var code = result.StatusCode;
        Console.WriteLine(code);
        if (code == HttpStatusCode.NoContent) return true;
        return false;
    }
}


