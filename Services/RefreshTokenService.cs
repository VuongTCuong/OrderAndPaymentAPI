
public interface IRefreshTokenService
{
    Task<RefreshToken> AddTokenAsync(RefreshToken token);
    Task<RefreshToken> GetTokenByTokenNameAsync(string tokenName);
}

public class RefreshTokenService: IRefreshTokenService
{
    private readonly IRefreshTokenRepository _refreshTokenRepo;
    public RefreshTokenService(IRefreshTokenRepository refreshTokenRepo)
    {
        _refreshTokenRepo = refreshTokenRepo;
    }

    public Task<RefreshToken> AddTokenAsync(RefreshToken token)
    {
        return _refreshTokenRepo.AddTokenAsync(token);
    }

    public Task<RefreshToken> GetTokenByTokenNameAsync(string tokenName)
    {
        return _refreshTokenRepo.GetTokenByTokenNameAsync(tokenName);
    }

}