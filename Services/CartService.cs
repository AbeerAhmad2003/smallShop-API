using smallShop.Dtos;
using smallShop.Models;
using System.Linq;
using System.Threading.Tasks;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    public CartService(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }
    public async Task<CartDto> GetCartByUserIdAsync(int userId)
    {
        var cart = await _cartRepository.GetCartByUserIdAsync(userId);
        if (cart == null) return null;

        return new CartDto
        {
            Id = cart.Id,
            AppUserId = cart.AppUserId,
            CartItems = cart.CartItems.Select(ci => new CartItemDto
            {
               Id = ci.Id,
                ProductId = ci.ProductId,
               ProductName = ci.Product.Name,
                ProductPrice = ci.Product.Price,
                Quantity = ci.Quantity,
               
            }).ToList()
        };
    }
    public async Task<bool> AddToCartAsync(int userId,int ProductId)
    {
        var cart=await _cartRepository.GetCartByUserIdAsync(userId);
        if (cart == null)
        {
            cart=new Cart{ AppUserId = userId };
            await _cartRepository.AddCartAsync(cart);
        }
        var cartItem = await _cartRepository.GetCartItemByProductIdAsync(cart.Id, ProductId);
       if(cartItem==null)
        {
            cartItem = new CartItem
            {
                CartId = cart.Id,
                ProductId = ProductId,
                Quantity = 1
            };
            await _cartRepository.AddCartItemAsync(cartItem);
        }
        else
        {
            cartItem.Quantity++;
            await _cartRepository.UpdateCartItemAsync(cartItem);
        }
        return true;

    }
    public async Task<bool> IncreaseQuantityAsync(int userId, int productId)
    {
        var cart = await _cartRepository.GetCartByUserIdAsync(userId);
        if (cart == null) return false;

        var cartItem = await _cartRepository.GetCartItemByProductIdAsync(cart.Id, productId);
        if (cartItem == null) return false;

        cartItem.Quantity++;
        await _cartRepository.UpdateCartItemAsync(cartItem);
        return true;
    }
    public async Task<bool> DecreaseQuantityAsync(int userId, int productId)
    {
        var cart = await _cartRepository.GetCartByUserIdAsync(userId);
        if (cart == null) return false;
        var cartItem = await _cartRepository.GetCartItemByProductIdAsync(cart.Id, productId);
        if (cartItem == null) return false;
        if (cartItem.Quantity > 1)
        {
            cartItem.Quantity--;
            await _cartRepository.UpdateCartItemAsync(cartItem);
        }
        else
        {
            await _cartRepository.RemoveCartItemAsync(cartItem);
        }
        return true;
    }
    public async Task<bool> RemoveItemAsync(int userId, int productId)
    {
        var cart = await _cartRepository.GetCartByUserIdAsync(userId);
        if (cart == null) return false;

        var cartItem = await _cartRepository.GetCartItemByProductIdAsync(cart.Id, productId);
        if (cartItem == null) return false;

        await _cartRepository.RemoveCartItemAsync(cartItem);
        return true;
    }
    public async Task<bool> ClearCartAsync(int userId)
    {
        var cart = await _cartRepository.GetCartByUserIdAsync(userId);
        if (cart == null) return false;

        await _cartRepository.RemoveCartItemsAsync(cart);
        return true;
    }

}
