using System.ComponentModel.DataAnnotations;

namespace Crud.ViewModels;

public class CreateProductViewModel
{
    [Required]
    public required string Name { get; set; }
}