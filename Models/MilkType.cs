using CoffeeGuide.Models.Enums;

namespace CoffeeGuide.Models;

public class MilkType
{
    public int Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public MilkCategory Category { get; set; }
    public decimal? FatContentPercent { get; set; }   
    public string? Texture { get; set; }
    public string? FoamingProperties { get; set; }
    public string? FlavorProfile { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
}
