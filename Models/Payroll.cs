using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace rahayu_konveksi_api.Models;

public class Payroll
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = null;

    [BsonElement("id_karyawan")]
    public string EmployeeId { get; set; } = null!;

    [BsonElement("transaksi")]
    public string Transaction { get; set; } = null!;

    [BsonElement("total_gaji")]
    public int TotalSalary { get; set; }

    [BsonElement("potongan")]
    public int Deduction { get; set; }

    [BsonElement("gaji_bayar")]
    public int SalaryPaid { get; set; }

    [BsonElement("tanggal")]
    public DateTime Date { get; set; }

    [BsonElement("status")]
    public string? Status { get; set; } = null;

    [BsonElement("xendit_ref")]
    public string? XenditRef { get; set; } = null;
}