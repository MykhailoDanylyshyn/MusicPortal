using AutoMapper;
using MusicPortal.BLL.DTO;
using MusicPortal.BLL.Interfaces;
using MusicPortal.DAL.Entities;
using MusicPortal.DAL.Interfaces;

namespace MusicPortal.BLL.Services;

public class ArtistService : IArtistService
{
    private readonly IUnitOfWork database;
    private readonly IMapper mapper;

    public ArtistService(IUnitOfWork database, IMapper mapper)
    {
        this.database = database;
        this.mapper = mapper;
    }

    public async Task<IEnumerable<ArtistDTO>> GetAllArtists()
    {
        var artists = await database.Artists.GetAll();
        return mapper.Map<IEnumerable<ArtistDTO>>(artists);
    }

    public async Task<ArtistDTO?> GetArtist(int id)
    {
        var artist = await database.Artists.Get(id);
        return mapper.Map<ArtistDTO?>(artist);
    }

    public async Task CreateArtist(ArtistDTO artist)
    {
        var newArtist = new Artist
        {
            Name = artist.Name,
            Biography = artist.Biography,
            Country = artist.Country,
            PhotoUrl = artist.PhotoUrl
        };

        await database.Artists.Create(newArtist);
        await database.Save();
    }

    public async Task UpdateArtist(ArtistDTO artist)
    {
        var existingArtist = await database.Artists.Get(artist.Id);

        if (existingArtist == null)
            return;

        existingArtist.Name = artist.Name;
        existingArtist.Biography = artist.Biography;
        existingArtist.Country = artist.Country;
        existingArtist.PhotoUrl = artist.PhotoUrl;

        database.Artists.Update(existingArtist);
        await database.Save();
    }

    public async Task DeleteArtist(int id)
    {
        await database.Artists.Delete(id);
        await database.Save();
    }
}
