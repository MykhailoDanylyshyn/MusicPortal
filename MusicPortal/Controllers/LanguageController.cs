using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace MusicPortal.Controllers;

public class LanguageController : Controller
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SetLanguage(string culture, string? returnUrl)
    {
        var supportedCultures = new[] { "uk-UA", "en-US" };
        if (!supportedCultures.Contains(culture))
            culture = "uk-UA";

        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                SameSite = SameSiteMode.Lax,
                Secure = Request.IsHttps
            });

        return LocalRedirect(Url.IsLocalUrl(returnUrl) ? returnUrl! : "/");
    }
}
