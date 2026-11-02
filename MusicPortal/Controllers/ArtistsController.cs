using Microsoft.AspNetCore.Mvc;
using MusicPortal.BLL.DTO;
using MusicPortal.BLL.Interfaces;

namespace MusicPortal.Controllers;

public class ArtistsController(IArtistService service) : Controller
{
    public async Task<IActionResult> Index() => View(await service.GetAllArtists());
    public IActionResult Create() => View(new ArtistDTO { Name = string.Empty });
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ArtistDTO model)
    {
        if (!ModelState.IsValid) return View(model);
        await service.CreateArtist(model);
        TempData["Success"] = "Виконавця додано.";
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Edit(int id)
    {
        var item = await service.GetArtist(id);
        return item == null ? NotFound() : View(item);
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ArtistDTO model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);
        await service.UpdateArtist(model);
        TempData["Success"] = "Дані виконавця оновлено.";
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Delete(int id)
    {
        var item = await service.GetArtist(id);
        return item == null ? NotFound() : View(item);
    }
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await service.GetArtist(id);
        if (item == null) return NotFound();
        if (item.RecordCount > 0)
        {
            TempData["Error"] = $"Не можна видалити виконавця: до нього прив'язано композицій — {item.RecordCount}.";
            return RedirectToAction(nameof(Index));
        }
        await service.DeleteArtist(id);
        TempData["Success"] = "Виконавця видалено.";
        return RedirectToAction(nameof(Index));
    }
}
