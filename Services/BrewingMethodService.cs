using CoffeeGuide.Models;
using CoffeeGuide.Services.Interfaces;

namespace CoffeeGuide.Services;

public class BrewingMethodService : IBrewingMethodService
{
    private readonly IContentLocalizationService _content;

    public BrewingMethodService(IContentLocalizationService content) => _content = content;

    private IReadOnlyList<BrewingMethod> Methods => _content.GetContent().BrewingMethods;

    public Task<IReadOnlyList<BrewingMethod>> GetAllAsync() =>
        Task.FromResult<IReadOnlyList<BrewingMethod>>(Methods.ToList());

    public Task<BrewingMethod?> GetByIdAsync(int id) => Task.FromResult(_content.GetContent().FindMethod(id));

    public Task<BrewingMethod?> GetBySlugAsync(string slug) =>
        Task.FromResult(Methods.FirstOrDefault(m => string.Equals(m.Slug, slug, StringComparison.OrdinalIgnoreCase)));

    public Task<IReadOnlyList<BrewingMethod>> SearchAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Task.FromResult<IReadOnlyList<BrewingMethod>>(new List<BrewingMethod>());

        var result = Methods
            .Where(m => SearchHelper.Matches(query, m.Name, m.Description, string.Join(" ", m.Equipment)))
            .ToList();
        return Task.FromResult<IReadOnlyList<BrewingMethod>>(result);
    }

    public Task<IReadOnlyList<BrewingMethod>> GetFeaturedAsync(int count) =>
        Task.FromResult<IReadOnlyList<BrewingMethod>>(Methods.Where(m => m.IsFeatured).Take(Math.Max(count, 0)).ToList());
}
