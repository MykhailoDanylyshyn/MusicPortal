using Microsoft.EntityFrameworkCore;
using MusicPortal.DAL.Entities;
using MusicPortal.DAL.EF;
using MusicPortal.DAL.Interfaces;

namespace MusicPortal.DAL.Repositories;

public class ArtistRepository(MusicPortalContext db) : IRepository<Artist>
{
    public async Task<IEnumerable<Artist>> GetAll()
    {
        return await db.Artists
            .Include(a => a.MusicalRecords)
            .ToListAsync();
    }

    public async Task<Artist?> Get(int id)
    {
        return await db.Artists
            .Include(a => a.MusicalRecords)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Artist?> Get(string name)
    {
        return await db.Artists
            .FirstOrDefaultAsync(a => a.Name == name);
    }

    public async Task Create(Artist artist) =>
        await db.Artists.AddAsync(artist);

    public void Update(Artist artist) =>
        db.Entry(artist).State = EntityState.Modified;

    public async Task Delete(int id)
    {
        Artist? artist = await db.Artists.FindAsync(id);

        if (artist != null)
        {
            db.Artists.Remove(artist);
        }
    }
}