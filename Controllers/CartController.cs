using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using smallShop.Data;
using smallShop.Dtos;
using smallShop.Models;
using System.Linq;
using System.Threading.Tasks;

namespace smallShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        // Helper method to get the current user's ID from claims
        private int GetUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException("User ID not found");
        }

        // Add 1 item to cart for the given productId
        [HttpPost("add/{productId}")]
        public async Task<IActionResult> AddToCart(int productId)
        {
            var userId = GetUserId();
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.AppUserId == userId);

            if (cart == null)
            {
                cart = new Cart { AppUserId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync(); // Save to get the CartId
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = 1 // Always add 1 item by default
                };
                _context.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity++; // Increment the quantity if the item is already in the cart
                _context.CartItems.Update(cartItem);
            }

            await _context.SaveChangesAsync();
            return Ok("Item added to cart");
        }

        // Get the cart for the current user
        [HttpGet("cart")]
        public async Task<ActionResult<CartDto>> GetCart()
        {
            var userId = GetUserId();
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.AppUserId == userId);

            if (cart == null)
            {
                return NotFound();
            }

            var cartDto = new CartDto
            {
                Id = cart.Id,
                AppUserId = cart.AppUserId,
                CartItems = cart.CartItems.Select(ci => new CartItemDto
                {
                    Id = ci.Id,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.Name,
                    Quantity = ci.Quantity,
                    ProductPrice = ci.Product.Price
                }).ToList()
            };

            return Ok(cartDto);
        }

        // Remove an item from the cart by productId
        [HttpDelete("remove/{productId}")]
        public async Task<IActionResult> RemoveItem(int productId)
        {
            var userId = GetUserId();
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.AppUserId == userId);

            if (cart == null)
            {
                return NotFound("Cart not found");
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem == null)
            {
                return NotFound("Item not found in cart");
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return Ok("Item removed from cart");
        }

        // Increase the quantity of a cart item by productId
        [HttpPut("increase/{productId}")]
        public async Task<IActionResult> IncreaseQuantity(int productId)
        {
            var userId = GetUserId();
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.AppUserId == userId);

            if (cart == null)
            {
                return NotFound("Cart not found");
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem == null)
            {
                return NotFound("Item not found in cart");
            }

            cartItem.Quantity++;
            _context.CartItems.Update(cartItem);
            await _context.SaveChangesAsync();

            return Ok("Item quantity increased");
        }

        // Decrease the quantity of a cart item by productId
        [HttpPut("decrease/{productId}")]
        public async Task<IActionResult> DecreaseQuantity(int productId)
        {
            var userId = GetUserId();
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.AppUserId == userId);

            if (cart == null)
            {
                return NotFound("Cart not found");
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem == null)
            {
                return NotFound("Item not found in cart");
            }

            if (cartItem.Quantity > 1)
            {
                cartItem.Quantity--;
                _context.CartItems.Update(cartItem);
            }
            else
            {
                _context.CartItems.Remove(cartItem);
            }

            await _context.SaveChangesAsync();

            return Ok("Item quantity decreased or removed");
        }

        // Clear the cart (remove all items) for the current user
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.AppUserId == userId);

            if (cart == null)
            {
                return NotFound("Cart not found");
            }

            _context.CartItems.RemoveRange(cart.CartItems);
            await _context.SaveChangesAsync();

            return Ok("Cart cleared");
        }
    }
}
