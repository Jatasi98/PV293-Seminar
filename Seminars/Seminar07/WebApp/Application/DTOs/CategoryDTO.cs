namespace Application.DTOs;
public class CategoryDTO
{
    public int Id { get; set; }

    //[Required, StringLength(200)]
    public string Name { get; set; } = default!;

    //[StringLength(1000)]
    public string? Description { get; set; }
}
