using CoffeeGuide.Models;
using CoffeeGuide.Models.Enums;

namespace CoffeeGuide.Services.Interfaces;

public interface ISyrupService
{
    Task<IReadOnlyList<Syrup>> GetAllAsync(SyrupCategory? category = null);
    Task<Syrup?> GetByIdAsync(int id);
    Task<Syrup?> GetBySlugAsync(string slug);
    Task<IReadOnlyList<Syrup>> SearchAsync(string query);
}
