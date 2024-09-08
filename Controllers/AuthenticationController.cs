using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using smallShop.Data;
using smallShop.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace smallShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly IConfiguration config;

        public AuthenticationController(AppDbContext context, IConfiguration config)
        {
            this.context = context;
            this.config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(AppUserDto user)
        {
            if (user == null)
                return BadRequest();
            var getUser = await context.AppUsers.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (getUser != null) return BadRequest("User already exists.");
            var entity = context.AppUsers.Add(new Models.AppUser()
            {
                Name = user.Name,
                Email = user.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password),
            }).Entity;
            await context.SaveChangesAsync();
            context.UserRoles.Add(new Models.UserRole()
            {
                AppUserId = entity.Id,
                Role = user.Role
            });
            await context.SaveChangesAsync();
            return Ok("Success");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return BadRequest("Email or password is empty.");
            }
            var user = await context.AppUsers.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return BadRequest("User not found.");
            var verifyPassword = BCrypt.Net.BCrypt.Verify(password, user.Password);
            if (!verifyPassword) return Unauthorized("Invalid credentials");

            var getRole = await context.UserRoles.FirstOrDefaultAsync(r => r.AppUserId == user.Id);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, getRole.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config["Authentication:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: config["Authentication:Issuer"],
                audience: config["Authentication:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            var tokenHandler = new JwtSecurityTokenHandler();
            var accessToken = tokenHandler.WriteToken(token);

            return Ok(new { AccessToken = accessToken });
        }

        [HttpGet("protected-admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetProtectedMessage() => Ok("You are authorized [Admin]");

        [HttpGet("protected-user")]
        [Authorize(Roles = "User")]
        public IActionResult UserGetProtectedMessage() => Ok("You are authorized [User]");
    }
}
