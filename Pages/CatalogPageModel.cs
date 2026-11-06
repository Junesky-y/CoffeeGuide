using CoffeeGuide.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoffeeGuide.Pages;

/// <summary>Спільна база для сторінок-каталогів: єдина відповідь для обробника Details.</summary>
public abstract class CatalogPageModel : PageModel
{
    protected IActionResult DetailsResult(DetailsDto? details) =>
        details is null
            ? new JsonResult(new { error = "not_found" }) { StatusCode = StatusCodes.Status404NotFound }
            : new JsonResult(details);
}
