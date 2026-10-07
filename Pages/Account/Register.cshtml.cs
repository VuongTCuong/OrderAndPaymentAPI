using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OrderAndPaymentAPI.Entities;

namespace OrderAndPaymentAPI.Web.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly IUserService _userService;
    private readonly IPasswordHasher<User> _passwordHasher;

    public RegisterModel(IUserService userService, IPasswordHasher<User> passwordHasher)
    {
        _userService = userService;
        _passwordHasher = passwordHasher;
    }

    [BindProperty]
    [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
    public string FullName { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var email = Email.Trim();
        if (await _userService.GetByEmailAsync(email) is not null)
        {
            ModelState.AddModelError(nameof(Email), "Email này đã được đăng ký.");
            return Page();
        }

        var user = new User
        {
            Fullname = FullName.Trim(),
            Email = email,
            Role = "user",
            CreatedDate = DateTime.UtcNow
        };
        user.Password = _passwordHasher.HashPassword(user, Password);
        await _userService.AddAsync(user);

        TempData["RegistrationSuccess"] = "Đăng ký thành công. Vui lòng đăng nhập.";
        return RedirectToPage("/Account/Login");
    }   
}
