namespace Demo.Models
{
    public interface ICosmosResource
    {
        public string Id { get; set; }
        public string PartitionKey { get; }
    }
}
