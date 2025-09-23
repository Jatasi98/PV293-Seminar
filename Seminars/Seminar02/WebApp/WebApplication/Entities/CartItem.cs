namespace WebApplication1.Entities
{
    public class CartItem : BaseEntity
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = "";
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }

        public decimal LineTotal => UnitPrice * Quantity;

        public int CartId { get; set; }
        public Cart Cart { get; set; } = default!;
    }
}
