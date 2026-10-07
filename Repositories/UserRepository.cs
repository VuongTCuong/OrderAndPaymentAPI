
using Microsoft.EntityFrameworkCore;
using OrderAndPaymentAPI.Data;
using OrderAndPaymentAPI.Entities;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task<User> GetByEmailAsync(string email);
    Task<User> GetByIdAsync(int Id);
}

public class UserRepository: IUserRepository {
    private readonly AppDbContext _context;
    public UserRepository (AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(User user)
    {
        _context.User.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        return await _context.User.SingleOrDefaultAsync(u => u.Email == email);
        
    }

    public async Task<User> GetByIdAsync(int Id)
    {
        return await _context.User.FindAsync(Id);
    }
}
