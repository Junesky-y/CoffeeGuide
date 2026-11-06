namespace CoffeeGuide.ViewModels;

/// <summary>
/// Універсальна модель для модального вікна. Сервер уже підставляє локалізовані заголовки,
/// а єдиний JavaScript-компонент просто малює секції — йому не важливо, що це: зерно чи рецепт.
/// </summary>
public class DetailsDto
{
    public int Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string? ImageUrl { get; set; }
    public string Emoji { get; set; } = "☕";
    public string? Summary { get; set; }
    public List<DetailSection> Sections { get; set; } = new();

    public void Add(DetailSection? section)
    {
        if (section is not null) Sections.Add(section);
    }
}

public static class SectionTypes
{
    public const string Text = "text";        // абзаци (розділювач — \n)
    public const string List = "list";        // маркований список
    public const string Ordered = "ordered";  // нумерований список
    public const string Facts = "facts";      // пари «назва — значення»
    public const string Links = "links";      // кнопки, що відкривають інший запис
}

public class DetailSection
{
    public string? Heading { get; set; }
    public string Type { get; set; } = SectionTypes.Text;
    public string? Text { get; set; }
    public List<string> Items { get; set; } = new();
    public List<DetailFact> Facts { get; set; } = new();
    public List<DetailLink> Links { get; set; } = new();
}

public record DetailFact(string Label, string Value);

public record DetailLink(string Text, string Url);
