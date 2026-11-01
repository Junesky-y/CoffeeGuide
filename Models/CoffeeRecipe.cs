using CoffeeGuide.Models.Enums;

namespace CoffeeGuide.Models;

public class CoffeeRecipe
{
    public int Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public RecipeCategory Category { get; set; }
    public string ShortDescription { get; set; } = string.Empty;
    public string FullDescription { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public string? HistoryAndCulture { get; set; }
    public string? PreparationMethod { get; set; }
    public List<RecipeIngredient> Ingredients { get; set; } = new();
    public int? RecommendedCoffeeBeanId { get; set; }
    public int? RecommendedMilkId { get; set; }
    public List<int> RecommendedSyrupIds { get; set; } = new();
    public int? Servings { get; set; }
}