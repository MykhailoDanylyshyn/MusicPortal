namespace MusicPortal.DAL.Entities;

public class Record
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string AudioUrl { get; set; } = string.Empty;

    public int? ArtistId { get; set; }

    public Artist? Artist { get; set; }

    public int GenreId { get; set; }

    public Genre Genre { get; set; } = null!;

    public ICollection<PlaylistRecord> PlaylistRecords { get; set; }
        = new List<PlaylistRecord>();
}