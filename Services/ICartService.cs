using smallShop.Dtos;
using System.Threading.Tasks;

public interface ICartService
{
    Task<CartDto> GetCartByUserIdAsync(int userId);
    Task<bool> AddToCartAsync(int userId, int productId);
    Task<bool> IncreaseQuantityAsync(int userId, int productId);
    Task<bool> DecreaseQuantityAsync(int userId, int productId);
    Task<bool> RemoveItemAsync(int userId, int productId);
    Task<bool> ClearCartAsync(int userId);
}
