using Microsoft.EntityFrameworkCore;
using MusicPortal.DAL.EF;
using MusicPortal.DAL.Entities;
using MusicPortal.DAL.Interfaces;

namespace MusicPortal.DAL.Repositories;

public class PlaylistRepository(MusicPortalContext db)
    : IRepository<Playlist>
{
    public async Task<IEnumerable<Playlist>> GetAll()
    {
        return await db.Playlists
            .Include(p => p.User)
            .Include(p => p.PlaylistRecords)
                .ThenInclude(pr => pr.Record)
                    .ThenInclude(r => r.Artist)
            .Include(p => p.PlaylistRecords)
                .ThenInclude(pr => pr.Record)
                    .ThenInclude(r => r.Genre)
            .ToListAsync();
    }

    public async Task<Playlist?> Get(int id)
    {
        return await db.Playlists
            .Include(p => p.User)
            .Include(p => p.PlaylistRecords)
                .ThenInclude(pr => pr.Record)
                    .ThenInclude(r => r.Artist)
            .Include(p => p.PlaylistRecords)
                .ThenInclude(pr => pr.Record)
                    .ThenInclude(r => r.Genre)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Playlist?> Get(string name)
    {
        return await db.Playlists
            .FirstOrDefaultAsync(p => p.Name == name);
    }

    public async Task Create(Playlist playlist)
    {
        await db.Playlists.AddAsync(playlist);
    }

    public void Update(Playlist playlist)
    {
        db.Entry(playlist).State = EntityState.Modified;
    }

    public async Task Delete(int id)
    {
        Playlist? playlist = await db.Playlists.FindAsync(id);

        if (playlist != null)
        {
            db.Playlists.Remove(playlist);
        }
    }
}