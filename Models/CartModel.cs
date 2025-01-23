namespace Crud.Models;

public class CartModel
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public ItemModel Item { get; set; }
    
    public decimal QuantityCart { get; set; }
}