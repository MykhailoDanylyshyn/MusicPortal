using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MusicPortal.Models;

public class RecordFormViewModel
{
    [Required(ErrorMessage = "Вкажіть назву композиції.")]
    [StringLength(160, ErrorMessage = "Назва не може перевищувати 160 символів.")]
    [Display(Name = "Назва композиції")]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "Виконавець")]
    public int? ArtistId { get; set; }

    [Display(Name = "Новий виконавець")]
    [StringLength(120)]
    public string? NewArtistName { get; set; }

    [Display(Name = "Жанр")]
    public int? GenreId { get; set; }

    [Display(Name = "Новий жанр")]
    [StringLength(80)]
    public string? NewGenreName { get; set; }

    [Display(Name = "Аудіофайл")]
    public IFormFile? AudioFile { get; set; }

    public string? ExistingAudioUrl { get; set; }
    public IEnumerable<SelectListItem> Artists { get; set; } = [];
    public IEnumerable<SelectListItem> Genres { get; set; } = [];
}
