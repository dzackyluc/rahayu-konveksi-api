using rahayu_konveksi_api.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace rahayu_konveksi_api.Services
{
    public class ProductsService
    {
        private readonly IMongoCollection<Product> _productsCollection;

        public ProductsService(IOptions<RahayuKonveksiDatabaseSettings> RahayuKonveksiDatabaseSettings)
        {
            var mongoClient = new MongoClient(RahayuKonveksiDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(RahayuKonveksiDatabaseSettings.Value.DatabaseName);
            _productsCollection = mongoDatabase.GetCollection<Product>(RahayuKonveksiDatabaseSettings.Value.ProductsCollectionName);
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _productsCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(string id)
        {
            return await _productsCollection.Find(product => product.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateProductAsync(Product product)
        {
            await _productsCollection.InsertOneAsync(product);
        }

        public async Task UpdateProductAsync(string id, Product productIn)
        {
            await _productsCollection.ReplaceOneAsync(product => product.Id == id, productIn);
        }

        public async Task DeleteProductAsync(string id)
        {
            await _productsCollection.DeleteOneAsync(product => product.Id == id);
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(string category)
        {
            return await _productsCollection.Find(product => product.Category == category).ToListAsync();
        }
    }
}