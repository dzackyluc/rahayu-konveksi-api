using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace rahayu_konveksi_api.Models;

public class Employee {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = null;

    [BsonElement("nama")]
    public string Name { get; set; } = null!;

    [BsonElement("status")]
    public string Status { get; set; } = null!;

    [BsonElement("posisi")]
    public string Position { get; set; } = null!;

    [BsonElement("foto")]
    public string Photo { get; set; } = null!;
}