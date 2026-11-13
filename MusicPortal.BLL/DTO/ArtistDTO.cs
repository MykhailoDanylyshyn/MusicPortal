using System.ComponentModel.DataAnnotations;

namespace MusicPortal.BLL.DTO;

public class ArtistDTO
{
    public int Id { get; set; }

    [Display(Name = "Ім'я виконавця")]
    [Required(ErrorMessage = "Поле є обов'язковим для заповнення.")]
    public required string Name { get; set; }

    [Display(Name = "Біографія")]
    public string? Biography { get; set; }

    [Display(Name = "Країна")]
    public string? Country { get; set; }

    [Display(Name = "Фото виконавця")]
    public string? PhotoUrl { get; set; }

    public int RecordCount { get; set; }
}
