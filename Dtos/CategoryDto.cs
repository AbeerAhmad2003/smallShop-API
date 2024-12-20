﻿using smallShop.Models;

namespace smallShop.Dtos
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UrlImg { get; set; }

        public ICollection<ProductDto> Products { get; set; }
    }
}
