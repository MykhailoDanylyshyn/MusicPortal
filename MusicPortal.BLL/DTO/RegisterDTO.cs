using System.ComponentModel.DataAnnotations;

namespace MusicPortal.BLL.DTO;

public class RegisterDTO
{
    [Display(Name = "Ім'я користувача")]
    [Required(ErrorMessage = "Поле є обов'язковим для заповнення.")]
    public required string Username { get; set; }

    [Display(Name = "Електронна пошта")]
    [Required(ErrorMessage = "Поле є обов'язковим для заповнення.")]
    [EmailAddress(ErrorMessage = "Некоректна адреса електронної пошти.")]
    public required string Email { get; set; }

    [Display(Name = "Пароль")]
    [Required(ErrorMessage = "Поле є обов'язковим для заповнення.")]
    [MinLength(8, ErrorMessage = "Пароль повинен містити щонайменше 8 символів.")]
    public required string Password { get; set; }
}
