namespace CoffeeGuide.ViewModels;

public record CategoryTile(string Page, string TitleKey, string TextKey, string? ImagePath, string Placeholder, int Count);

public class HomeViewModel
{
    public string? HeroImage { get; init; }
    public List<CategoryTile> Tiles { get; init; } = new();
    public List<CardViewModel> FeaturedRecipes { get; init; } = new();
    public List<CardViewModel> PopularMethods { get; init; } = new();
}
