using rahayu_konveksi_api.Models;
using rahayu_konveksi_api.Services;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;


namespace rahayu_konveksi_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(UsersService usersService) : ControllerBase
    {
        private readonly UsersService _usersService = usersService;

        // POST: api/auth/login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            var users = await _usersService.GetAllUsersAsync();
            var existingUser = users.FirstOrDefault(u => u.Username == user.Username);
            if (existingUser == null)
            {
                return NotFound(new { message = "User not found" });
            }

            if (existingUser.Password != user.Password)
            {
                return Unauthorized(new { message = "Invalid password" });
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("GenBadaiKelompok5xRahayuKonveksi2025");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.Name, existingUser.Username),
                    new Claim(ClaimTypes.Role, existingUser.Role!)
                ]),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new 
            { 
                message = "Login successful",
                token = tokenString});
        }

        // POST: api/auth/getAllUsers
        [HttpGet("getAllUsers")]
        // [Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _usersService.GetAllUsersAsync();
            return Ok(users);
        }
    }
}