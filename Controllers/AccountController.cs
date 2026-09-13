
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/auth")]
public class AccountController: ControllerBase
{

    private readonly IAccountService _accountService;
    private readonly IUserService _userService;

    private readonly IRefreshTokenService _refreshTokenService;
    public AccountController(IAccountService accountService, IUserService userService, IRefreshTokenService refreshTokenService)
    {
        _accountService = accountService;
        _userService = userService;
        _refreshTokenService = refreshTokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserDTO userDTO)
    {

        var user = await _userService.GetByEmailAsync(userDTO.Email);
      
        if (user == null || user.Password != userDTO.Password )
        {
            return Unauthorized(new {message = "Wrong Email or Password"});
        }

        var token = _accountService.GenerateAccessToken(user);
        return Ok(
            new
            {
                AccessToken = token,
                RefreshToken = _accountService.GenerateRefreshToken()
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
            return Unauthorized();
        }
        refreshToken.IsRevoked = true;
        
        return Ok();
    }
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ","");
        
        return Ok(new {token});
    }
}