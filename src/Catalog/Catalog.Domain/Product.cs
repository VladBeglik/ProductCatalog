namespace Catalog.Domain;

public class Product : BaseEntity
{
    public string Name { get; set; } = null!;
    public Category? CategoryId { get; set; }
    public Category? Category { get; set; }
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public string NoteGeneral { get; set; } = null!;
    public string NoteSpecial { get; set; } = null!;
}