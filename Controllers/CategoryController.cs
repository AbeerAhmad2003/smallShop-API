using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using smallShop.Data;
using smallShop.Dtos;
using smallShop.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _appDbContext;

    public CategoriesController(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
    {
        var categories = await _appDbContext.Categories
            .Select(c => new CategoryDto
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
                    Price = p.Price,
                    UrlImg = p.UrlImg,
                    CategoryId = p.CategoryId
                }).ToList()
            }).ToListAsync();

        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetCategory(int id)
    {
        var category = await _appDbContext.Categories
            .Where(c => c.Id == id)
            .Select(c => new CategoryDto
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
                    Price = p.Price,
                    UrlImg = p.UrlImg,
                    CategoryId = p.CategoryId
                }).ToList()
            }).FirstOrDefaultAsync();

        if (category == null)
        {
            return NotFound();
        }

        return Ok(category);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryDto>> PostCategory([FromBody] CategoryDto categoryDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

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

        _appDbContext.Categories.Add(category);
        await _appDbContext.SaveChangesAsync();

        categoryDto.Id = category.Id;
        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, categoryDto);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PutCategory(int id, [FromBody] CategoryDto categoryDto)
    {
        if (id != categoryDto.Id)
        {
            return BadRequest();
        }

        var category = await _appDbContext.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        category.Name = categoryDto.Name;
        category.Description = categoryDto.Description;
        category.UrlImg = categoryDto.UrlImg;

        // Remove existing products and add updated ones
        _appDbContext.Products.RemoveRange(category.Products);
        category.Products = categoryDto.Products.Select(dto => new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            UrlImg = dto.UrlImg,
            CategoryId = categoryDto.Id
        }).ToList();

        _appDbContext.Entry(category).State = EntityState.Modified;

        try
        {
            await _appDbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CategoryExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }
    [HttpGet("Products/{categoryId}")]
    [Authorize(Roles = "User")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategory(int categoryId)
    {
        var products = await _appDbContext.Products
            .Where(p => p.CategoryId == categoryId)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                UrlImg = p.UrlImg,
                CategoryId = p.CategoryId
            }).ToListAsync();

        if (!products.Any())
        {
            return NotFound();
        }

        return Ok(products);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _appDbContext.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        _appDbContext.Categories.Remove(category);
        await _appDbContext.SaveChangesAsync();
        return NoContent();
    }

    private bool CategoryExists(int id)
    {
        return _appDbContext.Categories.Any(e => e.Id == id);
    }
}
