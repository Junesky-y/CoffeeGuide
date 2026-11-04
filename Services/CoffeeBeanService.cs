using CoffeeGuide.Models;
using CoffeeGuide.Models.Enums;
using CoffeeGuide.Services.Interfaces;

namespace CoffeeGuide.Services;

// Методи повертають Task, бо дані вже в пам'яті (кеш ContentStore).
// Інтерфейс не зміниться, якщо пізніше джерелом стане база даних.
public class CoffeeBeanService : ICoffeeBeanService
{
    private readonly IContentLocalizationService _content;

    public CoffeeBeanService(IContentLocalizationService content) => _content = content;

    private IReadOnlyList<CoffeeBean> Beans => _content.GetContent().Beans;

    public Task<IReadOnlyList<CoffeeBean>> GetAllAsync(string? origin = null, string? variety = null, RoastLevel? roast = null)
    {
        IEnumerable<CoffeeBean> query = Beans;

        if (!string.IsNullOrWhiteSpace(origin))
            query = query.Where(b => string.Equals(b.Origin, origin, StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(variety))
            query = query.Where(b => string.Equals(b.Variety, variety, StringComparison.OrdinalIgnoreCase));
        if (roast.HasValue)
            query = query.Where(b => b.RoastLevel == roast.Value);

        return Task.FromResult<IReadOnlyList<CoffeeBean>>(query.ToList());
    }

    public Task<CoffeeBean?> GetByIdAsync(int id) => Task.FromResult(_content.GetContent().FindBean(id));

    public Task<CoffeeBean?> GetBySlugAsync(string slug) =>
        Task.FromResult(Beans.FirstOrDefault(b => string.Equals(b.Slug, slug, StringComparison.OrdinalIgnoreCase)));

    public Task<IReadOnlyList<CoffeeBean>> SearchAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Task.FromResult<IReadOnlyList<CoffeeBean>>(new List<CoffeeBean>());

        var result = Beans
            .Where(b => SearchHelper.Matches(query, b.Name, b.Description, b.Origin, b.Variety, string.Join(" ", b.FlavorNotes)))
            .ToList();
        return Task.FromResult<IReadOnlyList<CoffeeBean>>(result);
    }

    public Task<IReadOnlyList<string>> GetOriginsAsync() =>
        Task.FromResult<IReadOnlyList<string>>(Beans.Select(b => b.Origin).Where(o => !string.IsNullOrWhiteSpace(o))
            .Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(o => o).ToList());

    public Task<IReadOnlyList<string>> GetVarietiesAsync() =>
        Task.FromResult<IReadOnlyList<string>>(Beans.Select(b => b.Variety).Where(v => !string.IsNullOrWhiteSpace(v))
            .Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(v => v).ToList());
}
