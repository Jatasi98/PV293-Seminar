using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Entities
{
    public class Cart : BaseEntity
    {
        public List<CartItem> Items { get; set; } = new();

        [Required]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = default!;
    }
}
