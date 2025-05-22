using rahayu_konveksi_api.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace rahayu_konveksi_api.Services
{
    public class GeneralsService
    {
        private readonly IMongoCollection<General> _generalsCollection;

        public GeneralsService(IOptions<RahayuKonveksiDatabaseSettings> rahayuKonveksiDatabaseSettings)
        {
            var mongoClient = new MongoClient(rahayuKonveksiDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(rahayuKonveksiDatabaseSettings.Value.DatabaseName);
            _generalsCollection = mongoDatabase.GetCollection<General>(rahayuKonveksiDatabaseSettings.Value.GeneralsCollectionName);
        }

        public async Task<List<General>> GetAllGeneralsAsync()
        {
            return await _generalsCollection.Find(_ => true).ToListAsync();
        }

        public async Task<General?> GetGeneralByIdAsync(string id)
        {
            return await _generalsCollection.Find(general => general.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateGeneralAsync(General general)
        {
            await _generalsCollection.InsertOneAsync(general);
        }

        public async Task UpdateGeneralAsync(string id, General generalIn)
        {
            await _generalsCollection.ReplaceOneAsync(general => general.Id == id, generalIn);
        }

        public async Task DeleteGeneralAsync(string id)
        {
            await _generalsCollection.DeleteOneAsync(general => general.Id == id);
        }

        public async Task<List<General>> GetGeneralsByXenditRefAsync(string xenditRef)
        {
            return await _generalsCollection.Find(general => general.XenditRef == xenditRef).ToListAsync();
        }

    }
}