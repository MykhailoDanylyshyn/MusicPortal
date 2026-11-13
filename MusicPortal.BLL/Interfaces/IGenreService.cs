using MusicPortal.BLL.DTO;

namespace MusicPortal.BLL.Interfaces;

public interface IGenreService
{
    Task<IEnumerable<GenreDTO>> GetAllGenres();

    Task<GenreDTO?> GetGenre(int id);

    Task CreateGenre(GenreDTO genre);

    Task UpdateGenre(GenreDTO genre);

    Task DeleteGenre(int id);
}
