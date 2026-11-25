using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using CoffeeGuide.Helpers;
using CoffeeGuide.Models;
using Microsoft.Extensions.Options;

namespace CoffeeGuide.Data;

/// <summary>Критична помилка завантаження контенту (немає файлу, зламаний JSON).</summary>
public sealed class ContentLoadException : Exception
{
    public ContentLoadException(string message, Exception? inner = null) : base(message, inner) { }
}

/// <summary>
/// Єдине джерело контенту: читає JSON-файли, перевіряє їх, зв'язує сутності між собою
/// і кешує результат окремо для кожної мови (файли читаються лише один раз).
/// Реєструється як Singleton.
/// </summary>
public sealed class ContentStore
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly ContentOptions _options;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<ContentStore> _logger;
    private readonly ConcurrentDictionary<string, Lazy<ContentSet>> _cache = new();

    public ContentStore(IOptions<ContentOptions> options, IWebHostEnvironment env, ILogger<ContentStore> logger)
    {
        _options = options.Value;
        _env = env;
        _logger = logger;
    }

    /// <summary>Повертає контент для мови. Невідома мова замінюється на мову за замовчуванням.</summary>
    public ContentSet Get(string language)
    {
        language = Normalize(language);
        var lazy = _cache.GetOrAdd(language, lang => new Lazy<ContentSet>(() => Build(lang)));
        try
        {
            return lazy.Value;
        }
        catch
        {
            // Не кешуємо помилку: після виправлення JSON наступний запит спробує ще раз.
            _cache.TryRemove(language, out _);
            throw;
        }
    }

    private string Normalize(string language) =>
        _options.SupportedLanguages.Contains(language, StringComparer.OrdinalIgnoreCase)
            ? language.ToLowerInvariant()
            : _options.DefaultLanguage;

    // ---------- Побудова набору контенту ----------

    private sealed class RawContent
    {
        public List<CoffeeBean> Beans { get; set; } = new();
        public List<MilkType> Milks { get; set; } = new();
        public List<Syrup> Syrups { get; set; } = new();
        public List<BrewingMethod> Methods { get; set; } = new();
        public List<CoffeeRecipe> Recipes { get; set; } = new();
        public List<Ingredient> Ingredients { get; set; } = new();
    }

    private ContentSet Build(string language)
    {
        var fallbackLanguage = _options.DefaultLanguage;

        // Резервну мову читаємо заново для кожної побудови, щоб різні мови
        // ніколи не ділили між собою одні й ті самі об'єкти.
        var data = Load(fallbackLanguage, required: true);
        Sanitize(data, fallbackLanguage);

        if (!string.Equals(language, fallbackLanguage, StringComparison.OrdinalIgnoreCase))
        {
            var local = Load(language, required: false);
            Sanitize(local, language);
            data = Merge(data, local);
        }

        Link(data, language);
        _logger.LogInformation("Контент [{Language}] завантажено: {Beans} зерен, {Milks} молока, {Syrups} сиропів, {Methods} способів, {Recipes} рецептів.",
            language, data.Beans.Count, data.Milks.Count, data.Syrups.Count, data.Methods.Count, data.Recipes.Count);

        return new ContentSet(data.Beans, data.Milks, data.Syrups, data.Methods, data.Recipes, data.Ingredients);
    }

    private RawContent Load(string language, bool required) => new()
    {
        Beans = ReadList<CoffeeBean>(language, "coffee-beans.json", required),
        Milks = ReadList<MilkType>(language, "milk-types.json", required),
        Syrups = ReadList<Syrup>(language, "syrups.json", required),
        Methods = ReadList<BrewingMethod>(language, "brewing-methods.json", required),
        Recipes = ReadList<CoffeeRecipe>(language, "coffee-recipes.json", required),
        Ingredients = ReadList<Ingredient>(language, "ingredients.json", required)
    };

    private List<T> ReadList<T>(string language, string fileName, bool required)
    {
        var path = Path.Combine(_env.ContentRootPath, _options.JsonFolder, language, fileName);

        if (!File.Exists(path))
        {
            if (required)
                throw new ContentLoadException($"Файл контенту не знайдено: {path}");

            _logger.LogWarning("[{Language}] Файл {File} відсутній, буде використано резервну мову.", language, fileName);
            return new List<T>();
        }

        try
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<T>>(json, JsonOptions) ?? new List<T>();
        }
        catch (Exception ex) when (ex is JsonException or IOException or UnauthorizedAccessException)
        {
            if (required)
                throw new ContentLoadException($"Не вдалося прочитати файл контенту: {path}", ex);

            _logger.LogError(ex, "[{Language}] Файл {File} пошкоджено, буде використано резервну мову.", language, fileName);
            return new List<T>();
        }
    }

    // ---------- Перевірка коректності ----------

    private void Sanitize(RawContent c, string language)
    {
        c.Beans = Validate(c.Beans, "coffee-beans", language, x => x.Id, x => x.Slug, x => x.Name);
        c.Milks = Validate(c.Milks, "milk-types", language, x => x.Id, x => x.Slug, x => x.Name);
        c.Syrups = Validate(c.Syrups, "syrups", language, x => x.Id, x => x.Slug, x => x.Name);
        c.Methods = Validate(c.Methods, "brewing-methods", language, x => x.Id, x => x.Slug, x => x.Name);
        c.Recipes = Validate(c.Recipes, "coffee-recipes", language, x => x.Id, x => x.Slug, x => x.Name);
        c.Ingredients = Validate(c.Ingredients, "ingredients", language, x => x.Id, null, x => x.Name);
    }

    /// <summary>Відкидає записи без Id/назви/slug та дублікати, не зупиняючи весь застосунок.</summary>
    private List<T> Validate<T>(List<T> items, string file, string language,
        Func<T, int> id, Func<T, string?>? slug, Func<T, string?> name)
    {
        var result = new List<T>();
        var ids = new HashSet<int>();
        var slugs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in items)
        {
            if (item is null) continue;

            var itemId = id(item);
            if (itemId <= 0 || string.IsNullOrWhiteSpace(name(item)))
            {
                _logger.LogWarning("[{Language}] {File}: запис з Id={Id} пропущено (немає Id або назви).", language, file, itemId);
                continue;
            }

            if (slug is not null)
            {
                var itemSlug = slug(item);
                if (string.IsNullOrWhiteSpace(itemSlug))
                {
                    _logger.LogWarning("[{Language}] {File}: запис Id={Id} пропущено (порожній slug).", language, file, itemId);
                    continue;
                }
                if (!slugs.Add(itemSlug))
                {
                    _logger.LogWarning("[{Language}] {File}: запис Id={Id} пропущено (дублікат slug '{Slug}').", language, file, itemId, itemSlug);
                    continue;
                }
            }

            if (!ids.Add(itemId))
            {
                _logger.LogWarning("[{Language}] {File}: запис пропущено (дублікат Id={Id}).", language, file, itemId);
                continue;
            }

            result.Add(item);
        }

        return result;
    }

    // ---------- Резервна мова ----------

    /// <summary>Записи з поточної мови замінюють записи з тим самим Id із резервної; решта лишається з резервної.</summary>
    private static RawContent Merge(RawContent fallback, RawContent local) => new()
    {
        Beans = MergeList(fallback.Beans, local.Beans, x => x.Id),
        Milks = MergeList(fallback.Milks, local.Milks, x => x.Id),
        Syrups = MergeList(fallback.Syrups, local.Syrups, x => x.Id),
        Methods = MergeList(fallback.Methods, local.Methods, x => x.Id),
        Recipes = MergeList(fallback.Recipes, local.Recipes, x => x.Id),
        Ingredients = MergeList(fallback.Ingredients, local.Ingredients, x => x.Id)
    };

    private static List<T> MergeList<T>(List<T> fallback, List<T> local, Func<T, int> id)
    {
        var map = fallback.ToDictionary(id);
        foreach (var item in local) map[id(item)] = item;
        return map.Values.OrderBy(id).ToList();
    }

    // ---------- Зв'язки між сутностями ----------

    private void Link(RawContent c, string language)
    {
        var missingImages = new List<string>();
        var ingredients = c.Ingredients.ToDictionary(x => x.Id);
        var beanIds = c.Beans.Select(x => x.Id).ToHashSet();
        var milkIds = c.Milks.Select(x => x.Id).ToHashSet();
        var syrupIds = c.Syrups.Select(x => x.Id).ToHashSet();
        var methodIds = c.Methods.Select(x => x.Id).ToHashSet();
        var recipeIds = c.Recipes.Select(x => x.Id).ToHashSet();

        foreach (var bean in c.Beans)
        {
            var owner = $"зерно {bean.Id}";
            bean.FlavorNotes ??= new();
            bean.ImagePath = CheckImage(bean.ImagePath, owner, language, missingImages);
            bean.RecommendedBrewingMethodIds = KeepKnown(bean.RecommendedBrewingMethodIds, methodIds, owner, "спосіб приготування", language);
        }

        foreach (var milk in c.Milks)
            milk.ImagePath = CheckImage(milk.ImagePath, $"молоко {milk.Id}", language, missingImages);

        foreach (var syrup in c.Syrups)
        {
            var owner = $"сироп {syrup.Id}";
            syrup.ImagePath = CheckImage(syrup.ImagePath, owner, language, missingImages);
            syrup.RecommendedRecipeIds = KeepKnown(syrup.RecommendedRecipeIds, recipeIds, owner, "рецепт", language);
        }

        foreach (var method in c.Methods)
        {
            method.Equipment ??= new();
            method.Steps ??= new();
            method.ImagePath = CheckImage(method.ImagePath, $"спосіб {method.Id}", language, missingImages);
        }

        foreach (var ingredient in c.Ingredients)
            ingredient.ImagePath = CheckImage(ingredient.ImagePath, $"інгредієнт {ingredient.Id}", language, missingImages);

        foreach (var recipe in c.Recipes)
        {
            var owner = $"рецепт {recipe.Id}";
            recipe.ImagePath = CheckImage(recipe.ImagePath, owner, language, missingImages);

            var kept = new List<RecipeIngredient>();
            foreach (var ri in recipe.Ingredients ?? new List<RecipeIngredient>())
            {
                if (!ingredients.TryGetValue(ri.IngredientId, out var ingredient))
                {
                    _logger.LogWarning("[{Language}] {Owner}: інгредієнт Id={Id} не існує, рядок пропущено.", language, owner, ri.IngredientId);
                    continue;
                }
                if (ri.Quantity.HasValue && ri.Quantity.Value < 0)
                {
                    _logger.LogWarning("[{Language}] {Owner}: від'ємна кількість для інгредієнта Id={Id}, значення скинуто.", language, owner, ri.IngredientId);
                    ri.Quantity = null;
                }
                ri.Ingredient = ingredient;
                kept.Add(ri);
            }
            recipe.Ingredients = kept.OrderBy(x => x.DisplayOrder).ToList();

            if (recipe.RecommendedCoffeeBeanId is { } beanId && !beanIds.Contains(beanId))
            {
                _logger.LogWarning("[{Language}] {Owner}: зерно Id={Id} не існує, зв'язок видалено.", language, owner, beanId);
                recipe.RecommendedCoffeeBeanId = null;
            }
            if (recipe.RecommendedMilkId is { } milkId && !milkIds.Contains(milkId))
            {
                _logger.LogWarning("[{Language}] {Owner}: молоко Id={Id} не існує, зв'язок видалено.", language, owner, milkId);
                recipe.RecommendedMilkId = null;
            }
            recipe.RecommendedSyrupIds = KeepKnown(recipe.RecommendedSyrupIds, syrupIds, owner, "сироп", language);
        }

        if (missingImages.Count > 0)
        {
            _logger.LogInformation("[{Language}] Не знайдено зображень: {Count} (буде показано заглушки). Наприклад: {Example}. Див. IMAGES.md.",
                language, missingImages.Count, missingImages[0]);
        }
    }

    private List<int> KeepKnown(List<int>? ids, HashSet<int> known, string owner, string kind, string language)
    {
        var result = new List<int>();
        foreach (var id in ids ?? new List<int>())
        {
            if (known.Contains(id)) result.Add(id);
            else _logger.LogWarning("[{Language}] {Owner}: {Kind} Id={Id} не існує, зв'язок видалено.", language, owner, kind, id);
        }
        return result;
    }

    /// <summary>
    /// Перевіряє фото. Якщо файлу немає в wwwroot (навіть з іншим розширенням), повертає null,
    /// і картка показує заглушку. Відсутні файли рахуються й виводяться одним повідомленням.
    /// </summary>
    private string? CheckImage(string? path, string owner, string language, List<string> missing)
    {
        if (string.IsNullOrWhiteSpace(path)) return null;

        if (!path.StartsWith('/') || path.Contains(".."))
        {
            _logger.LogWarning("[{Language}] {Owner}: некоректний шлях до зображення '{Path}'.", language, owner, path);
            return null;
        }

        var found = ImageHelper.FindExisting(_env.WebRootFileProvider, path);
        if (found is null) missing.Add(path);
        return found;
    }
}
