using CoffeeGuide.Models.Enums;

namespace CoffeeGuide.Models;

public class Syrup
{
    public int Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? FlavorProfile { get; set; }
    public SyrupCategory Category { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<int> RecommendedRecipeIds { get; set; } = new();
    public string? ImagePath { get; set; }
}
