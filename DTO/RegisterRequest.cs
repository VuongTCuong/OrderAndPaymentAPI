
using System.ComponentModel.DataAnnotations;

namespace OrderAndPaymentAPI.DTO;

public class RegisterRequestDTO
{
    public string Fullname {get;set;} = string.Empty;
    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
    public string Email {get;set;} = string.Empty;
    public string Password {get;set;} = string.Empty;

}
