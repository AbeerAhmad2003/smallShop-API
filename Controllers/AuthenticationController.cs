using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using smallShop.Data;
using smallShop.Dtos;
using System.Text;

namespace smallShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(AppDbContext context, IConfiguration config) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(AppUserDto user)
        {
            if (user == null)
                return BadRequest();
            var getUser = await context.AppUsers.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (getUser != null) return BadRequest();
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
        [HttpPost("login/{email}/{password}")]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return BadRequest();
            }
            var user = await context.AppUsers.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return BadRequest();
            var verifyPassword = BCrypt.Net.BCrypt.Verify(password, user.Password);
            if (!verifyPassword) return NotFound("Invaild credentials");
            var getRole = await context.UserRoles.FirstOrDefaultAsync(r => r.AppUserId == user.Id);
            string key = $"{config["Authentication:Key"]}.{user.Name}.{user.Id}";
            string accessToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(key));
            return Ok($"Access Token:{accessToken}");


        }
        [HttpGet("protected-admin")]
        [Authorize(AuthenticationSchemes = "Netcode-Scheme", Roles ="Admin")]
        public string GetProtectedMessege() => "You are authorized [Admin]";

        [HttpGet("protected-user")]
        [Authorize(AuthenticationSchemes = "Netcode-Scheme", Roles = "User")]
        public string UserGetProtectedMessege() => "You are authorized[User]";

    }
}
