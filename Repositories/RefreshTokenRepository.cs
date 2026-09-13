

using Microsoft.EntityFrameworkCore;
using OrderAndPaymentAPI.Data;

public interface IRefreshTokenRepository
{
    Task<RefreshToken> AddTokenAsync(RefreshToken token);
    Task<RefreshToken> GetTokenByTokenNameAsync(string tokenName);
}

public class RefreshTokenRepository: IRefreshTokenRepository
{
    private readonly AppDbContext _context;
    public RefreshTokenRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<RefreshToken> AddTokenAsync(RefreshToken token)
    {
        _context.RefreshToken.Add(token);
        await _context.SaveChangesAsync();
        return token;
    }

    public async Task<RefreshToken> GetTokenByTokenNameAsync(string tokenName)
    {
        return await _context.RefreshToken.FirstOrDefaultAsync(rt => rt.Token == tokenName);
        
    }

}