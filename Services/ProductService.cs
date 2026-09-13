using OrderAndPaymentAPI.DTO;
using OrderAndPaymentAPI.Entities;
using OrderAndPaymentAPI.Repositories;

namespace OrderAndPaymentAPI.Services;

public interface IProductService
{
    Task<List<Product>> GetAllProductsAsync();
    Task<Product> GetProductByIdAsync(int Id);
    Task<Product> AddProduct(ProductDTO productDTO);
    Task<bool> DeleteProductById(int Id);
}

public class ProductService: IProductService
{
    //attribute repository
    private readonly IProductRepository _productRepo;

    public ProductService(IProductRepository productRepo)
    {
        _productRepo = productRepo;
    }
    public async Task<List<Product>> GetAllProductsAsync()
    {
        var products = await _productRepo.GetAllAsync();

        return products;
    }

    public async Task<Product> GetProductByIdAsync(int Id)
    {   
        var product =  await _productRepo.GetByIdAsync(Id);
        return product;
    }

    public async Task<Product> AddProduct(ProductDTO productDTO)
    {   
        var product = new Product
        {
            Name = productDTO.Name,
            Price = productDTO.Price,
            Stock = productDTO.Stock
        };
        return await _productRepo.AddAsync(product);
    }

    public async Task<bool> DeleteProductById(int Id)
    {
        return await _productRepo.DeleteByIdAsync(Id);
    }
}
