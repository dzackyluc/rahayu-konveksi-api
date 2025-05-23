namespace rahayu_konveksi_api.Models;

public class RahayuKonveksiDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string UsersCollectionName { get; set; } = null!;
    public string EmployeesCollectionName { get; set; } = null!;
    public string ProductsCollectionName { get; set; } = null!;
    public string OrdersCollectionName { get; set; } = null!;
    public string TransactionsCollectionName { get; set; } = null!;
    public string GeneralsCollectionName { get; set; } = null!;
    public string LoansCollectionName { get; set; } = null!;
    public string PayrollsCollectionName { get; set; } = null!;
}