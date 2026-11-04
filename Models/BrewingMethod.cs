namespace CoffeeGuide.Models;

public class BrewingMethod
{
    public int Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Equipment { get; set; } = new();
    public string? RecommendedGrindSize { get; set; }
    public int? WaterTemperatureMinC { get; set; }
    public int? WaterTemperatureMaxC { get; set; }
    public int? BrewTimeSeconds { get; set; }
    public string? CoffeeToWaterRatio { get; set; }    // наприклад "1:16"
    public List<string> Steps { get; set; } = new();   // порядок у списку = порядок кроків
    public string? ImagePath { get; set; }
    public bool IsFeatured { get; set; }               // показувати на головній
}
