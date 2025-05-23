using rahayu_konveksi_api.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace rahayu_konveksi_api.Services
{
    public class LoansService
    {
        private readonly IMongoCollection<Loan> _loansCollection;

        public LoansService(IOptions<RahayuKonveksiDatabaseSettings> rahayuKonveksiDatabaseSettings)
        {
            var mongoClient = new MongoClient(rahayuKonveksiDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(rahayuKonveksiDatabaseSettings.Value.DatabaseName);
            _loansCollection = mongoDatabase.GetCollection<Loan>(rahayuKonveksiDatabaseSettings.Value.LoansCollectionName);
        }
        public async Task<List<Loan>> GetAllLoansAsync()
        {
            return await _loansCollection.Find(_ => true).ToListAsync();
        }
        public async Task<Loan?> GetLoanByIdAsync(string id)
        {
            return await _loansCollection.Find(loan => loan.Id == id).FirstOrDefaultAsync();
        }
        public async Task CreateLoanAsync(Loan loan)
        {
            await _loansCollection.InsertOneAsync(loan);
        }
        public async Task UpdateLoanAsync(string id, Loan loanIn)
        {
            await _loansCollection.ReplaceOneAsync(loan => loan.Id == id, loanIn);
        }
        public async Task DeleteLoanAsync(string id)
        {
            await _loansCollection.DeleteOneAsync(loan => loan.Id == id);
        }
        public async Task<List<Loan>> GetLoansByXenditRefAsync(string xenditRef)
        {
            return await _loansCollection.Find(loan => loan.XenditRef == xenditRef).ToListAsync();
        }
    }
}