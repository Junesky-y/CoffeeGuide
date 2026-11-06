using CoffeeGuide.Models.Enums;
using CoffeeGuide.Services.Interfaces;
using CoffeeGuide.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace CoffeeGuide.Pages.Milk;

public class IndexModel : CatalogPageModel
{
    private readonly IMilkService _milks;
    private readonly DetailsFactory _details;
    private readonly IStringLocalizer<SharedResource> _l;

    public IndexModel(IMilkService milks, DetailsFactory details, IStringLocalizer<SharedResource> l)
    {
        _milks = milks;
        _details = details;
        _l = l;
    }

    [BindProperty(SupportsGet = true)] public MilkCategory? Category { get; set; }

    public List<CardViewModel> Cards { get; private set; } = new();

    public async Task OnGetAsync()
    {
        var items = await _milks.GetAllAsync(Category);
        Cards = items.Select(m => m.ToCard(_l)).ToList();
    }

    public async Task<IActionResult> OnGetDetailsAsync(int id)
    {
        var milk = await _milks.GetByIdAsync(id);
        return DetailsResult(milk is null ? null : _details.ForMilk(milk));
    }
}
