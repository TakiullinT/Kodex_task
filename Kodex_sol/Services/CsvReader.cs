using Kodex_task.Models;
using Kodex_task.Services.Interfaces;

namespace Kodex_task.Services;

public class CsvReader : ICsvReader
{
    private readonly ISaleParser _saleParser;

    public CsvReader(ISaleParser saleParser)
    {
        _saleParser = saleParser;
    }
    
    public List<Sale> ReadSales(string filePath)
    {
        ValidateFilePath(filePath);
        
        var sales = new List<Sale>();
        int skippedCount = 0;
        using var reader = new StreamReader(filePath);
        
        string? line = reader.ReadLine();
        while ((line = reader.ReadLine()) != null)
        {
            var sale = _saleParser.ParseRow(line);
            if (sale != null) 
                sales.Add(sale);
            else 
                skippedCount++;
        }

        if (skippedCount > 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[WARNING] Пропущено некорректных строк (данные повреждены): {skippedCount}");
            Console.ResetColor();
        }
        return sales;
    }

    public async Task<List<Sale>> ReadSalesAsync(string filePath, CancellationToken cancellationToken = default)
    {
        ValidateFilePath(filePath);
        
        var sales = new List<Sale>();
        int skippedCount = 0;
        using var reader = new StreamReader(filePath);
        
        await reader.ReadLineAsync(cancellationToken);
        string? line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            var sale = _saleParser.ParseRow(line);
            if (sale != null) 
                sales.Add(sale);
            else 
                skippedCount++;
        }
        
        if (skippedCount > 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[WARNING] Пропущено некорректных строк (данные повреждены): {skippedCount}");
            Console.ResetColor();
        }
        
        return sales;
    }
    
    private static void ValidateFilePath(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
        {
            throw new FileNotFoundException($"Файл не найден по пути: '{filePath}'");
        }
    }
}