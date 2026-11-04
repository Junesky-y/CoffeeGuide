using CoffeeGuide.Models;
using CoffeeGuide.Models.Enums;

namespace CoffeeGuide.Services.Interfaces;

public interface ICoffeeRecipeService
{
    /// <summary>Усі рецепти; за потреби відфільтровані за назвою (query) та категорією.</summary>
    Task<IReadOnlyList<CoffeeRecipe>> GetAllAsync(string? query = null, RecipeCategory? category = null);
    Task<CoffeeRecipe?> GetByIdAsync(int id);
    Task<CoffeeRecipe?> GetBySlugAsync(string slug);
    /// <summary>Пошук за назвою, описом та інгредієнтами.</summary>
    Task<IReadOnlyList<CoffeeRecipe>> SearchAsync(string query);
    Task<IReadOnlyList<CoffeeRecipe>> GetFeaturedAsync(int count);
}
