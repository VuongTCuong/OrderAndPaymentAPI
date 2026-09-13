using System.Collections.Concurrent;

namespace OrderAndPaymentAPI.Services;

public interface ITokenBlacklist
{
    void Revoke(string tokenId, DateTimeOffset expiresAt);
    bool IsRevoked(string tokenId);
}

public sealed class InMemoryTokenBlacklist : ITokenBlacklist
{
    private readonly ConcurrentDictionary<string, DateTimeOffset> _revokedTokens = new();

    public void Revoke(string tokenId, DateTimeOffset expiresAt)
    {
        _revokedTokens[tokenId] = expiresAt;
    }

    public bool IsRevoked(string tokenId)
    {
        if (!_revokedTokens.TryGetValue(tokenId, out var expiresAt))
        {
            return false;
        }

        if (expiresAt > DateTimeOffset.UtcNow)
        {
            return true;
        }

        _revokedTokens.TryRemove(tokenId, out _);
        return false;
    }
}