using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicPortal.BLL.DTO;
using MusicPortal.BLL.Interfaces;

namespace MusicPortal.Controllers.Api;

[ApiController]
[Route("api/genres")]
[Authorize(Roles = "Admin")]
[Produces("application/json")]
public class GenresApiController(IGenreService genres) : ControllerBase
{
    // GET: api/genres
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GenreDTO>>> GetAll()
        => Ok(await genres.GetAllGenres());

    // GET: api/genres/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<GenreDTO>> GetById(int id)
    {
        var genre = await genres.GetGenre(id);
        return genre == null ? NotFound() : Ok(genre);
    }

    // POST: api/genres
    [HttpPost]
    public async Task<ActionResult<GenreDTO>> Create(GenreDTO request)
    {
        var allGenres = await genres.GetAllGenres();
        if (allGenres.Any(x => x.Name.Equals(request.Name.Trim(), StringComparison.OrdinalIgnoreCase)))
            return Conflict(new { message = "Жанр із такою назвою вже існує." });

        request.Name = request.Name.Trim();
        await genres.CreateGenre(request);
        var created = (await genres.GetAllGenres())
            .First(x => x.Name.Equals(request.Name.Trim(), StringComparison.OrdinalIgnoreCase));

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT: api/genres/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, GenreDTO request)
    {
        if (id != request.Id)
            return BadRequest(new { message = "ID у маршруті не збігається з ID у тілі запиту." });

        if (await genres.GetGenre(id) == null)
            return NotFound();

        if ((await genres.GetAllGenres()).Any(x => x.Id != id &&
            x.Name.Equals(request.Name.Trim(), StringComparison.OrdinalIgnoreCase)))
            return Conflict(new { message = "Жанр із такою назвою вже існує." });

        request.Name = request.Name.Trim();
        await genres.UpdateGenre(request);
        return NoContent();
    }

    // DELETE: api/genres/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var genre = await genres.GetGenre(id);
        if (genre == null)
            return NotFound();

        if (genre.RecordCount > 0)
            return Conflict(new { message = $"Неможливо видалити жанр: до нього прив'язано композицій — {genre.RecordCount}." });

        await genres.DeleteGenre(id);
        return NoContent();
    }
}
