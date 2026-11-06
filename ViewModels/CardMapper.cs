using CoffeeGuide.Helpers;
using CoffeeGuide.Models;
using Microsoft.Extensions.Localization;

namespace CoffeeGuide.ViewModels;

/// <summary>Перетворює доменні моделі на картки. Тексти беруться з даних, підписи enum — з .resx.</summary>
public static class CardMapper
{
    private const int TextLimit = 140;

    public static CardViewModel ToCard(this CoffeeBean b, IStringLocalizer l) =>
        new(b.Id, PageNames.Beans, b.Name, b.Origin, l.EnumLabel(b.RoastLevel), Shorten(b.Description), b.ImagePath, "🌱");

    public static CardViewModel ToCard(this MilkType m, IStringLocalizer l) =>
        new(m.Id, PageNames.Milk, m.Name, m.FlavorProfile, l.EnumLabel(m.Category), Shorten(m.Description), m.ImagePath, "🥛");

    public static CardViewModel ToCard(this Syrup s, IStringLocalizer l) =>
        new(s.Id, PageNames.Syrups, s.Name, s.FlavorProfile, l.EnumLabel(s.Category), Shorten(s.Description), s.ImagePath, "🍯");

    public static CardViewModel ToCard(this BrewingMethod m, IStringLocalizer l) =>
        new(m.Id, PageNames.Methods, m.Name, null, null, Shorten(m.Description), m.ImagePath, "♨️");

    public static CardViewModel ToCard(this CoffeeRecipe r, IStringLocalizer l) =>
        new(r.Id, PageNames.Recipes, r.Name, null, l.EnumLabel(r.Category), r.ShortDescription, r.ImagePath, "☕");

    private static string? Shorten(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return null;
        text = text.Trim();
        if (text.Length <= TextLimit) return text;

        var cut = text.LastIndexOf(' ', TextLimit);
        if (cut < TextLimit / 2) cut = TextLimit;
        return text[..cut].TrimEnd(' ', ',', '.', ';', ':') + "…";
    }
}
