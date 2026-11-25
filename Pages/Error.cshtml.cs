using CoffeeGuide.Data;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoffeeGuide.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{
    private readonly ILogger<ErrorModel> _logger;

    public ErrorModel(ILogger<ErrorModel> logger) => _logger = logger;

    public string TitleKey { get; private set; } = "Error_Title_Generic";
    public string TextKey { get; private set; } = "Error_Text_Generic";

    // /Error (виняток) або /Error/404 (повторне виконання для коду статусу)
    public void OnGet(int? code)
    {
        var exception = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;

        if (exception is ContentLoadException)
        {
            TitleKey = "Error_Title_Data";
            TextKey = "Error_Text_Data";
        }
        else if (exception is null && code is 404 or 400)
        {
            TitleKey = "Error_Title_404";
            TextKey = "Error_Text_404";
        }

        if (exception is not null)
        {
            // Деталі потрапляють у журнал, а не на екран користувача
            _logger.LogError(exception, "Необроблена помилка запиту {Path}", HttpContext.Request.Path);
        }
    }
}
