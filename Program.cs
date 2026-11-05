using System.Globalization;
using CoffeeGuide.Data;
using CoffeeGuide.Services;
using CoffeeGuide.Services.Interfaces;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

// ---------- Налаштування та залежності (Dependency Injection) ----------
builder.Services.Configure<ContentOptions>(builder.Configuration.GetSection("Content"));
var contentOptions = builder.Configuration.GetSection("Content").Get<ContentOptions>() ?? new ContentOptions();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
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

// ---------- Локалізація: мова береться з cookie, за замовчуванням українська ----------
var cultures = contentOptions.SupportedLanguages.Select(l => new CultureInfo(l)).ToList();
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(contentOptions.DefaultLanguage),
    SupportedCultures = cultures,
    SupportedUICultures = cultures,
    RequestCultureProviders = new List<IRequestCultureProvider> { new CookieRequestCultureProvider() }
});

app.UseRouting();

// Перемикач мови: /set-language?culture=en&returnUrl=/recipes
app.MapGet("/set-language", (string culture, string? returnUrl, HttpContext context) =>
{
    if (contentOptions.SupportedLanguages.Contains(culture))
    {
        context.Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                SameSite = SameSiteMode.Lax
            });
    }

    // Перенаправляємо лише на локальні адреси
    var target = !string.IsNullOrEmpty(returnUrl) && returnUrl[0] == '/'
                 && (returnUrl.Length == 1 || (returnUrl[1] != '/' && returnUrl[1] != '\\'))
        ? returnUrl
        : "/";
    return Results.LocalRedirect(target);
});

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
