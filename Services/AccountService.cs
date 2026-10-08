

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OrderAndPaymentAPI.DTO;
using OrderAndPaymentAPI.Entities;


public interface IAccountService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    Task<RefreshTokenResponseDTO> Refresh(RefreshToken refreshToken);
    Task<RefreshToken> UpdateTokenRevokeAsync(RefreshTokenRequestDTO rtRequestDTO);
    Task<LoginResponseDTO> RegisterAsync(RegisterRequestDTO rrRequestDTO);
}

public class AccountService: IAccountService
{
    private readonly IConfiguration _config;
    private readonly IUserRepository _userRepo;
    private readonly IRefreshTokenRepository _refreshTokenRepo;
    private readonly IPasswordHasher<User> _passwordHasher;
    public AccountService(IConfiguration config, IUserRepository userRepo, 
                        IRefreshTokenRepository refreshTokenRepo, IPasswordHasher<User> passwordHasher)
    {
        _config = config;
        _userRepo = userRepo;
        _refreshTokenRepo = refreshTokenRepo;
        _passwordHasher = passwordHasher; 
    }

    public string GenerateAccessToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(10),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        return Convert.ToBase64String(randomBytes);
    }

    public async Task<RefreshTokenResponseDTO> Refresh(RefreshToken refreshToken)
    {
        var user = await _userRepo.GetByIdAsync(refreshToken.UserId);

        refreshToken.IsRevoked = true;
        var newRefreshToken = GenerateRefreshToken();
                  
        await _refreshTokenRepo.AddTokenAsync(new RefreshToken{
            Token = newRefreshToken,
            UserId = user.Id,
            User = user,
            ExpiredDate = DateTime.UtcNow.AddDays(7),
            IsRevoked = false

        });    

        return new RefreshTokenResponseDTO
        {
            AccessToken = GenerateAccessToken(user),
            RefreshToken = newRefreshToken
        };
    }

    public async Task<RefreshToken> UpdateTokenRevokeAsync(RefreshTokenRequestDTO rtRequestDTO)
    {
        return await _refreshTokenRepo.UpdateTokenRevokeAsync(rtRequestDTO);
    }

    public async Task<LoginResponseDTO> RegisterAsync(RegisterRequestDTO rrRequestDTO)
    {
        var user = new User
        {
            Fullname = rrRequestDTO.Fullname.Trim(),
            Email = rrRequestDTO.Email,
            Role = "user",
            CreatedDate = DateTime.UtcNow
        };
        user.Password = _passwordHasher.HashPassword(user, rrRequestDTO.Password);
        await _userRepo.AddAsync(user);

        // sau khi đăng ký sẽ tự động đăng nhập và tạo 2 token

        var newRefreshToken = GenerateRefreshToken();
        await _refreshTokenRepo.AddTokenAsync(new RefreshToken{
            Token = newRefreshToken,
            UserId = user.Id,
            User = user,
            ExpiredDate = DateTime.UtcNow.AddDays(7),
            IsRevoked = false

        });    

        var token = GenerateAccessToken(user);
        return new LoginResponseDTO
        {
            UserId = user.Id,
            Email = user.Email,
            AccessToken = token,
            RefreshToken = newRefreshToken
        };
    }
}