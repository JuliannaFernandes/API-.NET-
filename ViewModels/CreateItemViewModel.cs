using System.ComponentModel.DataAnnotations;

namespace Crud.ViewModels;

public class CreateItemViewModel
{
    [Required]
    public int Quantity { get; set; }
    public required string UnitMeasure { get; set; }
    public required int ProductId { get; set; }
    public int CartId { get; set; }
}