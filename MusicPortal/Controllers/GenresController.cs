using Microsoft.AspNetCore.Mvc;
using MusicPortal.BLL.DTO;
using MusicPortal.BLL.Interfaces;

namespace MusicPortal.Controllers;

public class GenresController(IGenreService service) : Controller
{
    public async Task<IActionResult> Index() => View(await service.GetAllGenres());
    public IActionResult Create() => View(new GenreDTO { Name = string.Empty });
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GenreDTO model)
    {
        if (!ModelState.IsValid) return View(model);
        await service.CreateGenre(model);
        TempData["Success"] = "Жанр додано.";
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Edit(int id)
    {
        var item = await service.GetGenre(id);
        return item == null ? NotFound() : View(item);
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, GenreDTO model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);
        await service.UpdateGenre(model);
        TempData["Success"] = "Жанр оновлено.";
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Delete(int id)
    {
        var item = await service.GetGenre(id);
        return item == null ? NotFound() : View(item);
    }
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await service.GetGenre(id);
        if (item == null) return NotFound();
        if (item.RecordCount > 0)
        {
            TempData["Error"] = $"Не можна видалити жанр: до нього прив'язано композицій — {item.RecordCount}.";
            return RedirectToAction(nameof(Index));
        }
        await service.DeleteGenre(id);
        TempData["Success"] = "Жанр видалено.";
        return RedirectToAction(nameof(Index));
    }
}
