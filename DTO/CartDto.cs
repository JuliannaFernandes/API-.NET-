namespace Crud.DTO;

public class CartDto
{
    public int Id { get; set; }          
    public required List<ItemDto> Items { get; set; }
    
    public int ItemId { get; set; }
}