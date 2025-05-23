using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace rahayu_konveksi_api.Models;

public class Loan
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = null;

    [BsonElement("id_karyawan")]
    public string EmployeeId { get; set; } = null!;

    [BsonElement("nominal")]
    public int Amount { get; set; }

    [BsonElement("keterangan")]
    public string Description { get; set; } = null!;

    [BsonElement("transaksi")]
    public string Transaction { get; set; } = null!;

    [BsonElement("status")]
    public string? Status { get; set; } = null;

    [BsonElement("tanggal")]
    public DateTime Date { get; set; }

    [BsonElement("xendit_ref")]
    public string? XenditRef { get; set; } = null;
}