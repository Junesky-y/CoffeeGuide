using System.Globalization;
using CoffeeGuide.Data;
using CoffeeGuide.Services.Interfaces;

namespace CoffeeGuide.Services;

public class ContentLocalizationService : IContentLocalizationService
{
    private readonly ContentStore _store;

    public ContentLocalizationService(ContentStore store) => _store = store;

    // Культуру запиту встановлює UseRequestLocalization у Program.cs
    public string CurrentLanguage => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

    public ContentSet GetContent() => _store.Get(CurrentLanguage);
}
