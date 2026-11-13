using MusicPortal.DAL.Entities;

namespace MusicPortal.DAL.Interfaces;

public interface IUnitOfWork
{
    IRepository<Record> Records { get; }

    IRepository<Artist> Artists { get; }

    IRepository<Genre> Genres { get; }

    IRepository<User> Users { get; }

    IRepository<Playlist> Playlists { get; }

    Task Save();
}
