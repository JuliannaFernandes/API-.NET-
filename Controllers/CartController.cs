using Crud.Data;
using Crud.DTO;
using Crud.Models;
using Crud.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Crud.Controllers;

[ApiController]
[Route("v1")]
public class CartController : ControllerBase
{
  //metodo para listar todos os carrinhos
  [HttpGet("carts")]
  public async Task<IActionResult> GetCartsAsync(
  [FromServices] AppDbContext context)
  {
    var carts = await context.Carts
      .Include(c => c.Item)
      .ThenInclude(i => i.Product)
      .Select(cart => new
      {
        cart.Id,
        cart.ItemId,
        Item = new
        {
          cart.Item.Quantity,
          cart.Item.UnitMeasure,
          Product = new
          {
            cart.Item.Product.Name
          }
        }
      })
      .ToListAsync();
    return Ok(carts);
  }
 
  //metodo para buscar um carrinho pelo id
  [HttpGet("carts/{id}")]
  public async Task<IActionResult> GetByIdAsync(
    [FromServices] AppDbContext context,
    [FromRoute] int id)
  {
    var cart = await context
      .Carts
      .AsNoTracking()
      .FirstOrDefaultAsync(x => x.Id == id);
    return cart == null ? NotFound(new{message = "Carrinho não encontrado"}) : Ok(cart);
  }
 
  
}




    
