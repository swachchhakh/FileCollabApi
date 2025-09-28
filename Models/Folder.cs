using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Folder {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string Name { get; set; }
    public string ParentId { get; set; } // null for root
    public string OwnerId { get; set; }
}
