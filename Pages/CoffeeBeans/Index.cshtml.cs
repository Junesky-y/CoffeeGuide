using CoffeeGuide.Models.Enums;
using CoffeeGuide.Services.Interfaces;
using CoffeeGuide.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace CoffeeGuide.Pages.CoffeeBeans;

public class IndexModel : CatalogPageModel
{
    private readonly ICoffeeBeanService _beans;
    private readonly DetailsFactory _details;
    private readonly IStringLocalizer<SharedResource> _l;

    public IndexModel(ICoffeeBeanService beans, DetailsFactory details, IStringLocalizer<SharedResource> l)
    {
        _beans = beans;
        _details = details;
        _l = l;
    }

    [BindProperty(SupportsGet = true)] public string? Origin { get; set; }
    [BindProperty(SupportsGet = true)] public string? Variety { get; set; }
    [BindProperty(SupportsGet = true)] public RoastLevel? Roast { get; set; }

    public List<CardViewModel> Cards { get; private set; } = new();
    public IReadOnlyList<string> Origins { get; private set; } = new List<string>();
    public IReadOnlyList<string> Varieties { get; private set; } = new List<string>();

    public async Task OnGetAsync()
    {
        Origins = await _beans.GetOriginsAsync();
        Varieties = await _beans.GetVarietiesAsync();
        var items = await _beans.GetAllAsync(Origin, Variety, Roast);
        Cards = items.Select(b => b.ToCard(_l)).ToList();
    }

    // GET /coffee-beans?handler=Details&id=1
    public async Task<IActionResult> OnGetDetailsAsync(int id)
    {
        var bean = await _beans.GetByIdAsync(id);
        return DetailsResult(bean is null ? null : _details.ForBean(bean));
    }
}
