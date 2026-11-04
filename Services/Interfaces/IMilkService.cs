using CoffeeGuide.Models;
using CoffeeGuide.Models.Enums;

namespace CoffeeGuide.Services.Interfaces;

public interface IMilkService
{
    Task<IReadOnlyList<MilkType>> GetAllAsync(MilkCategory? category = null);
    Task<MilkType?> GetByIdAsync(int id);
    Task<MilkType?> GetBySlugAsync(string slug);
    Task<IReadOnlyList<MilkType>> SearchAsync(string query);
}
