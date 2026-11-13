namespace MusicPortal.DAL.Entities;

public class PlaylistRecord
{
    public int PlaylistId { get; set; }

    public Playlist Playlist { get; set; } = null!;

    public int RecordId { get; set; }

    public Record Record { get; set; } = null!;

    public int Position { get; set; }
}