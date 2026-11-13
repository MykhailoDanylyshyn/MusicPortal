using AutoMapper;
using MusicPortal.BLL.DTO;
using MusicPortal.BLL.Interfaces;
using MusicPortal.DAL.Entities;
using MusicPortal.DAL.Interfaces;

namespace MusicPortal.BLL.Services;

public class GenreService : IGenreService
{
    private readonly IUnitOfWork database;
    private readonly IMapper mapper;

    public GenreService(IUnitOfWork database, IMapper mapper)
    {
        this.database = database;
        this.mapper = mapper;
    }

    public async Task<IEnumerable<GenreDTO>> GetAllGenres()
    {
        var genres = await database.Genres.GetAll();
        return mapper.Map<IEnumerable<GenreDTO>>(genres);
    }

    public async Task<GenreDTO?> GetGenre(int id)
    {
        var genre = await database.Genres.Get(id);
        return mapper.Map<GenreDTO?>(genre);
    }

    public async Task CreateGenre(GenreDTO genre)
    {
        var newGenre = new Genre
        {
            Name = genre.Name,
            Description = genre.Description
        };

        await database.Genres.Create(newGenre);
        await database.Save();
    }

    public async Task UpdateGenre(GenreDTO genre)
    {
        var existingGenre = await database.Genres.Get(genre.Id);

        if (existingGenre == null)
            return;

        existingGenre.Name = genre.Name;
        existingGenre.Description = genre.Description;

        database.Genres.Update(existingGenre);
        await database.Save();
    }

    public async Task DeleteGenre(int id)
    {
        await database.Genres.Delete(id);
        await database.Save();
    }
}
