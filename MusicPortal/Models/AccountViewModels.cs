using System.ComponentModel.DataAnnotations;

namespace MusicPortal.Models;

public class RegisterViewModel
{
    [Display(Name = "Ім'я користувача")]
    [Required(ErrorMessage = "Вкажіть ім'я користувача.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Ім'я має містити від 3 до 50 символів.")]
    public string Username { get; set; } = string.Empty;

    [Display(Name = "Електронна пошта")]
    [Required(ErrorMessage = "Вкажіть електронну пошту.")]
    [EmailAddress(ErrorMessage = "Некоректна адреса електронної пошти.")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Пароль")]
    [Required(ErrorMessage = "Вкажіть пароль.")]
    [MinLength(8, ErrorMessage = "Пароль має містити щонайменше 8 символів.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Підтвердження пароля")]
    [Compare(nameof(Password), ErrorMessage = "Паролі не збігаються.")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class LoginViewModel
{
    [Display(Name = "Ім'я користувача")]
    [Required(ErrorMessage = "Вкажіть ім'я користувача.")]
    public string Username { get; set; } = string.Empty;

    [Display(Name = "Пароль")]
    [Required(ErrorMessage = "Вкажіть пароль.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
