using Microsoft.AspNetCore.Mvc;
using MusicPortal.BLL.Interfaces;

namespace MusicPortal.Controllers;

public class HomeController(IRecordService records, IArtistService artists, IGenreService genres) : Controller
{
    public async Task<IActionResult> Index()
    {
        ViewBag.RecordCount = (await records.GetAllRecords()).Count();
        ViewBag.ArtistCount = (await artists.GetAllArtists()).Count();
        ViewBag.GenreCount = (await genres.GetAllGenres()).Count();
        ViewBag.LatestRecords = (await records.GetAllRecords()).OrderByDescending(x => x.Id).Take(6).ToList();
        return View();
    }
    public IActionResult Privacy() => View();
    public IActionResult Error() => View();
}
