using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace rahayu_konveksi_api.Models;

public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = null;

    [BsonElement("nama_produk")]
    public string Name { get; set; } = null!;

    [BsonElement("harga")]
    public int Price { get; set; }

    [BsonElement("kategori")]
    public string Category { get; set; } = null!;

    [BsonElement("gambar")]
    public string Photo { get; set; } = null!;
}