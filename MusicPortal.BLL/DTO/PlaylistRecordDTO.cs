using System.ComponentModel.DataAnnotations;

namespace MusicPortal.BLL.DTO;

public class PlaylistRecordDTO
{
    [Display(Name = "Плейлист")]
    public int PlaylistId { get; set; }

    [Display(Name = "Музичний запис")]
    public int RecordId { get; set; }

    [Display(Name = "Позиція у плейлисті")]
    [Range(1, int.MaxValue, ErrorMessage = "Позиція повинна бути більшою за нуль.")]
    public int Position { get; set; }
}
