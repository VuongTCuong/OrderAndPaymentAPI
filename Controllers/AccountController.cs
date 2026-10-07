
using System.Security.Principal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using OrderAndPaymentAPI.Entities;
using Microsoft.AspNetCore.Identity.Data;
using OrderAndPaymentAPI.DTO;

[ApiController]
[Route("api/auth")]
public class AccountController: ControllerBase
{

    private readonly IAccountService _accountService;
    private readonly IUserService _userService;
    private readonly IPasswordHasher<User> _passwordHasher;

    private readonly IRefreshTokenService _refreshTokenService;
    public AccountController(IAccountService accountService, IUserService userService, IRefreshTokenService refreshTokenService, IPasswordHasher<User> passwordHasher)
    {
        _accountService = accountService;
        _userService = userService;
        _passwordHasher = passwordHasher;
        _refreshTokenService = refreshTokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserDTO userDTO)
    {

        var user = await _userService.GetByEmailAsync(userDTO.Email);
      
        if (user == null || _passwordHasher.VerifyHashedPassword(user, user.Password, userDTO.Password) == PasswordVerificationResult.Failed)
        {
            return Unauthorized(new {message = "Wrong Email or Password"});
        }

        var newRefreshToken = _accountService.GenerateRefreshToken();

        await _refreshTokenService.AddTokenAsync(new RefreshToken{
            Token = newRefreshToken,
            UserId = user.Id,
            User = user,
            ExpiredDate = DateTime.UtcNow.AddDays(7),
            IsRevoked = false

        });    

        var token = _accountService.GenerateAccessToken(user);
        return Ok(
            new LoginResponseDTO
            {
                UserId = user.Id,
                Email = user.Email,
                AccessToken = token,
                RefreshToken = newRefreshToken
            }
        );
    }

    [Authorize]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequestDTO rtRequestDTO)
    {
        var refreshToken = await _refreshTokenService.GetTokenByTokenNameAsync(rtRequestDTO.RefreshToken);
        if(refreshToken == null || refreshToken.IsRevoked == true || refreshToken.ExpiredDate < DateTime.UtcNow)
        {
            return Unauthorized("Invalid or expired token");
        }

        var refreshTokenResponse = await _accountService.Refresh(refreshToken);
        return Ok(refreshTokenResponse);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshTokenRequestDTO rtRequestDTO)
    {
        
        var refreshToken = await _refreshTokenService.UpdateTokenRevokeAsync(rtRequestDTO);
        if(refreshToken == null){
            return NotFound("Cannot found any token");
        }
        return Ok(refreshToken);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequestDTO rrRequestDTO)
    {
        rrRequestDTO.Email = rrRequestDTO.Email.Trim();
        var user = await _userService.GetByEmailAsync(rrRequestDTO.Email);
        if (user is not null)
        {
            return BadRequest(new {error = "Email này đã được đăng ký."});
        }

        var response = await _accountService.RegisterAsync(rrRequestDTO);
        return Ok(response);
    }
}
