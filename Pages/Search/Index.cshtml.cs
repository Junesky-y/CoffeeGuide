using CoffeeGuide.Services.Interfaces;
using CoffeeGuide.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace CoffeeGuide.Pages.Search;

public class IndexModel : PageModel
{
    private const int MaxQueryLength = 100;

    private readonly ICoffeeBeanService _beans;
    private readonly IMilkService _milks;
    private readonly ISyrupService _syrups;
    private readonly IBrewingMethodService _methods;
    private readonly ICoffeeRecipeService _recipes;
    private readonly IStringLocalizer<SharedResource> _l;

    public IndexModel(ICoffeeBeanService beans, IMilkService milks, ISyrupService syrups,
        IBrewingMethodService methods, ICoffeeRecipeService recipes, IStringLocalizer<SharedResource> l)
    {
        _beans = beans;
        _milks = milks;
        _syrups = syrups;
        _methods = methods;
        _recipes = recipes;
        _l = l;
    }

    [BindProperty(SupportsGet = true, Name = "q")] public string? Query { get; set; }

    public SearchViewModel Result { get; private set; } = new();

    public async Task OnGetAsync()
    {
        var query = Query?.Trim();
        if (string.IsNullOrEmpty(query)) return;
        if (query.Length > MaxQueryLength) query = query[..MaxQueryLength];

        var groups = new List<SearchGroup>();
        AddGroup(groups, "Nav_Recipes", (await _recipes.SearchAsync(query)).Select(x => x.ToCard(_l)));
        AddGroup(groups, "Nav_Beans", (await _beans.SearchAsync(query)).Select(x => x.ToCard(_l)));
        AddGroup(groups, "Nav_Milk", (await _milks.SearchAsync(query)).Select(x => x.ToCard(_l)));
        AddGroup(groups, "Nav_Syrups", (await _syrups.SearchAsync(query)).Select(x => x.ToCard(_l)));
        AddGroup(groups, "Nav_Methods", (await _methods.SearchAsync(query)).Select(x => x.ToCard(_l)));

        Result = new SearchViewModel { Query = query, Groups = groups };
    }

    private static void AddGroup(List<SearchGroup> groups, string titleKey, IEnumerable<CardViewModel> cards)
    {
        var list = cards.ToList();
        if (list.Count > 0) groups.Add(new SearchGroup(titleKey, list));
    }
}
