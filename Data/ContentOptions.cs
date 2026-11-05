namespace CoffeeGuide.Data;

/// <summary>Налаштування з розділу "Content" у appsettings.json.</summary>
public class ContentOptions
{
    public string DefaultLanguage { get; set; } = "uk";
    public string[] SupportedLanguages { get; set; } = { "uk", "en" };
    public string JsonFolder { get; set; } = "Data/Json";
}
