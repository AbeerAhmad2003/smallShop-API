using smallShop.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace smallShop.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetCategoriesAsync();
        Task<CategoryDto> GetCategoryByIdAsync(int id);
        Task<CategoryDto> AddCategoryAsync(CategoryDto categoryDto);
        Task<bool> UpdateCategoryAsync(int id, CategoryDto categoryDto);
        Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId);
        Task<bool> DeleteCategoryAsync(int id);
    }
}