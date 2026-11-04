using CoffeeGuide.Models;
using CoffeeGuide.Models.Enums;
using CoffeeGuide.Services.Interfaces;

namespace CoffeeGuide.Services;

public class CoffeeRecipeService : ICoffeeRecipeService
{
    private readonly IContentLocalizationService _content;

    public CoffeeRecipeService(IContentLocalizationService content) => _content = content;

    private IReadOnlyList<CoffeeRecipe> Recipes => _content.GetContent().Recipes;

    public Task<IReadOnlyList<CoffeeRecipe>> GetAllAsync(string? query = null, RecipeCategory? category = null)
    {
        IEnumerable<CoffeeRecipe> result = Recipes;

        if (category.HasValue)
            result = result.Where(r => r.Category == category.Value);
        if (!string.IsNullOrWhiteSpace(query))
            result = result.Where(r => SearchHelper.Matches(query, r.Name));

        return Task.FromResult<IReadOnlyList<CoffeeRecipe>>(result.ToList());
    }

    public Task<CoffeeRecipe?> GetByIdAsync(int id) => Task.FromResult(_content.GetContent().FindRecipe(id));

    public Task<CoffeeRecipe?> GetBySlugAsync(string slug) =>
        Task.FromResult(Recipes.FirstOrDefault(r => string.Equals(r.Slug, slug, StringComparison.OrdinalIgnoreCase)));

    public Task<IReadOnlyList<CoffeeRecipe>> SearchAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Task.FromResult<IReadOnlyList<CoffeeRecipe>>(new List<CoffeeRecipe>());

        var result = Recipes
            .Where(r => SearchHelper.Matches(query, r.Name, r.ShortDescription, r.FullDescription,
                string.Join(" ", r.Ingredients.Select(i => i.Ingredient?.Name))))
            .ToList();
        return Task.FromResult<IReadOnlyList<CoffeeRecipe>>(result);
    }

    public Task<IReadOnlyList<CoffeeRecipe>> GetFeaturedAsync(int count) =>
        Task.FromResult<IReadOnlyList<CoffeeRecipe>>(Recipes.Where(r => r.IsFeatured).Take(Math.Max(count, 0)).ToList());
}
