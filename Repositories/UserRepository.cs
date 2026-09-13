
using Microsoft.EntityFrameworkCore;
using OrderAndPaymentAPI.Data;
using OrderAndPaymentAPI.Entities;

public interface IUserRepository
{
    Task<User> GetByEmailAsync(string email);
}

public class UserRepository: IUserRepository {
    private readonly AppDbContext _context;
    public UserRepository (AppDbContext context)
    {
        _context = context;
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        return await _context.User.SingleOrDefaultAsync(u => u.Email == email);
        
    }
}