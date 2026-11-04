using CoffeeGuide.Models;
using CoffeeGuide.Models.Enums;

namespace CoffeeGuide.Services.Interfaces;

public interface ICoffeeBeanService
{
    Task<IReadOnlyList<CoffeeBean>> GetAllAsync(string? origin = null, string? variety = null, RoastLevel? roast = null);
    Task<CoffeeBean?> GetByIdAsync(int id);
    Task<CoffeeBean?> GetBySlugAsync(string slug);
    Task<IReadOnlyList<CoffeeBean>> SearchAsync(string query);
    Task<IReadOnlyList<string>> GetOriginsAsync();
    Task<IReadOnlyList<string>> GetVarietiesAsync();
}
