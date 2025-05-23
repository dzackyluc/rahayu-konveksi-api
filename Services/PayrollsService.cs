using rahayu_konveksi_api.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace rahayu_konveksi_api.Services
{
    public class PayrollsService
    {
        private readonly IMongoCollection<Payroll> _payrollsCollection;

        public PayrollsService(IOptions<RahayuKonveksiDatabaseSettings> rahayuKonveksiDatabaseSettings)
        {
            var mongoClient = new MongoClient(rahayuKonveksiDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(rahayuKonveksiDatabaseSettings.Value.DatabaseName);
            _payrollsCollection = mongoDatabase.GetCollection<Payroll>(rahayuKonveksiDatabaseSettings.Value.PayrollsCollectionName);
        }

        public async Task<List<Payroll>> GetAllPayrollsAsync()
        {
            return await _payrollsCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Payroll?> GetPayrollByIdAsync(string id)
        {
            return await _payrollsCollection.Find(payroll => payroll.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreatePayrollAsync(Payroll payroll)
        {
            await _payrollsCollection.InsertOneAsync(payroll);
        }

        public async Task UpdatePayrollAsync(string id, Payroll payrollIn)
        {
            await _payrollsCollection.ReplaceOneAsync(payroll => payroll.Id == id, payrollIn);
        }

        public async Task DeletePayrollAsync(string id)
        {
            await _payrollsCollection.DeleteOneAsync(payroll => payroll.Id == id);
        }

        public async Task<List<Payroll>> GetPayrollsByXenditRefAsync(string xenditRef)
        {
            return await _payrollsCollection.Find(payroll => payroll.XenditRef == xenditRef).ToListAsync();
        }
    }
}