using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OrderAndPaymentAPI.Entities;

namespace OrderAndPaymentAPI.Web.Pages.Account;

public class LoginModel : PageModel
{
    private readonly IUserService _userService;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IAccountService _accountService;
    private readonly IRefreshTokenService _refreshTokenService;

    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    public LoginModel(
        IUserService userService,
        IAccountService accountService,
        IRefreshTokenService refreshTokenService,
        IPasswordHasher<User> passwordHasher)
    {
        _userService = userService;
        _passwordHasher = passwordHasher;
        _accountService = accountService;
        _refreshTokenService = refreshTokenService;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Vui lòng nhập đầy đủ email và mật khẩu.";
            return Page();
        }

        var user = await _userService.GetByEmailAsync(Email);
        if (user is null || _passwordHasher.VerifyHashedPassword(user, user.Password, Password) == PasswordVerificationResult.Failed)
        {
            ErrorMessage = "Email hoặc mật khẩu không đúng.";
            return Page();
        }

        var refreshToken = _accountService.GenerateRefreshToken();
        await _refreshTokenService.AddTokenAsync(new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            User = user,
            ExpiredDate = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        });

        var accessToken = _accountService.GenerateAccessToken(user);
        Response.Cookies.Append("accessToken", accessToken, CreateTokenCookieOptions(TimeSpan.FromMinutes(10)));
        Response.Cookies.Append("refreshToken", refreshToken, CreateTokenCookieOptions(TimeSpan.FromDays(7)));

        
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Fullname),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(System.Globalization.CultureInfo.InvariantCulture)),
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            });

        return RedirectToPage("/Index");
    }

    public async Task<IActionResult> OnPostLogoutAsync()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            await _refreshTokenService.UpdateTokenRevokeAsync(new RefreshTokenRequestDTO
            {
                RefreshToken = refreshToken
            });
        }

        Response.Cookies.Delete("accessToken");
        Response.Cookies.Delete("refreshToken");
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToPage("/Index");
    }

    private CookieOptions CreateTokenCookieOptions(TimeSpan lifetime) => new()
    {
        HttpOnly = true,
        Secure = Request.IsHttps,
        SameSite = SameSiteMode.Strict,
        Expires = DateTimeOffset.UtcNow.Add(lifetime)
    };
}
