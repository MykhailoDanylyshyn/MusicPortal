using System.ComponentModel.DataAnnotations;

namespace MusicPortal.BLL.DTO;

public class RecordDTO
{
    public int Id { get; set; }

    [Display(Name = "Назва композиції")]
    [Required(ErrorMessage = "Вкажіть назву композиції.")]
    public required string Title { get; set; }

    [Display(Name = "Аудіофайл")]
    public string AudioUrl { get; set; } = string.Empty;

    public int? ArtistId { get; set; }

    [Display(Name = "Виконавець")]
    public string? ArtistName { get; set; }

    [Display(Name = "Жанр")]
    [Required(ErrorMessage = "Оберіть жанр.")]
    public int GenreId { get; set; }

    public string GenreName { get; set; } = string.Empty;
}
