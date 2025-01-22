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
            return products == null ? NotFound() : Ok(products);
        }
        
        // metodo para criar um novo produto
        [HttpPost("products")]
        public async Task<IActionResult> PostAsync(
            [FromServices] AppDbContext context,
            [FromBody] CreateProductViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var product = new ProductModel
            {
                Name = model.Name
            };

            try
            {
                await context.Products.AddAsync(product);
                await context.SaveChangesAsync();
                return Created($"v1/products/{{product.Id}}", product);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }
        
        [HttpPut("products/{id}")]
        public async Task<IActionResult> PutAsync(
            [FromServices] AppDbContext context,
            [FromBody] UpdateProductViewModel model,
            [FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest();
                
            var product = await context
                .Products
                .FirstOrDefaultAsync(x => x.Id == id);
                
            if (product == null)
                return NotFound();
                
            try
            {
                product.Name = model.Name;
                    
                context.Products.Update(product);
                await context.SaveChangesAsync();
                    
                return Ok(product);
            }
            catch (Exception e)
            {
                return BadRequest();
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
                return NotFound();
                
            try
            {
                context.Products.Remove(product);
                await context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }
    }
}