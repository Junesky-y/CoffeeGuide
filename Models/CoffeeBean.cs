using CoffeeGuide.Models.Enums;

namespace CoffeeGuide.Models;

public class CoffeeBean
{
    public int Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string Variety { get; set; } = string.Empty;      // Arabica, Robusta...
    public RoastLevel RoastLevel { get; set; }
    public List<string> FlavorNotes { get; set; } = new();
    public string? ProcessingMethod { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public List<int> RecommendedBrewingMethodIds { get; set; } = new();
}