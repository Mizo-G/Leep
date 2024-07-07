using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Demo.models
{
    public class User
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId(); 

        [BsonElement("name")]
        public string Name { get; set; } = "";
        
        [BsonElement("interests")]
        public List<string> Interests { get; set; } = new List<string>(); 
        
        [BsonElement("userType")]
        public UserType UserType { get; set; }
        [BsonElement("workingAt")]
        public string? WorkingAt { get; set; }
        [BsonElement("jobTitle")]
        public string? JobTitle { get; set; }
        
        [BsonElement("sector")]
        public required Sector Sector { get; set; }
        
        [BsonElement("subSector")]
        public required SubSector SubSector { get; set; }
    }


    public class Sector
    {
        [BsonElement("name")]
        public required string Name { get; set; }
    }

    public class SubSector
    {
        [BsonElement("parentSector")]
        public required Sector ParentSector { get; set; }
        [BsonElement("name")]
        public required string Name { get; set; }
    }
    public enum UserType
    {
        Professional,
        Social_Entrepreneur
    }
}