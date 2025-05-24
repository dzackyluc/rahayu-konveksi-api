using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace rahayu_konveksi_api.Models;

public class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = null;

    [BsonElement("Nama Pemesan")]
    public string CustomerName { get; set; } = null!;

    [BsonElement("Nomor Telepon")]
    public string PhoneNumber { get; set; } = null!;

    [BsonElement("Kota")]
    public string City { get; set; } = null!;

    [BsonElement("Alamat")]
    public string Address { get; set; } = null!;

    [BsonElement("Longitude")]
    public double Longitude { get; set; }

    [BsonElement("Latitude")]
    public double Latitude { get; set; }

    [BsonElement("Jenis Produk")]
    public string ProductType { get; set; } = null!;

    [BsonElement("Jumlah Produk")]
    public int ProductQuantity { get; set; }

    [BsonElement("deadline")]
    public DateTime Deadline { get; set; }

    [BsonElement("status")]
    public string? Status { get; set; } = null;

    [BsonElement("extras")]
    public List<ExtrasDTO> Extras { get; set; } = null!;

    [BsonElement("Extra Price")]
    public int ExtraPrice { get; set; }

    [BsonElement("ukuran_s")]
    public int SizeS { get; set; }

    [BsonElement("ukuran_m")]
    public int SizeM { get; set; }

    [BsonElement("ukuran_l")]
    public int SizeL { get; set; }

    [BsonElement("ukuran_xl")]
    public int SizeXL { get; set; }

    [BsonElement("ukuran_xxl")]
    public int SizeXXL { get; set; }

    [BsonElement("ukuran_lainnya")]
    public string? OtherSize { get; set; } = null;

    [BsonElement("Total Harga")]
    public int TotalPrice { get; set; }

    [BsonElement("xendit_ref")]
    public string? XenditRef { get; set; } = null;

    [BsonElement("Payment Url")]
    public string? PaymentUrl { get; set; } = null;
}