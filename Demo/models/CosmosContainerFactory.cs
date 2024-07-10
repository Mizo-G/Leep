using Microsoft.Azure.Cosmos;

public class CosmosContainerFactory
{
    private readonly CosmosClient _cosmosClient;

    public CosmosContainerFactory(CosmosClient cosmosClient)
    {
        _cosmosClient = cosmosClient;
    }

    public Container GetContainer(string containerName)
    {
        if (string.IsNullOrEmpty(containerName))
        {
            throw new ArgumentNullException(nameof(containerName));
        }

        return _cosmosClient.GetContainer("Shared", containerName);
    }
}
