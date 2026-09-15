using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Kodex_task.Models;
using Kodex_task.Services.Interfaces;

namespace Kodex_task.Services;

public class ResultWriter : IResultWriter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };
    
    public void WriteConsole(AnalyticsResult result)
    {
        Console.WriteLine("=== Общая сумма продаж по категориям ===");
        foreach (var cat in result.RevenueByCategory)
        {
            Console.WriteLine($"  {cat.Category}: {cat.TotalRevenue:N2}");
        }

        Console.WriteLine("\n=== Топ-5 категорий по количеству проданных единиц ===");
        foreach (var p in result.TopCategoriesByQuantity)
        {
            Console.WriteLine($"  {p.Category}: {p.TotalQuantity} шт.");
        }

        Console.WriteLine("\n=== Средняя цена за месяц ===");
        foreach (var m in result.AveragePriceByMonth)
        {
            Console.WriteLine($"  {m.MonthYear}: {m.AveragePrice:N2}");
        }

        Console.WriteLine("\n=== Топ-5 покупателей по рейтингу ===");
        foreach (var c in result.TopCustomersByRating)
        {
            Console.WriteLine($"  ID {c.CustomerId}: {c.AverageRating:F2}");
        }

        Console.WriteLine($"\n=== Среднее время доставки: {result.AverageDeliveryDays:F2} дней ===");

        Console.WriteLine("\n=== Средняя скидка по категориям (по месяца) ===");
        foreach (var d in result.AverageDiscountByMonthAndCategory)
        {
            Console.WriteLine($"  [{d.MonthYear}] {d.Category}: {d.AverageDiscount * 100:F2}%");
        }
    }

    public void WriteJson(AnalyticsResult result, string outputPath)
    {
        ValidateOutputDirectory(outputPath);
        var json = JsonSerializer.Serialize(result, JsonOptions);
        File.WriteAllText(outputPath, json);
        Console.WriteLine($"Результаты успешно сохранены в файл: {outputPath}");
    }

    public async Task WriteJsonAsync(AnalyticsResult result, string outputPath, CancellationToken cancellationToken = default)
    {
        ValidateOutputDirectory(outputPath);
        await using var stream = File.Create(outputPath);
        await JsonSerializer.SerializeAsync(stream, result, JsonOptions, cancellationToken);
        Console.WriteLine($"Результаты асинхронно сохранены в файл: {outputPath}");
    }
    
    private static void ValidateOutputDirectory(string outputPath)
    {
        var dir = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }
}