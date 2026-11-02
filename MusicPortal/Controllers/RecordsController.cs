using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MusicPortal.BLL.DTO;
using MusicPortal.BLL.Interfaces;
using MusicPortal.Models;

namespace MusicPortal.Controllers;

public class RecordsController : Controller
{
    private const long MaxAudioSize = 20 * 1024 * 1024;
    private static readonly string[] AudioExtensions = [".mp3", ".wav", ".ogg", ".m4a"];
    private readonly IRecordService _records;
    private readonly IArtistService _artists;
    private readonly IGenreService _genres;
    private readonly IWebHostEnvironment _environment;

    public RecordsController(IRecordService records, IArtistService artists,
        IGenreService genres, IWebHostEnvironment environment)
    {
        _records = records; _artists = artists; _genres = genres; _environment = environment;
    }

    public async Task<IActionResult> Index(string? search, int? genreId)
    {
        var records = await _records.GetAllRecords();
        if (!string.IsNullOrWhiteSpace(search))
            records = records.Where(x => x.Title.Contains(search, StringComparison.OrdinalIgnoreCase)
                || (x.ArtistName?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false));
        if (genreId.HasValue) records = records.Where(x => x.GenreId == genreId.Value);
        ViewBag.Search = search;
        ViewBag.GenreId = genreId;
        ViewBag.Genres = new SelectList(await _genres.GetAllGenres(), "Id", "Name", genreId);
        return View(records.OrderBy(x => x.Title));
    }

    public async Task<IActionResult> Details(int id)
    {
        var record = await _records.GetRecord(id);
        return record == null ? NotFound() : View(record);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new RecordFormViewModel();
        await LoadOptions(model);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RecordFormViewModel model)
    {
        ValidateAudio(model.AudioFile, required: true);
        if (model.GenreId == null && string.IsNullOrWhiteSpace(model.NewGenreName))
            ModelState.AddModelError(nameof(model.GenreId), "Оберіть жанр або введіть новий.");
        if (model.ArtistId.HasValue && string.IsNullOrWhiteSpace(model.NewArtistName)
            && !(await _artists.GetAllArtists()).Any(x => x.Id == model.ArtistId.Value))
            ModelState.AddModelError(nameof(model.ArtistId), "Обраного виконавця не знайдено.");
        if (model.GenreId.HasValue && string.IsNullOrWhiteSpace(model.NewGenreName)
            && !(await _genres.GetAllGenres()).Any(x => x.Id == model.GenreId.Value))
            ModelState.AddModelError(nameof(model.GenreId), "Обраного жанру не знайдено.");
        if (!ModelState.IsValid)
        {
            await LoadOptions(model);
            return View(model);
        }

        var artistId = await ResolveArtist(model);
        var genreId = await ResolveGenre(model);
        var audioUrl = await SaveAudio(model.AudioFile!);
        await _records.CreateRecord(new RecordDTO
        {
            Title = model.Title.Trim(), AudioUrl = audioUrl,
            ArtistId = artistId, GenreId = genreId!.Value
        });
        TempData["Success"] = "Композицію додано до каталогу.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var record = await _records.GetRecord(id);
        if (record == null) return NotFound();
        var model = new RecordFormViewModel { Title = record.Title, ArtistId = record.ArtistId,
            GenreId = record.GenreId, ExistingAudioUrl = record.AudioUrl };
        await LoadOptions(model);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, RecordFormViewModel model)
    {
        var existing = await _records.GetRecord(id);
        if (existing == null) return NotFound();
        ValidateAudio(model.AudioFile, required: false);
        if (model.GenreId == null && string.IsNullOrWhiteSpace(model.NewGenreName))
            ModelState.AddModelError(nameof(model.GenreId), "Оберіть жанр або введіть новий.");
        if (model.ArtistId.HasValue && string.IsNullOrWhiteSpace(model.NewArtistName)
            && !(await _artists.GetAllArtists()).Any(x => x.Id == model.ArtistId.Value))
            ModelState.AddModelError(nameof(model.ArtistId), "Обраного виконавця не знайдено.");
        if (model.GenreId.HasValue && string.IsNullOrWhiteSpace(model.NewGenreName)
            && !(await _genres.GetAllGenres()).Any(x => x.Id == model.GenreId.Value))
            ModelState.AddModelError(nameof(model.GenreId), "Обраного жанру не знайдено.");
        if (!ModelState.IsValid)
        {
            model.ExistingAudioUrl = existing.AudioUrl;
            await LoadOptions(model);
            return View(model);
        }
        var artistId = await ResolveArtist(model);
        var genreId = await ResolveGenre(model);
        var audioUrl = model.AudioFile is { Length: > 0 } ? await SaveAudio(model.AudioFile) : existing.AudioUrl;
        await _records.UpdateRecord(new RecordDTO { Id = id, Title = model.Title.Trim(), AudioUrl = audioUrl,
            ArtistId = artistId, GenreId = genreId!.Value });
        if (audioUrl != existing.AudioUrl) DeleteAudio(existing.AudioUrl);
        TempData["Success"] = "Зміни збережено.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var record = await _records.GetRecord(id);
        return record == null ? NotFound() : View(record);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var record = await _records.GetRecord(id);
        if (record == null) return NotFound();
        await _records.DeleteRecord(id);
        DeleteAudio(record.AudioUrl);
        TempData["Success"] = "Композицію видалено.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<int?> ResolveArtist(RecordFormViewModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.NewArtistName))
        {
            var name = model.NewArtistName.Trim();
            var all = await _artists.GetAllArtists();
            var found = all.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (found != null) return found.Id;
            await _artists.CreateArtist(new ArtistDTO { Name = name });
            all = await _artists.GetAllArtists();
            return all.First(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).Id;
        }
        return model.ArtistId;
    }

    private async Task<int?> ResolveGenre(RecordFormViewModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.NewGenreName))
        {
            var name = model.NewGenreName.Trim();
            var all = await _genres.GetAllGenres();
            var found = all.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (found != null) return found.Id;
            await _genres.CreateGenre(new GenreDTO { Name = name });
            all = await _genres.GetAllGenres();
            return all.First(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).Id;
        }
        return model.GenreId;
    }

    private void ValidateAudio(IFormFile? file, bool required)
    {
        if (file == null || file.Length == 0)
        {
            if (required) ModelState.AddModelError(nameof(RecordFormViewModel.AudioFile), "Оберіть аудіофайл.");
            return;
        }
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AudioExtensions.Contains(ext)) ModelState.AddModelError(nameof(RecordFormViewModel.AudioFile), "Дозволені формати: MP3, WAV, OGG, M4A.");
        if (file.Length > MaxAudioSize) ModelState.AddModelError(nameof(RecordFormViewModel.AudioFile), "Максимальний розмір файлу — 20 МБ.");
    }

    private async Task<string> SaveAudio(IFormFile file)
    {
        var folder = Path.Combine(_environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot"), "uploads");
        Directory.CreateDirectory(folder);
        var name = $"{Guid.NewGuid():N}{Path.GetExtension(file.FileName).ToLowerInvariant()}";
        await using var stream = System.IO.File.Create(Path.Combine(folder, name));
        await file.CopyToAsync(stream);
        return $"/uploads/{name}";
    }

    private void DeleteAudio(string? url)
    {
        if (string.IsNullOrWhiteSpace(url) || !url.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase)) return;
        var path = Path.Combine(_environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot"), url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
    }

    private async Task LoadOptions(RecordFormViewModel model)
    {
        model.Artists = (await _artists.GetAllArtists()).OrderBy(x => x.Name)
            .Select(x => new SelectListItem(x.Name, x.Id.ToString(), x.Id == model.ArtistId)).ToList();
        model.Genres = (await _genres.GetAllGenres()).OrderBy(x => x.Name)
            .Select(x => new SelectListItem(x.Name, x.Id.ToString(), x.Id == model.GenreId)).ToList();
    }
}
