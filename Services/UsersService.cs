using rahayu_konveksi_api.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace rahayu_konveksi_api.Services
{
    public class UsersService
    {
        private readonly IMongoCollection<User> _usersCollection;

        public UsersService(IOptions<RahayuKonveksiDatabaseSettings> RahayuKonveksiDatabaseSettings)
        {
            var mongoClient = new MongoClient(RahayuKonveksiDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(RahayuKonveksiDatabaseSettings.Value.DatabaseName);
            _usersCollection = mongoDatabase.GetCollection<User>(RahayuKonveksiDatabaseSettings.Value.UsersCollectionName);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _usersCollection.Find(_ => true).ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await _usersCollection.Find(user => user.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateUserAsync(User user)
        {
            await _usersCollection.InsertOneAsync(user);
        }

        public async Task UpdateUserAsync(string id, User userIn)
        {
            await _usersCollection.ReplaceOneAsync(user => user.Id == id, userIn);
        }

        public async Task DeleteUserAsync(string id)
        {
            await _usersCollection.DeleteOneAsync(user => user.Id == id);
        }
    }
}