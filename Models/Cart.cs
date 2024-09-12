namespace smallShop.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int AppUserId {  get; set; }
        public AppUser AppUser { get; set; }
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    }
}
