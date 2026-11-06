using System.Globalization;
using CoffeeGuide.Helpers;
using CoffeeGuide.Models;
using CoffeeGuide.Services.Interfaces;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;

namespace CoffeeGuide.ViewModels;

/// <summary>
/// Будує DetailsDto для модального вікна з будь-якої сутності.
/// Порожні необов'язкові поля просто пропускаються: секція не з'являється, помилок немає.
/// </summary>
public class DetailsFactory
{
    private readonly IStringLocalizer<SharedResource> _l;
    private readonly IContentLocalizationService _content;
    private readonly LinkGenerator _links;

    public DetailsFactory(IStringLocalizer<SharedResource> l, IContentLocalizationService content, LinkGenerator links)
    {
        _l = l;
        _content = content;
        _links = links;
    }

    // ---------- Кавове зерно ----------
    public DetailsDto ForBean(CoffeeBean b)
    {
        var content = _content.GetContent();
        var dto = Start(b.Id, b.Slug, b.Name, $"{b.Origin} · {_l.EnumLabel(b.RoastLevel)}", b.ImagePath, "🌱", b.Description);

        dto.Add(Facts(
            ("Sec_Origin", b.Origin),
            ("Sec_Variety", b.Variety),
            ("Sec_Roast", _l.EnumLabel(b.RoastLevel)),
            ("Sec_Processing", b.ProcessingMethod)));
        dto.Add(ListOf("Sec_Flavors", b.FlavorNotes));
        dto.Add(LinksOf("Sec_BrewingMethods", b.RecommendedBrewingMethodIds
            .Select(id => content.FindMethod(id))
            .OfType<BrewingMethod>()
            .Select(m => Link(PageNames.Methods, m.Id, m.Name))));
        return dto;
    }

    // ---------- Молоко ----------
    public DetailsDto ForMilk(MilkType m)
    {
        var dto = Start(m.Id, m.Slug, m.Name, _l.EnumLabel(m.Category), m.ImagePath, "🥛", m.Description);

        dto.Add(Facts(
            ("Sec_Category", _l.EnumLabel(m.Category)),
            ("Sec_Fat", m.FatContentPercent.HasValue ? m.FatContentPercent.Value.ToString("0.##", CultureInfo.CurrentCulture) + " %" : null),
            ("Sec_Texture", m.Texture),
            ("Sec_Foam", m.FoamingProperties),
            ("Sec_FlavorProfile", m.FlavorProfile)));
        return dto;
    }

    // ---------- Сироп ----------
    public DetailsDto ForSyrup(Syrup s)
    {
        var content = _content.GetContent();
        var dto = Start(s.Id, s.Slug, s.Name, _l.EnumLabel(s.Category), s.ImagePath, "🍯", s.Description);

        dto.Add(Facts(
            ("Sec_Category", _l.EnumLabel(s.Category)),
            ("Sec_FlavorProfile", s.FlavorProfile)));
        dto.Add(LinksOf("Sec_RecommendedRecipes", s.RecommendedRecipeIds
            .Select(id => content.FindRecipe(id))
            .OfType<CoffeeRecipe>()
            .Select(r => Link(PageNames.Recipes, r.Id, r.Name))));
        return dto;
    }

    // ---------- Спосіб приготування ----------
    public DetailsDto ForMethod(BrewingMethod m)
    {
        var dto = Start(m.Id, m.Slug, m.Name, null, m.ImagePath, "♨️", m.Description);

        dto.Add(Facts(
            ("Sec_Grind", m.RecommendedGrindSize),
            ("Sec_Temp", FormatTemperature(m.WaterTemperatureMinC, m.WaterTemperatureMaxC)),
            ("Sec_Time", FormatDuration(m.BrewTimeSeconds)),
            ("Sec_Ratio", m.CoffeeToWaterRatio)));
        dto.Add(ListOf("Sec_Equipment", m.Equipment));
        dto.Add(ListOf("Sec_Steps", m.Steps, SectionTypes.Ordered));
        return dto;
    }

    // ---------- Рецепт ----------
    public DetailsDto ForRecipe(CoffeeRecipe r)
    {
        var content = _content.GetContent();
        var dto = Start(r.Id, r.Slug, r.Name, _l.EnumLabel(r.Category), r.ImagePath, "☕", r.FullDescription);

        dto.Add(Facts(("Sec_Servings", r.Servings?.ToString(CultureInfo.CurrentCulture))));
        dto.Add(ListOf("Sec_Ingredients", r.Ingredients.Select(FormatIngredient)));
        dto.Add(TextOf("Sec_Preparation", r.PreparationMethod));
        dto.Add(TextOf("Sec_History", r.HistoryAndCulture));

        if (r.RecommendedCoffeeBeanId is { } beanId && content.FindBean(beanId) is { } bean)
            dto.Add(LinksOf("Sec_Bean", new[] { Link(PageNames.Beans, bean.Id, bean.Name) }));

        if (r.RecommendedMilkId is { } milkId && content.FindMilk(milkId) is { } milk)
            dto.Add(LinksOf("Sec_Milk", new[] { Link(PageNames.Milk, milk.Id, milk.Name) }));

        dto.Add(LinksOf("Sec_Syrups", r.RecommendedSyrupIds
            .Select(id => content.FindSyrup(id))
            .OfType<Syrup>()
            .Select(s => Link(PageNames.Syrups, s.Id, s.Name))));
        return dto;
    }

    // ---------- Допоміжні методи ----------

    private static DetailsDto Start(int id, string slug, string title, string? subtitle, string? image, string emoji, string? summary) => new()
    {
        Id = id,
        Slug = slug,
        Title = title,
        Subtitle = subtitle,
        ImageUrl = image,
        Emoji = emoji,
        Summary = summary
    };

    private DetailSection? Facts(params (string Key, string? Value)[] rows)
    {
        var facts = rows
            .Where(r => !string.IsNullOrWhiteSpace(r.Value))
            .Select(r => new DetailFact(_l[r.Key].Value, r.Value!))
            .ToList();

        return facts.Count == 0 ? null : new DetailSection { Type = SectionTypes.Facts, Facts = facts };
    }

    private DetailSection? ListOf(string headingKey, IEnumerable<string>? items, string type = SectionTypes.List)
    {
        var list = (items ?? Enumerable.Empty<string>()).Where(i => !string.IsNullOrWhiteSpace(i)).ToList();
        return list.Count == 0 ? null : new DetailSection { Heading = _l[headingKey].Value, Type = type, Items = list };
    }

    private DetailSection? TextOf(string headingKey, string? text) =>
        string.IsNullOrWhiteSpace(text) ? null : new DetailSection { Heading = _l[headingKey].Value, Type = SectionTypes.Text, Text = text };

    private DetailSection? LinksOf(string headingKey, IEnumerable<DetailLink?> links)
    {
        var list = links.OfType<DetailLink>().ToList();
        return list.Count == 0 ? null : new DetailSection { Heading = _l[headingKey].Value, Type = SectionTypes.Links, Links = list };
    }

    /// <summary>Посилання на обробник Details іншої сторінки, наприклад /recipes?handler=Details&amp;id=3.</summary>
    private DetailLink? Link(string page, int id, string text)
    {
        var url = _links.GetPathByPage(page, "Details", new { id });
        return url is null ? null : new DetailLink(text, url);
    }

    private static string FormatIngredientAmount(RecipeIngredient i)
    {
        var parts = new List<string>();
        if (i.Quantity.HasValue) parts.Add(i.Quantity.Value.ToString("0.##", CultureInfo.CurrentCulture));
        if (!string.IsNullOrWhiteSpace(i.Unit)) parts.Add(i.Unit!);
        return string.Join(" ", parts);
    }

    private static string FormatIngredient(RecipeIngredient i)
    {
        var name = i.Ingredient?.Name ?? string.Empty;
        var amount = FormatIngredientAmount(i);
        return amount.Length == 0 ? name : $"{name} — {amount}";
    }

    private string? FormatTemperature(int? min, int? max)
    {
        if (min is null && max is null) return null;
        var lo = min ?? max!.Value;
        var hi = max ?? min!.Value;
        var unit = _l["Unit_C"].Value;
        return lo == hi ? $"{lo} {unit}" : $"{lo}–{hi} {unit}";
    }

    private string? FormatDuration(int? seconds)
    {
        if (seconds is null || seconds <= 0) return null;
        var minutes = seconds.Value / 60;
        var rest = seconds.Value % 60;
        var min = _l["Unit_Min"].Value;
        var sec = _l["Unit_Sec"].Value;

        if (minutes == 0) return $"{rest} {sec}";
        return rest == 0 ? $"{minutes} {min}" : $"{minutes} {min} {rest} {sec}";
    }
}
