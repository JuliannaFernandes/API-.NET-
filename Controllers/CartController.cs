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
    return cart == null ? NotFound(new { message = "Carrinho não encontrado!" }) : Ok(cart);
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
        return NotFound(new { message = "Carrinho não encontrado!" });

    var item = await context.Items
        .Include(i => i.Product)
        .FirstOrDefaultAsync(i => i.Id == model.ItemId);

    if (item == null)
        return NotFound(new { message = "Item não encontrado!" });

    // Recuperar a quantidade antiga do item no carrinho
    var oldQuantity = cart.QuantityCart;

    // Atualizar a quantidade do item no estoque
    item.Quantity += oldQuantity; // Restaurar a quantidade antiga
    if (item.Quantity < model.QuantityCart)
        return BadRequest(new { message = "Quantidade insuficiente do item!" });

    item.Quantity -= model.QuantityCart; // Subtrair a nova quantidade

    // Atualizar a quantidade do item no carrinho
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
  
  //metodo para deletar um carrinho
  [HttpDelete("carts/{id}")]
  public async Task<IActionResult> DeleteAsync(
    [FromServices] AppDbContext context,
    [FromRoute] int id)
  {
    var cart = await context.Carts
      .FirstOrDefaultAsync(c => c.Id == id);

    if (cart == null)
      return NotFound(new { message = "Carrinho não encontrado!" });

    try
    {
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




    
