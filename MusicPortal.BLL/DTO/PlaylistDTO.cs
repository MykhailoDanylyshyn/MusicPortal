using System.ComponentModel.DataAnnotations;

namespace MusicPortal.BLL.DTO;

public class PlaylistDTO
{
    public int Id { get; set; }

    [Display(Name = "Назва плейлиста")]
    [Required(ErrorMessage = "Поле є обов'язковим для заповнення.")]
    public required string Name { get; set; }

    [Display(Name = "Опис плейлиста")]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public int UserId { get; set; }
}
