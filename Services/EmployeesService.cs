using rahayu_konveksi_api.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace rahayu_konveksi_api.Services
{
    public class EmployeesService
    {
        private readonly IMongoCollection<Employee> _employeesCollection;

        public EmployeesService(IOptions<RahayuKonveksiDatabaseSettings> RahayuKonveksiDatabaseSettings)
        {
            var mongoClient = new MongoClient(RahayuKonveksiDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(RahayuKonveksiDatabaseSettings.Value.DatabaseName);
            _employeesCollection = mongoDatabase.GetCollection<Employee>(RahayuKonveksiDatabaseSettings.Value.EmployeesCollectionName);
        }

        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            return await _employeesCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Employee?> GetEmployeeByIdAsync(string id)
        {
            return await _employeesCollection.Find(employee => employee.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateEmployeeAsync(Employee employee)
        {
            await _employeesCollection.InsertOneAsync(employee);
        }

        public async Task UpdateEmployeeAsync(string id, Employee employeeIn)
        {
            await _employeesCollection.ReplaceOneAsync(employee => employee.Id == id, employeeIn);
        }

        public async Task DeleteEmployeeAsync(string id)
        {
            await _employeesCollection.DeleteOneAsync(employee => employee.Id == id);
        }

        public async Task<List<Employee>> GetEmployeesByStatusAsync(string status)
        {
            return await _employeesCollection.Find(employee => employee.Status == status).ToListAsync();
        }

        public async Task<List<Employee>> GetEmployeesByPositionAsync(string position)
        {
            return await _employeesCollection.Find(employee => employee.Position == position).ToListAsync();
        }
    }
}
