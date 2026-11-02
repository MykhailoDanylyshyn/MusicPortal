using System.ComponentModel.DataAnnotations;

namespace MusicPortal.BLL.DTO;

public class GenreDTO
{
    public int Id { get; set; }

    [Display(Name = "Назва жанру")]
    [Required(ErrorMessage = "Поле є обов'язковим для заповнення.")]
    public required string Name { get; set; }

    [Display(Name = "Опис жанру")]
    public string? Description { get; set; }

    public int RecordCount { get; set; }

}
