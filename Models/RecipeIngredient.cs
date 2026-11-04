using System.Text.Json.Serialization;

namespace CoffeeGuide.Models;

public class RecipeIngredient
{
    public int IngredientId { get; set; }
    public decimal? Quantity { get; set; }   // null, якщо кількість не визначена
    public string? Unit { get; set; }        // мл, г, шт. або "за смаком"
    public int DisplayOrder { get; set; }

    // Заповнюється під час завантаження даних, у JSON не зберігається
    [JsonIgnore]
    public Ingredient? Ingredient { get; set; }
}
