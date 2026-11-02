using MusicPortal.DAL.Entities;
using MusicPortal.DAL.EF;
using MusicPortal.DAL.Interfaces;

namespace MusicPortal.DAL.Repositories;

public class EFUnitOfWork(MusicPortalContext db) : IUnitOfWork
{
    private RecordRepository? _recordRepository;
    private ArtistRepository? _artistRepository;
    private UserRepository? _userRepository;
    private GenreRepository? _genreRepository;
    private PlaylistRepository? _playlistRepository;

    public IRepository<Record> Records =>
        _recordRepository ??= new RecordRepository(db);

    public IRepository<Artist> Artists =>
        _artistRepository ??= new ArtistRepository(db);

    public IRepository<User> Users =>
        _userRepository ??= new UserRepository(db);

    public IRepository<Genre> Genres =>
        _genreRepository ??= new GenreRepository(db);

    public IRepository<Playlist> Playlists =>
    _playlistRepository ??= new PlaylistRepository(db);

    public async Task Save() => await db.SaveChangesAsync();
}
