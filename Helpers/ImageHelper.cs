using Microsoft.Extensions.FileProviders;

namespace CoffeeGuide.Helpers;

public static class ImageHelper
{
    private static readonly string[] Extensions = { ".jpg", ".jpeg", ".png", ".webp" };

    /// <summary>
    /// Повертає шлях до існуючого файлу в wwwroot. Якщо файлу з указаним розширенням немає,
    /// пробує ту саму назву з розширеннями .jpg, .jpeg, .png, .webp. Якщо нічого немає — null.
    /// </summary>
    public static string? FindExisting(IFileProvider webRoot, string? path)
    {
        if (string.IsNullOrWhiteSpace(path) || !path.StartsWith('/') || path.Contains("..")) return null;

        if (webRoot.GetFileInfo(path).Exists) return path;

        var withoutExtension = path[..^Path.GetExtension(path).Length];
        foreach (var extension in Extensions)
        {
            var candidate = withoutExtension + extension;
            if (webRoot.GetFileInfo(candidate).Exists) return candidate;
        }

        return null;
    }
}
