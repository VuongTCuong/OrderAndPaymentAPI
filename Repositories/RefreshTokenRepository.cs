

using Microsoft.EntityFrameworkCore;
using OrderAndPaymentAPI.Data;

public interface IRefreshTokenRepository
{
    Task<RefreshToken> AddTokenAsync(RefreshToken token);
    Task<RefreshToken> GetTokenByTokenNameAsync(string tokenName);
    Task<RefreshToken> UpdateTokenRevokeAsync(RefreshTokenRequestDTO rtRequestDTO);
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

    public async Task<RefreshToken> UpdateTokenRevokeAsync(RefreshTokenRequestDTO rtRequestDTO)
    {
        var token = _context.RefreshToken.FirstOrDefault(rf => rf.Token == rtRequestDTO.RefreshToken);
        if(token == null)
        {
            return null;
        }
        token.IsRevoked = true;
        await _context.SaveChangesAsync();
        return token;
    }
}