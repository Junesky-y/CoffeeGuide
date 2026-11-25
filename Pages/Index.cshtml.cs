using CoffeeGuide.Helpers;
using CoffeeGuide.Services.Interfaces;
using CoffeeGuide.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace CoffeeGuide.Pages;

public class IndexModel : PageModel
{
    private readonly ICoffeeBeanService _beans;
    private readonly IMilkService _milks;
    private readonly ISyrupService _syrups;
    private readonly IBrewingMethodService _methods;
    private readonly ICoffeeRecipeService _recipes;
    private readonly IStringLocalizer<SharedResource> _l;
    private readonly IWebHostEnvironment _env;

    public IndexModel(ICoffeeBeanService beans, IMilkService milks, ISyrupService syrups,
        IBrewingMethodService methods, ICoffeeRecipeService recipes, IStringLocalizer<SharedResource> l, IWebHostEnvironment env)
    {
        _beans = beans;
        _milks = milks;
        _syrups = syrups;
        _methods = methods;
        _recipes = recipes;
        _l = l;
        _env = env;
    }

    // Необов'язкові фото розділів: wwwroot/images/categories/<назва>.jpg
    private string? Photo(string name) =>
        ImageHelper.FindExisting(_env.WebRootFileProvider, $"/images/categories/{name}.jpg");

    public HomeViewModel Home { get; private set; } = new();

    public async Task OnGetAsync()
    {
        var beans = await _beans.GetAllAsync();
        var milks = await _milks.GetAllAsync();
        var syrups = await _syrups.GetAllAsync();
        var methods = await _methods.GetAllAsync();
        var recipes = await _recipes.GetAllAsync();

        var featuredRecipes = await _recipes.GetFeaturedAsync(4);
        var featuredMethods = await _methods.GetFeaturedAsync(3);

        Home = new HomeViewModel
        {
            HeroImage = ImageHelper.FindExisting(_env.WebRootFileProvider, "/images/hero.jpg"),
            Tiles =
            {
                new CategoryTile(PageNames.Beans, "Nav_Beans", "Tile_Beans", Photo("beans"), Placeholders.Bean, beans.Count),
                new CategoryTile(PageNames.Milk, "Nav_Milk", "Tile_Milk", Photo("milk"), Placeholders.Milk, milks.Count),
                new CategoryTile(PageNames.Syrups, "Nav_Syrups", "Tile_Syrups", Photo("syrups"), Placeholders.Syrup, syrups.Count),
                new CategoryTile(PageNames.Methods, "Nav_Methods", "Tile_Methods", Photo("methods"), Placeholders.Method, methods.Count),
                new CategoryTile(PageNames.Recipes, "Nav_Recipes", "Tile_Recipes", Photo("recipes"), Placeholders.Recipe, recipes.Count)
            },
            FeaturedRecipes = featuredRecipes.Select(r => r.ToCard(_l)).ToList(),
            PopularMethods = featuredMethods.Select(m => m.ToCard(_l)).ToList()
        };
    }
}
