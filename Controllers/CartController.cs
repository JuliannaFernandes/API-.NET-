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
    return cart == null ? NotFound(new { message = "Carrinho não encontrado" }) : Ok(cart);
  }

  //metodo para criar um novo carrinho
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
      return NotFound(new { message = "Item não encontrado." });

    var cart = new CartModel
    {
      ItemId = item.Id,
      Item = item
    };

    try
    {
      if(item.Quantity == 0)
        return BadRequest(new { message = "Item esgotado." });
      
      await context.Carts.AddAsync(cart);
      await context.SaveChangesAsync();

      item.Quantity -= 1; 
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

      var cartDTO = new CartDto
      {
        Id = cart.Id,
        Items = new List<ItemDto> { itemDto }
      };

      return Created($"v1/carts/{cart.Id}", cartDTO);
    }
    catch (Exception e)
    {
      return StatusCode(500, new
      {
        message = "Erro ao criar o carrinho.",
        error = e.Message,
        innerException = e.InnerException?.Message
      });
    }
  }

  //metodo para atualizar um carrinho
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
      return NotFound(new { message = "Carrinho não encontrado." });

    var item = await context.Items
      .Include(i => i.Product)
      .FirstOrDefaultAsync(i => i.Id == model.ItemId);;

    if (item == null)
      return NotFound(new { message = "Item não encontrado." });

    cart.ItemId = item.Id;
    cart.Item = item;

    try
    {
      await context.SaveChangesAsync();
      return Ok(cart);
    }
    catch (Exception e)
    {
      return StatusCode(500, new
      {
        message = "Erro ao atualizar o carrinho.",
        error = e.Message,
        innerException = e.InnerException?.Message
      });
    }
  }
  
  //metodo para deletar um carrinho
  [HttpDelete("carts/{id}")]
  public async Task<IActionResult> DeleteAsync(
    [FromServices] AppDbContext context,
    [FromRoute] int id)
  {
    var cart = await context.Carts
      .FirstOrDefaultAsync(c => c.Id == id);

    if (cart == null)
      return NotFound(new { message = "Carrinho não encontrado." });

    try
    {
      context.Carts.Remove(cart);
      await context.SaveChangesAsync();
      return NoContent();
    }
    catch (Exception e)
    {
      return StatusCode(500, new
      {
        message = "Erro ao deletar o carrinho.",
        error = e.Message,
        innerException = e.InnerException?.Message
      });
    }
  }
}




    
