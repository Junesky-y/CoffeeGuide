using CoffeeGuide.Services.Interfaces;
using CoffeeGuide.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace CoffeeGuide.Pages.BrewingMethods;

public class IndexModel : CatalogPageModel
{
    private readonly IBrewingMethodService _methods;
    private readonly DetailsFactory _details;
    private readonly IStringLocalizer<SharedResource> _l;

    public IndexModel(IBrewingMethodService methods, DetailsFactory details, IStringLocalizer<SharedResource> l)
    {
        _methods = methods;
        _details = details;
        _l = l;
    }

    public List<CardViewModel> Cards { get; private set; } = new();

    public async Task OnGetAsync()
    {
        var items = await _methods.GetAllAsync();
        Cards = items.Select(m => m.ToCard(_l)).ToList();
    }

    public async Task<IActionResult> OnGetDetailsAsync(int id)
    {
        var method = await _methods.GetByIdAsync(id);
        return DetailsResult(method is null ? null : _details.ForMethod(method));
    }
}
