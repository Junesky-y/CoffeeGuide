using System.Text.Json.Serialization;

namespace CoffeeGuide.Models;

public class RecipeIngredient
{
    public int IngredientId { get; set; }
    public decimal? Quantity { get; set; }  
    public string? Unit { get; set; }      
    public int DisplayOrder { get; set; }

    [JsonIgnore]
    public Ingredient? Ingredient { get; set; }
}