using CoffeeGuide.Models;

namespace CoffeeGuide.Services.Interfaces;

public interface IBrewingMethodService
{
    Task<IReadOnlyList<BrewingMethod>> GetAllAsync();
    Task<BrewingMethod?> GetByIdAsync(int id);
    Task<BrewingMethod?> GetBySlugAsync(string slug);
    Task<IReadOnlyList<BrewingMethod>> SearchAsync(string query);
    Task<IReadOnlyList<BrewingMethod>> GetFeaturedAsync(int count);
}
