using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

public class FileMeta {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } // GridFS id as string
    public string Name { get; set; }
    public string FolderId { get; set; }
    public string OwnerId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public long Size { get; set; }
    // other fields: mime, version, permissions, etc.
}
