
public interface IRefreshTokenService
{
    Task<RefreshToken> AddTokenAsync(RefreshToken token);
    Task<RefreshToken> GetTokenByTokenNameAsync(string tokenName);
    Task<RefreshToken> UpdateTokenRevokeAsync(RefreshTokenRequestDTO rtRequestDTO);
}

public class RefreshTokenService: IRefreshTokenService
{
    private readonly IRefreshTokenRepository _refreshTokenRepo;
    public RefreshTokenService(IRefreshTokenRepository refreshTokenRepo)
    {
        _refreshTokenRepo = refreshTokenRepo;
    }

    public async Task<RefreshToken> AddTokenAsync(RefreshToken token)
    {
        return await _refreshTokenRepo.AddTokenAsync(token);
    }

    public async Task<RefreshToken> GetTokenByTokenNameAsync(string tokenName)
    {
        return await _refreshTokenRepo.GetTokenByTokenNameAsync(tokenName);
    }

    public async Task<RefreshToken> UpdateTokenRevokeAsync(RefreshTokenRequestDTO rtRequestDTO)
    {
        return await _refreshTokenRepo.UpdateTokenRevokeAsync(rtRequestDTO);
    }

}