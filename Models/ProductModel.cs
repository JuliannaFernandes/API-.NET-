namespace Crud.Models;

public class ProductModel
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public ICollection<ItemModel> Items { get; set; } = new List<ItemModel>();
}