namespace smallShop.Dtos
{
    public class CartDto
    {
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public ICollection<CartItemDto> CartItems { get; set; } = new List<CartItemDto>();
    }
}
