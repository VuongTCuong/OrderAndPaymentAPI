
using Microsoft.EntityFrameworkCore;
using OrderAndPaymentAPI.Data;
using OrderAndPaymentAPI.DTO;
using OrderAndPaymentAPI.Entities;

namespace OrderAndPaymentAPI.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int Id);
    Task<Product> AddAsync(Product product);
    
    Task<Product> UpdateAsync(Product product, UpdateProductDTO updateProductDTO);
    Task<bool> DeleteByIdAsync(int Id);
}

public class ProductRepository: IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAllAsync(){
        return await _context.Product.ToListAsync();
        
    }

    public async Task<Product?> GetByIdAsync(int Id)
    {
        return await _context.Product.FindAsync(Id);    
    }

    public async Task<Product> AddAsync(Product product)
    {
        _context.Product.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }
    public async Task<Product> UpdateAsync(Product product, UpdateProductDTO updateProductDTO)
    {
        product.Name = updateProductDTO.Name;
        product.Price = updateProductDTO.Price;
        product.Stock = updateProductDTO.Stock;

        await _context.SaveChangesAsync();
        return product;
    }
    public async Task<bool> DeleteByIdAsync(int Id)
    {
        var product = await GetByIdAsync(Id);
        if(product == null)
        {
            return false;
        }
        _context.Product.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }
} 