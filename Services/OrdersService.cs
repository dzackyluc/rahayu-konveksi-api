using rahayu_konveksi_api.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace rahayu_konveksi_api.Services
{
    public class OrdersService
    {
        private readonly IMongoCollection<Order> _ordersCollection;

        public OrdersService(IOptions<RahayuKonveksiDatabaseSettings> rahayuKonveksiDatabaseSettings)
        {
            var mongoClient = new MongoClient(rahayuKonveksiDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(rahayuKonveksiDatabaseSettings.Value.DatabaseName);
            _ordersCollection = mongoDatabase.GetCollection<Order>(rahayuKonveksiDatabaseSettings.Value.OrdersCollectionName);
        }
        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _ordersCollection.Find(_ => true).ToListAsync();
        }
        public async Task<Order?> GetOrderByIdAsync(string id)
        {
            return await _ordersCollection.Find(order => order.Id == id).FirstOrDefaultAsync();
        }
        public async Task CreateOrderAsync(Order order)
        {
            await _ordersCollection.InsertOneAsync(order);
        }
        public async Task UpdateOrderAsync(string id, Order orderIn)
        {
            await _ordersCollection.ReplaceOneAsync(order => order.Id == id, orderIn);
        }
        public async Task DeleteOrderAsync(string id)
        {
            await _ordersCollection.DeleteOneAsync(order => order.Id == id);
        }
        public async Task<List<Order>> GetOrdersByXenditRefAsync(string xenditRef)
        {
            return await _ordersCollection.Find(order => order.XenditRef == xenditRef).ToListAsync();
        }
    }
}