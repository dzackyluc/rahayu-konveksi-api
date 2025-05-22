using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace rahayu_konveksi_api.Models;

public class General
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = null;

    [BsonElement("nama")]
    public string Name { get; set; } = null!;

    [BsonElement("email")]
    public string Email { get; set; } = null!;

    [BsonElement("jumlah_pengeluaran")]
    public int TotalExpense { get; set; }

    [BsonElement("keterangan")]
    public string Description { get; set; } = null!;

    [BsonElement("status")]
    public string? Status { get; set; } = null;

    [BsonElement("tanggal")]
    public DateTime Date { get; set; }

    [BsonElement("xendit_ref")]
    public string? XenditRef { get; set; } = null;
}