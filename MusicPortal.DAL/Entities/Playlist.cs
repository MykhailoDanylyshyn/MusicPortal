namespace MusicPortal.DAL.Entities;

public class Playlist
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int UserId { get; set; }

    public User User { get; set; } = null!;

    public ICollection<PlaylistRecord> PlaylistRecords { get; set; }
        = new List<PlaylistRecord>();
}