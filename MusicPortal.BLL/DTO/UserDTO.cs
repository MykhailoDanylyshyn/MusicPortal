using System.ComponentModel.DataAnnotations;

namespace MusicPortal.BLL.DTO;

public class UserDTO
{
    public int Id { get; set; }

    [Display(Name = "Ім'я користувача")]
    [Required(ErrorMessage = "Поле є обов'язковим для заповнення.")]
    public required string Username { get; set; }

    [Display(Name = "Електронна пошта")]
    [Required(ErrorMessage = "Поле є обов'язковим для заповнення.")]
    [EmailAddress(ErrorMessage = "Некоректна адреса електронної пошти.")]
    public required string Email { get; set; }

    public bool IsAdmin { get; set; }

    public bool IsRegistrationConfirmed { get; set; }

    public DateTime RegistrationDate { get; set; }
}
