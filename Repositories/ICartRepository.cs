using smallShop.Models;
using System.Threading.Tasks;

public interface ICartRepository
{
    Task<Cart> GetCartByUserIdAsync(int userId);
    Task AddCartAsync(Cart cart);
    Task UpdateCartAsync(Cart cart);
    Task AddCartItemAsync(CartItem cartItem);
    Task UpdateCartItemAsync(CartItem cartItem);
    Task RemoveCartItemAsync(CartItem cartItem);
    Task RemoveCartItemsAsync(Cart cart);
    Task<CartItem> GetCartItemByProductIdAsync(int cartId, int productId);
}
