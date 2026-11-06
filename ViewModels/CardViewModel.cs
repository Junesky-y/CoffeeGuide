namespace CoffeeGuide.ViewModels;

/// <summary>Універсальна картка каталогу. Одна розмітка (_ContentCard) для всіх типів сутностей.</summary>
public record CardViewModel(
    int Id,
    string DetailsPage,     // сторінка, що віддає деталі: PageNames.*
    string Title,
    string? Subtitle,
    string? Badge,
    string? Text,
    string? ImagePath,
    string Emoji);          // заглушка, якщо немає зображення
