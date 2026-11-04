using CoffeeGuide.Models.Enums;

namespace CoffeeGuide.Models;

public class Ingredient
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public IngredientCategory Category { get; set; }
    public string? Description { get; set; }
    public string? ImagePath { get; set; }
}
