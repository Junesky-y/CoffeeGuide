using CoffeeGuide.Data;
using CoffeeGuide.Services;
using CoffeeGuide.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ---------- Налаштування та залежності (Dependency Injection) ----------
builder.Services.Configure<ContentOptions>(builder.Configuration.GetSection("Content"));
var contentOptions = builder.Configuration.GetSection("Content").Get<ContentOptions>() ?? new ContentOptions();

builder.Services.AddRazorPages();

// Singleton: читає JSON один раз і кешує окремо для кожної мови
builder.Services.AddSingleton<ContentStore>();

// Scoped: залежать від мови поточного запиту
builder.Services.AddScoped<IContentLocalizationService, ContentLocalizationService>();
builder.Services.AddScoped<ICoffeeBeanService, CoffeeBeanService>();
builder.Services.AddScoped<IMilkService, MilkService>();
builder.Services.AddScoped<ISyrupService, SyrupService>();
builder.Services.AddScoped<IBrewingMethodService, BrewingMethodService>();
builder.Services.AddScoped<ICoffeeRecipeService, CoffeeRecipeService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();

// ---------- Перевірка контенту під час запуску: помилки одразу видно в консолі ----------
try
{
    var store = app.Services.GetRequiredService<ContentStore>();
    foreach (var language in contentOptions.SupportedLanguages)
    {
        store.Get(language);
    }
}
catch (ContentLoadException ex)
{
    app.Logger.LogError(ex, "Не вдалося завантажити контент. Перевірте файли в папці Data/Json.");
}

app.Run();
