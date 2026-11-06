using CoffeeGuide.Models.Enums;
using CoffeeGuide.Services.Interfaces;
using CoffeeGuide.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace CoffeeGuide.Pages.Recipes;

public class IndexModel : CatalogPageModel
{
    private readonly ICoffeeRecipeService _recipes;
    private readonly DetailsFactory _details;
    private readonly IStringLocalizer<SharedResource> _l;

    public IndexModel(ICoffeeRecipeService recipes, DetailsFactory details, IStringLocalizer<SharedResource> l)
    {
        _recipes = recipes;
        _details = details;
        _l = l;
    }

    [BindProperty(SupportsGet = true, Name = "q")] public string? Query { get; set; }
    [BindProperty(SupportsGet = true)] public RecipeCategory? Category { get; set; }

    public List<CardViewModel> Cards { get; private set; } = new();

    public async Task OnGetAsync()
    {
        var items = await _recipes.GetAllAsync(Query, Category);
        Cards = items.Select(r => r.ToCard(_l)).ToList();
    }

    // GET /recipes?handler=Details&id=3
    public async Task<IActionResult> OnGetDetailsAsync(int id)
    {
        var recipe = await _recipes.GetByIdAsync(id);
        return DetailsResult(recipe is null ? null : _details.ForRecipe(recipe));
    }
}
