using OrderAndPaymentAPI.DTO;
using OrderAndPaymentAPI.Entities;
using OrderAndPaymentAPI.Exceptions;
using OrderAndPaymentAPI.Repositories;

namespace OrderAndPaymentAPI.Services;

public interface IProductService
{
    Task<List<Product>> GetAllProductsAsync();
    Task<Product?> GetProductByIdAsync(int Id);
    Task<Product> AddProduct(ProductDTO productDTO);
    Task<Product> UpdateProductAsync(UpdateProductDTO updateProductDTO);
    Task DeleteProductById(int Id);
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

    public async Task<Product?> GetProductByIdAsync(int Id)
    {   
        var product =  await _productRepo.GetByIdAsync(Id);
        if (product == null)
        {
            throw new AppException("Sản phẩm không tồn tại",404);
        }
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

    public async Task<Product> UpdateProductAsync(UpdateProductDTO updateProductDTO)
    {
        var product = await _productRepo.GetByIdAsync(updateProductDTO.Id);
        if(product == null)
        {
            throw new AppException("Sản phẩm muốn chỉnh sửa không tồn tại", 404);
        }

        return await _productRepo.UpdateAsync(product,updateProductDTO);
    }
    public async Task DeleteProductById(int Id)
    {
        var product = await _productRepo.GetByIdAsync(Id);
        if (product == null)
        {
            throw new AppException("Sản phẩm muốn xoá không tồn tại", 404);
        }
        await _productRepo.DeleteByIdAsync(product);
    }
}
