using System.ComponentModel.DataAnnotations;

namespace DAL.Entities.Base;

public class BaseEntity
{
    [Key]
    public int Id { get; set; }

    public DateTime CreatedOnUTC { get; set; } = DateTime.UtcNow;
}
