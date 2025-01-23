namespace Crud.DTO;

public class ItemDto
{
    public int Id { get; set; }          
    public int Quantity { get; set; }   
    public string UnitMeasure { get; set; }  
    public int ProductId { get; set; }   
    public string ProductName { get; set; }
    
    public ProductDto Product { get; set; }
}