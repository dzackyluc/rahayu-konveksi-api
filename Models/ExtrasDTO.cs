using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace rahayu_konveksi_api.Models;

public class ExtrasDTO
{

    [BsonElement("name")]
    public string name { get; set; } = null!;

    [BsonElement("quantity")]
    public int quantity { get; set; }

    [BsonElement("price")]
    public string price { get; set; } = null!;
}