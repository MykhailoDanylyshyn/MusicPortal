using MusicPortal.BLL.DTO;

namespace MusicPortal.BLL.Interfaces;

public interface IArtistService
{
    Task<IEnumerable<ArtistDTO>> GetAllArtists();

    Task<ArtistDTO?> GetArtist(int id);

    Task CreateArtist(ArtistDTO artist);

    Task UpdateArtist(ArtistDTO artist);

    Task DeleteArtist(int id);
}
