using CoffeeGuide.Models;
using CoffeeGuide.Models.Enums;
using CoffeeGuide.Services.Interfaces;

namespace CoffeeGuide.Services;

public class MilkService : IMilkService
{
    private readonly IContentLocalizationService _content;

    public MilkService(IContentLocalizationService content) => _content = content;

    private IReadOnlyList<MilkType> Milks => _content.GetContent().Milks;

    public Task<IReadOnlyList<MilkType>> GetAllAsync(MilkCategory? category = null)
    {
        IEnumerable<MilkType> query = Milks;
        if (category.HasValue) query = query.Where(m => m.Category == category.Value);
        return Task.FromResult<IReadOnlyList<MilkType>>(query.ToList());
    }

    public Task<MilkType?> GetByIdAsync(int id) => Task.FromResult(_content.GetContent().FindMilk(id));

    public Task<MilkType?> GetBySlugAsync(string slug) =>
        Task.FromResult(Milks.FirstOrDefault(m => string.Equals(m.Slug, slug, StringComparison.OrdinalIgnoreCase)));

    public Task<IReadOnlyList<MilkType>> SearchAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Task.FromResult<IReadOnlyList<MilkType>>(new List<MilkType>());

        var result = Milks
            .Where(m => SearchHelper.Matches(query, m.Name, m.Description, m.Texture, m.FoamingProperties, m.FlavorProfile))
            .ToList();
        return Task.FromResult<IReadOnlyList<MilkType>>(result);
    }
}
