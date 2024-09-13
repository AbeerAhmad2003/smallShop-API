using smallShop.Dtos;
using smallShop.Models;
using smallShop.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace smallShop.Services
{
    public class CategoryService:ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync()
        {
            var categories = await _categoryRepository.GetCategoriesAsync();
            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                UrlImg = c.UrlImg,
                Products = c.Products.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    UrlImg = p.UrlImg,
                    Price = p.Price,
                    CategoryId = p.CategoryId,

                }).ToList()
            });
        }
        public async Task<CategoryDto> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return null;
            }
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                UrlImg = category.UrlImg,
                Products = category.Products.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    UrlImg = p.UrlImg,
                    Price = p.Price,
                    CategoryId = p.CategoryId,
                }).ToList()
            };
        }
        public async Task<CategoryDto> AddCategoryAsync(CategoryDto categoryDto)
        {
            var category = new Category
            {
                Name = categoryDto.Name,
                Description = categoryDto.Description,
                UrlImg = categoryDto.UrlImg,
                Products = categoryDto.Products.Select(dto => new Product
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    Price = dto.Price,
                    UrlImg = dto.UrlImg,
                    CategoryId = categoryDto.Id
                }).ToList()
            };

            await _categoryRepository.AddCategoryAsync(category);
            categoryDto.Id = category.Id;
            return categoryDto;
        }
        public async Task<bool> UpdateCategoryAsync(int id, CategoryDto categoryDto)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return false;
            }

            category.Name = categoryDto.Name;
            category.Description = categoryDto.Description;
            category.UrlImg = categoryDto.UrlImg;

            
            await _categoryRepository.DeleteCategoryAsync(category); 
            category.Products = categoryDto.Products.Select(dto => new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                UrlImg = dto.UrlImg,
                CategoryId = categoryDto.Id
            }).ToList();

            await _categoryRepository.UpdateCategoryAsync(category);
            return true;
        }
        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(categoryId);
            if (category == null)
            {
                return Enumerable.Empty<ProductDto>();
            }

            return category.Products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                UrlImg = p.UrlImg,
                CategoryId = p.CategoryId
            });
        }
        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null){
                return false;
            }
            await _categoryRepository.DeleteCategoryAsync(category);
            return true;
        }

    }
}
