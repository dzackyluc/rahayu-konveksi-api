using System.Net.Http.Headers;
using rahayu_konveksi_api.Models;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace rahayu_konveksi_api.Services
{
    public class EwalletService
    {
        private readonly HttpClient client = new();

        public EwalletService(IOptions<XenditConnectionSettings> xenditConnectionSettings)
        {
            client.BaseAddress = new Uri(xenditConnectionSettings.Value.BaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var username = xenditConnectionSettings.Value.ApiKey;
            var password = "";
            var credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{username}:{password}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        }
        public async Task<int?> GetEwalletBalanceAsync()
        {
            HttpResponseMessage response = await client.GetAsync("balance");
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();

                // Parse the JSON response
                var jsonDocument = JsonDocument.Parse(jsonResponse);
                var balance = jsonDocument.RootElement.GetProperty("balance").GetInt32(); // Replace "balance" with the actual key

                return balance;
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                return null;
            }
        }

        public async Task<string?> GetEwalletTransactionsAsync()
        {
            HttpResponseMessage response = await client.GetAsync("transactions?limit=30");
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return jsonResponse;
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                return null;
            }
        }
        
        public async Task<JsonElement?> CreatePayoutAsync(string external_id, int amount, string email)
        {
            var payload = new
            {
                external_id = external_id,
                amount = amount,
                email = email
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("payouts", content);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var jsonDocument = JsonDocument.Parse(jsonResponse);
                return jsonDocument.RootElement.Clone();
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                return null;
            }
        }
    }
}