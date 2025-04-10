using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductRepository repo) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts(string? collection,string? type,string? sort)
    {
        return Ok(await repo.GetProductsAsync(collection,type,sort));
    }
    [HttpGet("{id:int}")]//api/products/3
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await repo.GetProductByIdAsync(id);

        if(product == null) return NotFound();

        return product;
    }
    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        repo.AddProduct(product);

        if(await repo.SaveChangesAsync())
        {
            return CreatedAtAction("GetProduct",new{ id = product.Id},product);
        }

        return BadRequest("Problem creating the product");
    }
    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateProduct(int id,Product product)
    {
        if(product.Id != id || !ProductExists(id))
            return BadRequest("Cannot Update this Product.");

        repo.UpdateProduct(product);

        if(await repo.SaveChangesAsync())
        {
            return NoContent();
        }

        return BadRequest("Problem updating the product");


    }
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await repo.GetProductByIdAsync(id);

        if(product == null) return NotFound();

        repo.DeleteProduct(product);

        if(await repo.SaveChangesAsync())
        {
            return NoContent();
        }
        
        return BadRequest("Problem deleting the product");

    }
    [HttpGet("collections")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetCollections()
    {
        return Ok(await repo.GetCollectionsAsync());
    }
     [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
    {
        return Ok(await repo.GetTypeAsync());
    }

    private bool ProductExists(int id)
    {
        return repo.ProductExists(id);
    }
}