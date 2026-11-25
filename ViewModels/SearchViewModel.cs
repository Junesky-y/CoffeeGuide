namespace CoffeeGuide.ViewModels;

public record SearchGroup(string TitleKey, List<CardViewModel> Cards);

public class SearchViewModel
{
    public string Query { get; init; } = string.Empty;
    public List<SearchGroup> Groups { get; init; } = new();
    public int Total => Groups.Sum(g => g.Cards.Count);
}
