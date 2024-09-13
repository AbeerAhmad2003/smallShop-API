using Microsoft.EntityFrameworkCore;
using smallShop.Data;
using smallShop.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace smallShop.Repositories
{
    public class CategoryRepository:ICategoryRepository
    {
        private readonly AppDbContext _appDbContext;
        public CategoryRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _appDbContext.Categories.Include(c=>c.Products).ToListAsync();
        }
        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _appDbContext.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task AddCategoryAsync(Category category)
        {
            _appDbContext.Categories.Add(category);
            await _appDbContext.SaveChangesAsync();
        }
        public async Task UpdateCategoryAsync(Category category)
        {
            _appDbContext.Entry(category).State = EntityState.Modified;
            await _appDbContext.SaveChangesAsync();
        }
        public async Task<bool> CategoryExistsAsync(int id)
        {
            return await _appDbContext.Categories.AnyAsync(c => c.Id == id);
        }

        public Task DeleteCategoryAsync(Category category)
        {
            throw new NotImplementedException();
        }
    }
}
