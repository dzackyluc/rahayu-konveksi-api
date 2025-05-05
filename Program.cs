using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using rahayu_konveksi_api.Models;
using rahayu_konveksi_api.Services;
using System.Text;

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

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();