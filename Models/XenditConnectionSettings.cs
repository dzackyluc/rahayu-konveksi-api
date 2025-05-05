namespace rahayu_konveksi_api.Models
{
    public class XenditConnectionSettings
    {
        public string ApiKey { get; set; } = null!;
        public string BaseUrl { get; set; } = null!;
        public string WebhookSecret { get; set; } = null!;
    }
}