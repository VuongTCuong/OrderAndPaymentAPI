
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderAndPaymentAPI.DTO;
using OrderAndPaymentAPI.Entities;
using OrderAndPaymentAPI.Services;

namespace OrderAndPaymentAPI.Controllers;

[ApiController]
[Route("api/product")]
public class ProductController: ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }
    [HttpGet]
    public async Task<IActionResult> GetProduct()
    {
        //call function from service
        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
    }

    [HttpGet("{Id}")]
    public async Task<IActionResult> GetProductById(int Id)
    {
        var product = await _productService.GetProductByIdAsync(Id);
        if (product == null)
        {
            return NotFound("Cannot find any product");
        }
        return Ok(product);
    }
    [Authorize(Roles = "admin")]
    [HttpPost("add")]

    public async Task<IActionResult> AddProduct([FromBody] ProductDTO productDTO)
    {
        var product = await _productService.AddProduct(productDTO);
        return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
    }

    [Authorize(Roles = "admin")]
    [HttpDelete("delete/{Id}")]
    public async Task<IActionResult> DeleteProductById(int Id)
    {
        bool result = await _productService.DeleteProductById(Id);
        if (result == false)
        {
            return NotFound("Cannot find any product to delete");
        }
        return NoContent();
    }
}