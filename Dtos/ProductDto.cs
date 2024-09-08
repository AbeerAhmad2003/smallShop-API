using smallShop.Models;
using System.ComponentModel.DataAnnotations;

namespace smallShop.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public string UrlImg { get; set; }
        public int CategoryId { get; set; }
        
    }
}
