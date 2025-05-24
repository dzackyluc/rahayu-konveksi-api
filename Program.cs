using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using rahayu_konveksi_api.Models;
using rahayu_konveksi_api.Services;
using System.Text;
using Minio;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.Configure<RahayuKonveksiDatabaseSettings>(
    builder.Configuration.GetSection("RahayuKonveksiDatabase")
);

builder.Services.Configure<XenditConnectionSettings>(
    builder.Configuration.GetSection("XenditConnection")
);

builder.Services.AddSingleton<UsersService>();
builder.Services.AddSingleton<EmployeesService>();
builder.Services.AddSingleton<ProductsService>();
builder.Services.AddSingleton<EwalletService>();
builder.Services.AddSingleton<GeneralsService>();
builder.Services.AddSingleton<LoansService>();
builder.Services.AddSingleton<PayrollsService>();
builder.Services.AddSingleton<OrdersService>();

// Add JWT Authentication
var key = Encoding.ASCII.GetBytes("GenBadaiKelompok5xRahayuKonveksi2025"); // Replace with a secure key
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// Add CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add MinIO client configuration
builder.Services.AddSingleton<IMinioClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var endpoint = config["Minio:Endpoint"];
    var accessKey = config["Minio:AccessKey"];
    var secretKey = config["Minio:SecretKey"];
    return new MinioClient()
        .WithEndpoint(endpoint)
        .WithCredentials(accessKey, secretKey)
        .WithSSL()
        .Build();
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();