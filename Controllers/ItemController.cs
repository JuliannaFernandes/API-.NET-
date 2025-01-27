using Crud.Data;
using Crud.DTO;
using Crud.Models;
using Crud.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Crud.Controllers
{
    [ApiController]
    [Route("v1")]
    public class ItemController : ControllerBase
    {
        [HttpGet("items")]
        public async Task<IActionResult> GetItemsAsync(
            [FromServices] AppDbContext context)
        { 
            var items = await context.Items
                .Join(
                    context.Products,           
                    item => item.ProductId,      
                    product => product.Id,   
                    (item, product) => new
                    {
                        item.Id,
                        item.Quantity,
                        item.UnitMeasure,
                        item.ProductId,
                        ProductName = product.Name 
                    })
                .ToListAsync();
            return Ok(items);
        }
        
        [HttpGet("items/{id}")]
        public async Task<IActionResult> GetByIdAsync(
            [FromServices] AppDbContext context,
            [FromRoute] int id)
        {
            var item = await context
                .Items
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
            return item == null ? NotFound(new{message = "Item não encontrado!"}) : Ok(item);
        }
        
        [HttpPost("items")]
        public async Task<IActionResult> PostAsync(
            [FromServices] AppDbContext context,
            [FromBody] CreateItemViewModel model)
            
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var verifyExistenceProduct = await context
                .Products
                .FindAsync(model.ProductId);

            if (verifyExistenceProduct == null)
            {
                return NotFound(new { message = "Produto não encontrado!" });
            }

          var item = new ItemModel
          {   
            Quantity = model.Quantity,
            UnitMeasure = model.UnitMeasure,
            ProductId = model.ProductId,
            Product = verifyExistenceProduct
          };
          
          try
            {
                await context.Items.AddAsync(item);
                await context.SaveChangesAsync();
                
                var itemDto = new ItemDto
                {
                    Id = item.Id,
                    Quantity = item.Quantity,
                    UnitMeasure = item.UnitMeasure,
                    ProductId = item.ProductId,
                    ProductName = verifyExistenceProduct.Name
                };
                
                return Created($"v1/items/{{item.Id}}", new { message = "Item cadastrado com sucesso!", itemDto });;
            }
            catch (Exception e)
            {
                return BadRequest( new {message = "Não foi possível cadastrar o Item!", error = e.Message });
            }
        }
        
        [HttpPut("items/{id}")]
        public async Task<IActionResult> PutAsync(
            [FromServices] AppDbContext context,
            [FromBody] UpdateItemViewModel model,
            [FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var item = await context
                .Items
                .FindAsync(id);
                
            if (item == null)
                return NotFound(new { message = "Item não encontrado!" });
            
            var verifyExistenceProduct = await context
                .Products
                .FindAsync(model.ProductId);

            if (verifyExistenceProduct == null)
            {
                return NotFound(new { message = "Produto não encontrado!" });
            }

            item.Quantity = model.Quantity;
            item.UnitMeasure = model.UnitMeasure;
            item.ProductId = model.ProductId;
            item.Product = verifyExistenceProduct;

            try
            {
                context.Items.Update(item);
                await context.SaveChangesAsync();
                return Ok(item);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = "Erro ao atualizar o item!", error = e.Message });
            }
        }
        
        [HttpDelete("items/{id}")]
        public async Task<IActionResult> DeleteAsync(
            [FromServices] AppDbContext context,
            [FromRoute] int id)
        {
            var item = await context
                .Items
                .FindAsync(id);

            if (item == null)
                return NotFound(new { message = "Item não encontrado!" });

            try
            {
                context.Items.Remove(item);
                context.Carts.RemoveRange(item.Carts);
                
                await context.SaveChangesAsync();
                return Ok(new { message = "Item removido com sucesso!" });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = "Erro ao deletar o item!", error = e.Message });
            }
        }
    }
}