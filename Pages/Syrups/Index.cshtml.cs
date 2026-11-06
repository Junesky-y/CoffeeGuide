using CoffeeGuide.Models.Enums;
using CoffeeGuide.Services.Interfaces;
using CoffeeGuide.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace CoffeeGuide.Pages.Syrups;

public class IndexModel : CatalogPageModel
{
    private readonly ISyrupService _syrups;
    private readonly DetailsFactory _details;
    private readonly IStringLocalizer<SharedResource> _l;

    public IndexModel(ISyrupService syrups, DetailsFactory details, IStringLocalizer<SharedResource> l)
    {
        _syrups = syrups;
        _details = details;
        _l = l;
    }

    [BindProperty(SupportsGet = true)] public SyrupCategory? Category { get; set; }

    public List<CardViewModel> Cards { get; private set; } = new();

    public async Task OnGetAsync()
    {
        var items = await _syrups.GetAllAsync(Category);
        Cards = items.Select(s => s.ToCard(_l)).ToList();
    }

    public async Task<IActionResult> OnGetDetailsAsync(int id)
    {
        var syrup = await _syrups.GetByIdAsync(id);
        return DetailsResult(syrup is null ? null : _details.ForSyrup(syrup));
    }
}
