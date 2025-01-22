using System.ComponentModel.DataAnnotations;

namespace Crud.ViewModels;

public class CreateCartViewModel
{
  [Required]
  public required int ItemId { get; set; }
}