﻿namespace smallShop.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public Cart Cart { get; set; }

    }
}
