using Crud.Data;
using Crud.Models;
using Crud.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Crud.Controllers
{
    [ApiController]
    [Route("v1")]
    public class ProductController : ControllerBase
    {
        // metodo para listar todos os produtos
        [HttpGet("products")]
        public async Task<IActionResult> GetAsync(
            [FromServices] AppDbContext context)
        {
            var products = await context
                .Products
                .AsNoTracking()
                .ToListAsync();
            return Ok(products);
        }
        
        // metodo para buscar um produto pelo id
        [HttpGet("products/{id}")]
        public async Task<IActionResult> GetByIdAsync(
            [FromServices] AppDbContext context,
            [FromRoute]int id)
        {
            var products = await context
                .Products
                .AsNoTracking() 
                .FirstOrDefaultAsync(x => x.Id == id);
            return products == null ? NotFound(new {message = "Produto não encontrado!" }) : Ok(products);
        }
        
        // metodo para criar um novo produto
        [HttpPost("products")]
        public async Task<IActionResult> PostAsync(
            [FromServices] AppDbContext context,
            [FromBody] CreateProductViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = new ProductModel
            {
                Name = model.Name
            };

            try
            {
                await context.Products.AddAsync(product);
                await context.SaveChangesAsync();
                return Created($"v1/products/{product.Id}", new { message = "Produto cadastrado com sucesso!", product });
            }
            catch (Exception e)
            {
                return BadRequest( new {message = "Não foi possível cadastrar o produto!", error = e.Message });
            }
        }
        
        [HttpPut("products/{id}")]
        public async Task<IActionResult> PutAsync(
            [FromServices] AppDbContext context,
            [FromBody] UpdateProductViewModel model,
            [FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
                
            var product = await context
                .Products
                .FirstOrDefaultAsync(x => x.Id == id);
                
            if (product == null)
                return NotFound( new {message = "Produto não encontrado!" });
                
            try
            {
                product.Name = model.Name;
                    
                context.Products.Update(product);
                await context.SaveChangesAsync();
                    
                return Ok(new {message = "Produto atualizado com sucesso!" });
            }
            catch (Exception e)
            {
                return BadRequest( new {message = "Não foi possível atualizar o produto!", error = e.Message });
            }
        }
        
        [HttpDelete("products/{id}")]
        public async Task<IActionResult> DeleteAsync(
            [FromServices] AppDbContext context,
            [FromRoute] int id)
        {
            var product = await context
                .Products
                .FirstOrDefaultAsync(x => x.Id == id);
            
            if (product == null)
                return NotFound( new {message = "Produto não encontrado!" });
                
            try
            {
                context.Products.Remove(product);
                await context.SaveChangesAsync();
                return Ok( new {message = "Produto deletado com sucesso!" });
            }
            catch (Exception e)
            {
                return BadRequest( new {message = "Não foi possível deletar o produto!" ,error = e.Message });
            }
        }
    }
}