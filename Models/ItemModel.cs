namespace Crud.Models;

public class ItemModel
{
    public int Id { get; set; }
    public required int Quantity { get; set; }
    public required string UnitMeasure { get; set; }
    public int ProductId { get; set; }
    public required ProductModel Product { get; set; }

    public ICollection<CartModel> Carts { get; set; } = new List<CartModel>();
}