using CoffeeGuide.Data;

namespace CoffeeGuide.Services.Interfaces;

/// <summary>Надає контент тією мовою, яку обрав користувач (з урахуванням резервної мови).</summary>
public interface IContentLocalizationService
{
    string CurrentLanguage { get; }
    ContentSet GetContent();
}
