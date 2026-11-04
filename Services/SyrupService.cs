using CoffeeGuide.Models;
using CoffeeGuide.Models.Enums;
using CoffeeGuide.Services.Interfaces;

namespace CoffeeGuide.Services;

public class SyrupService : ISyrupService
{
    private readonly IContentLocalizationService _content;

    public SyrupService(IContentLocalizationService content) => _content = content;

    private IReadOnlyList<Syrup> Syrups => _content.GetContent().Syrups;

    public Task<IReadOnlyList<Syrup>> GetAllAsync(SyrupCategory? category = null)
    {
        IEnumerable<Syrup> query = Syrups;
        if (category.HasValue) query = query.Where(s => s.Category == category.Value);
        return Task.FromResult<IReadOnlyList<Syrup>>(query.ToList());
    }

    public Task<Syrup?> GetByIdAsync(int id) => Task.FromResult(_content.GetContent().FindSyrup(id));

    public Task<Syrup?> GetBySlugAsync(string slug) =>
        Task.FromResult(Syrups.FirstOrDefault(s => string.Equals(s.Slug, slug, StringComparison.OrdinalIgnoreCase)));

    public Task<IReadOnlyList<Syrup>> SearchAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Task.FromResult<IReadOnlyList<Syrup>>(new List<Syrup>());

        var result = Syrups
            .Where(s => SearchHelper.Matches(query, s.Name, s.Description, s.FlavorProfile))
            .ToList();
        return Task.FromResult<IReadOnlyList<Syrup>>(result);
    }
}
