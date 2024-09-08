using System.ComponentModel.DataAnnotations;

namespace smallShop.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public string Description { get; set; } 
        public int Price { get; set; }
        public string UrlImg { get; set; } 
        [Required]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
