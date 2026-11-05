using CoffeeGuide.Models;

namespace CoffeeGuide.Data;

/// <summary>
/// Повний набір контенту однією мовою. Після створення не змінюється,
/// тому його безпечно кешувати та використовувати з багатьох запитів одночасно.
/// </summary>
public sealed class ContentSet
{
    private readonly Dictionary<int, CoffeeBean> _beans;
    private readonly Dictionary<int, MilkType> _milks;
    private readonly Dictionary<int, Syrup> _syrups;
    private readonly Dictionary<int, BrewingMethod> _methods;
    private readonly Dictionary<int, CoffeeRecipe> _recipes;

    public IReadOnlyList<CoffeeBean> Beans { get; }
    public IReadOnlyList<MilkType> Milks { get; }
    public IReadOnlyList<Syrup> Syrups { get; }
    public IReadOnlyList<BrewingMethod> BrewingMethods { get; }
    public IReadOnlyList<CoffeeRecipe> Recipes { get; }
    public IReadOnlyList<Ingredient> Ingredients { get; }

    public ContentSet(
        List<CoffeeBean> beans, List<MilkType> milks, List<Syrup> syrups,
        List<BrewingMethod> methods, List<CoffeeRecipe> recipes, List<Ingredient> ingredients)
    {
        Beans = beans;
        Milks = milks;
        Syrups = syrups;
        BrewingMethods = methods;
        Recipes = recipes;
        Ingredients = ingredients;

        _beans = beans.ToDictionary(x => x.Id);
        _milks = milks.ToDictionary(x => x.Id);
        _syrups = syrups.ToDictionary(x => x.Id);
        _methods = methods.ToDictionary(x => x.Id);
        _recipes = recipes.ToDictionary(x => x.Id);
    }

    public CoffeeBean? FindBean(int id) => _beans.TryGetValue(id, out var v) ? v : null;
    public MilkType? FindMilk(int id) => _milks.TryGetValue(id, out var v) ? v : null;
    public Syrup? FindSyrup(int id) => _syrups.TryGetValue(id, out var v) ? v : null;
    public BrewingMethod? FindMethod(int id) => _methods.TryGetValue(id, out var v) ? v : null;
    public CoffeeRecipe? FindRecipe(int id) => _recipes.TryGetValue(id, out var v) ? v : null;
}
