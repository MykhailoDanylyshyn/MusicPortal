using MusicPortal.BLL.DTO;

namespace MusicPortal.Models;

public class PaginatedRecordsViewModel
{
    public IReadOnlyList<RecordDTO> Items { get; init; } = [];
    public int CurrentPage { get; init; }
    public int TotalPages { get; init; }
    public int PageSize { get; init; }
    public string? Search { get; init; }
    public int? GenreId { get; init; }
}
