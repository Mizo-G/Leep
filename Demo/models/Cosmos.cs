using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

public class CosmosDB
{
    private readonly Container _container;

    public CosmosDB(CosmosContainerFactory containerFactory, string containerName)
    {
        _container = containerFactory.GetContainer(containerName) ?? throw new ArgumentNullException(nameof(containerName), "Container is null");
    }

    public async Task<List<T>> QueryItems<T>(QueryDefinition query, QueryRequestOptions? opts = null)
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

    public async Task<bool> CreateItem<T>(T item)
    {
        var result = await _container.CreateItemAsync(item);
        if (result == null) throw new ArgumentNullException(nameof(result), "Result is null");
        return true;
    }

    public async Task<bool> UpdateItem<T>(T item)
    {
        var result = await _container.UpsertItemAsync(item);
        if (result == null) throw new ArgumentNullException(nameof(result), "Result is null");
        return true;
    }

    public async Task<bool> DeleteItem<T>(T item) where T : ICosmosResource
    {
        var result = await _container.DeleteItemAsync<T>(item.Id, new PartitionKey(item.Id));
        if (result == null) throw new ArgumentNullException(nameof(result), "Result is null");
        return true;
    }
}

public interface ICosmosResource
{
    [JsonProperty("id")]
    string Id { get; set; }
}
