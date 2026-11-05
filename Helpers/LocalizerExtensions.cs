using Microsoft.Extensions.Localization;

namespace CoffeeGuide.Helpers;

public static class LocalizerExtensions
{
    /// <summary>
    /// Повертає підпис значення enum із .resx за ключем "ІмяEnum_Значення",
    /// наприклад RoastLevel_Light або MilkCategory_Plant.
    /// </summary>
    public static string EnumLabel<TEnum>(this IStringLocalizer localizer, TEnum value) where TEnum : struct, Enum
    {
        var key = typeof(TEnum).Name + "_" + value;
        return localizer[key].Value;
    }
}
