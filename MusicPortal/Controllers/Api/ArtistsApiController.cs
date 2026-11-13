using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicPortal.BLL.DTO;
using MusicPortal.BLL.Interfaces;

namespace MusicPortal.Controllers.Api;

[ApiController]
[Route("api/artists")]
[Authorize(Roles = "Admin")]
[Produces("application/json")]
public class ArtistsApiController(IArtistService artists) : ControllerBase
{
    // GET: api/artists
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ArtistDTO>>> GetAll()
        => Ok(await artists.GetAllArtists());

    // GET: api/artists/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ArtistDTO>> GetById(int id)
    {
        var artist = await artists.GetArtist(id);
        return artist == null ? NotFound() : Ok(artist);
    }

    // POST: api/artists
    [HttpPost]
    public async Task<ActionResult<ArtistDTO>> Create(ArtistDTO request)
    {
        var allArtists = await artists.GetAllArtists();
        if (allArtists.Any(x => x.Name.Equals(request.Name.Trim(), StringComparison.OrdinalIgnoreCase)))
            return Conflict(new { message = "Виконавець із такою назвою вже існує." });

        request.Name = request.Name.Trim();
        await artists.CreateArtist(request);
        var created = (await artists.GetAllArtists())
            .First(x => x.Name.Equals(request.Name.Trim(), StringComparison.OrdinalIgnoreCase));

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT: api/artists/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ArtistDTO request)
    {
        if (id != request.Id)
            return BadRequest(new { message = "ID у маршруті не збігається з ID у тілі запиту." });

        if (await artists.GetArtist(id) == null)
            return NotFound();

        if ((await artists.GetAllArtists()).Any(x => x.Id != id &&
            x.Name.Equals(request.Name.Trim(), StringComparison.OrdinalIgnoreCase)))
            return Conflict(new { message = "Виконавець із такою назвою вже існує." });

        request.Name = request.Name.Trim();
        await artists.UpdateArtist(request);
        return NoContent();
    }

    // DELETE: api/artists/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var artist = await artists.GetArtist(id);
        if (artist == null)
            return NotFound();

        if (artist.RecordCount > 0)
            return Conflict(new { message = $"Неможливо видалити виконавця: до нього прив'язано композицій — {artist.RecordCount}." });

        await artists.DeleteArtist(id);
        return NoContent();
    }
}
