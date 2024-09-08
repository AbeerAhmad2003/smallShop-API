using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using smallShop.Data;
using smallShop.Dtos;
using smallShop.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace smallShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;

        public ProductController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products = await _appDbContext.Products
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    UrlImg = p.UrlImg,
                    CategoryId = p.CategoryId
                }).ToListAsync();

            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _appDbContext.Products
                .Where(p => p.Id == id)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    UrlImg = p.UrlImg,
                    CategoryId = p.CategoryId
                }).FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }
       
        

        [HttpPost]
        public async Task<ActionResult<ProductDto>> PostProduct([FromBody] ProductDto productDto)
        {
            if (productDto.CategoryId <= 0)
            {
                return BadRequest("CategoryId is required.");
            }

            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                UrlImg = productDto.UrlImg,
                CategoryId = productDto.CategoryId
            };

            _appDbContext.Products.Add(product);
            await _appDbContext.SaveChangesAsync();

            productDto.Id = product.Id;
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, productDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, [FromBody] ProductDto productDto)
        {
            if (id != productDto.Id)
            {
                return BadRequest();
            }

            var product = await _appDbContext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            product.Name = productDto.Name;
            product.Description = productDto.Description;
            product.Price = productDto.Price;
            product.UrlImg = productDto.UrlImg;
            product.CategoryId = productDto.CategoryId;

            _appDbContext.Entry(product).State = EntityState.Modified;

            try
            {
                await _appDbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _appDbContext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _appDbContext.Products.Remove(product);
            await _appDbContext.SaveChangesAsync();
            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _appDbContext.Products.Any(e => e.Id == id);
        }
    }
}
