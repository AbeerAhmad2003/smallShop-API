using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using smallShop.Dtos;
using System.Threading.Tasks;

namespace smallShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")]
  public class CartController : ControllerBase
  {
      private readonly ICartService _cartService;

      public CartController(ICartService cartService)
      {
        _cartService = cartService;
      }

      private int GetUserId()
      {
         var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
         if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
         {
            return userId;
         }
        throw new UnauthorizedAccessException("User ID not found");
      }

      [HttpPost("add/{productId}")]
      public async Task<IActionResult> AddToCart(int productId)
      {
         var userId = GetUserId();
         var result = await _cartService.AddToCartAsync(userId, productId);
         return result ? Ok("Item added to cart") : BadRequest("Failed to add item to cart");
      }

      [HttpGet("cart")]
      public async Task<ActionResult<CartDto>> GetCart()
      {
        var userId = GetUserId();
        var cart = await _cartService.GetCartByUserIdAsync(userId);
        return cart != null ? Ok(cart) : NotFound("Cart not found");
      }

     [HttpDelete("remove/{productId}")]
     public async Task<IActionResult> RemoveItem(int productId)
     {
       var userId = GetUserId();
       var result = await _cartService.RemoveItemAsync(userId, productId);
       return result ? Ok("Item removed from cart") : NotFound("Item not found in cart");
     }

     [HttpPut("increase/{productId}")]
     public async Task<IActionResult> IncreaseQuantity(int productId)
     {
        var userId = GetUserId();
        var result = await _cartService.IncreaseQuantityAsync(userId, productId);
        return result ? Ok("Item quantity increased") : NotFound("Item not found in cart");
     }

     [HttpPut("decrease/{productId}")]
     public async Task<IActionResult> DecreaseQuantity(int productId)
     {
         var userId = GetUserId();
         var result = await _cartService.DecreaseQuantityAsync(userId, productId);
         return result ? Ok("Item quantity decreased or removed") : NotFound("Item not found in cart");
     }



    }
}