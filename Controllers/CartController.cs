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
      cart.QuantityCart,
      ProductName = cart.Item.Product.Name,
      ProductId = cart.Item.Product.Id
    })
    .ToListAsync();
  return Ok(carts);
}
  
  [HttpGet("carts/{id}")]
  public async Task<IActionResult> GetByIdAsync(
    [FromServices] AppDbContext context,
    [FromRoute] int id)
  {
    var cart = await context
      .Carts
      .AsNoTracking()
      .FirstOrDefaultAsync(x => x.Id == id);
    return cart == null ? NotFound(new { message = "Carrinho não encontrado!" }) : Ok(cart);
  }
  
 [HttpPost("carts")]
  public async Task<IActionResult> PostAsync(
    [FromServices] AppDbContext context,
    [FromBody] CreateCartViewModel model)
  {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    var item = await context.Items
      .Include(i => i.Product)
      .FirstOrDefaultAsync(i => i.Id == model.ItemId);

    if (item == null)
      return NotFound(new { message = "Item não encontrado!" });

    var cart = new CartModel
    {
      ItemId = item.Id,
      Item = item,
      QuantityCart = model.QuantityCart
    };

    try
    {
      if(item.Quantity < model.QuantityCart)
        return BadRequest(new { message = "Quantidade insuficiente do item!" });
      
      await context.Carts.AddAsync(cart);
      await context.SaveChangesAsync();

      item.Quantity -= model.QuantityCart; 
      await context.SaveChangesAsync(); 

      var itemDto = new ItemDto
      {
        Id = item.Id,
        Quantity = item.Quantity,
        UnitMeasure = item.UnitMeasure,
        Product = new ProductDto
        {
          Id = item.Product.Id,
          Name = item.Product.Name
        }
      };

      var cartDto = new CartDto
      {
        Id = cart.Id,
        Items = new List<ItemDto> { itemDto },
        QuantityCart = cart.QuantityCart
        
      };

      return Created($"v1/carts/{{cart.Id}}", new { message = "Carrinho criado com sucesso!", cartDto });;
    }
    catch (Exception e)
    {
      return BadRequest(new {message = "Não foi possível criar o carrinho!", error = e.Message });
    }
  }
  
 [HttpPut("carts/{id}")]
  public async Task<IActionResult> PutAsync(
      [FromServices] AppDbContext context,
      [FromRoute] int id,
      [FromBody] UpdateCartViewModel model)
  {
      if (!ModelState.IsValid)
          return BadRequest(ModelState);

      var cart = await context.Carts
          .Include(c => c.Item)
          .FirstOrDefaultAsync(c => c.Id == id);

      if (cart == null)
          return NotFound(new { message = "Carrinho não encontrado!" });

      var item = await context.Items
          .Include(i => i.Product)
          .FirstOrDefaultAsync(i => i.Id == model.ItemId);

      if (item == null)
          return NotFound(new { message = "Item não encontrado!" });
      
      var oldQuantity = cart.QuantityCart;
      
      item.Quantity += oldQuantity;
      if (item.Quantity < model.QuantityCart)
          return BadRequest(new { message = "Quantidade insuficiente do item!" });

      item.Quantity -= model.QuantityCart;
    
      cart.ItemId = item.Id;
      cart.Item = item;
      cart.QuantityCart = model.QuantityCart;

      try
      {
          await context.SaveChangesAsync();
          return Ok(cart);
      }
      catch (Exception e)
      {
          return BadRequest(new { message = "Não foi possível atualizar o carrinho!", error = e.Message });
      }
  }
  
  [HttpDelete("carts/{id}")]
  public async Task<IActionResult> DeleteAsync(
    [FromServices] AppDbContext context,
    [FromRoute] int id)
  {
    var cart = await context.Carts.Include(cartModel => cartModel.Item)
      .FirstOrDefaultAsync(c => c.Id == id);

    if (cart == null)
      return NotFound(new { message = "Carrinho não encontrado!" });

    try
    {
      cart.Item.Quantity += cart.QuantityCart;
      context.Carts.Remove(cart);
      await context.SaveChangesAsync();
      return NoContent();
    }
    catch (Exception e)
    {
      return BadRequest(new { message = "Não foi possível deletar o carrinho!", error = e.Message });
    }
  }
}
