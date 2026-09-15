using Kodex_task.Models;
using Kodex_task.Services;
using Kodex_task.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


var options = CommandLineOptions.Parse(args);

if (options.ShowHelp || string.IsNullOrWhiteSpace(options.InputFilePath))
{
    PrintHelp();
    return 0;
}

try
{
    switch (options.Mode)
    {
        case "console":
            RunConsoleMode(options);
            break;
        case "file":
            RunFileMode(options);
            break;
        case "async":
            await RunAsyncModeAsync(options);
            break;
        case "parallel":
            RunParallelMode(options);
            break;
        case "di":
        case "full":
            await RunHostModeAsync(options);
            break;
        default:
            Console.WriteLine($"Ошибка: неизвестный режим '{options.Mode}'");
            PrintHelp();
            return 1;
    }
    return 0;
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Ошибка выполнения: {ex.Message}");
    Console.ResetColor();
    return 1;
}


void RunConsoleMode(CommandLineOptions opts)
{
    var parser = new SaleParser();
    var reader = new CsvReader(parser);
    var analytics = new AnalyticService();
    var writer = new ResultWriter();

    var sales = reader.ReadSales(opts.InputFilePath);
    var result = analytics.CalculateAnalytics(sales, opts.StartDate, opts.EndDate);
    writer.WriteConsole(result);
}

void RunFileMode(CommandLineOptions opts)
{
    if (string.IsNullOrWhiteSpace(opts.OutputFilePath))
        throw new ArgumentException("Для режима 'file' необходимо указать параметр --output");

    var parser = new SaleParser();
    var reader = new CsvReader(parser);
    var analytics = new AnalyticService();
    var writer = new ResultWriter();

    var sales = reader.ReadSales(opts.InputFilePath);
    var result = analytics.CalculateAnalytics(sales, opts.StartDate, opts.EndDate);
    writer.WriteJson(result, opts.OutputFilePath);
}

async Task RunAsyncModeAsync(CommandLineOptions opts)
{
    var parser = new SaleParser();
    var reader = new CsvReader(parser);
    var analytics = new AnalyticService();
    var writer = new ResultWriter();

    var sales = await reader.ReadSalesAsync(opts.InputFilePath);
    var result = await Task.Run(() => analytics.CalculateAnalytics(sales, opts.StartDate, opts.EndDate));

    if (!string.IsNullOrWhiteSpace(opts.OutputFilePath))
    {
        await writer.WriteJsonAsync(result, opts.OutputFilePath);
    }
    else
    {
        writer.WriteConsole(result);
    }
}

void RunParallelMode(CommandLineOptions opts)
{
    var parser = new SaleParser();
    var reader = new CsvReader(parser);
    var analytics = new AnalyticService();
    var writer = new ResultWriter();

    var sales = reader.ReadSales(opts.InputFilePath);
    var result = analytics.CalculateAnalyticsParallel(sales, opts.StartDate, opts.EndDate);

    if (!string.IsNullOrWhiteSpace(opts.OutputFilePath))
    {
        writer.WriteJson(result, opts.OutputFilePath);
    }
    else
    {
        writer.WriteConsole(result);
    }
}

async Task RunHostModeAsync(CommandLineOptions opts)
{
    var host = Host.CreateDefaultBuilder()
        .ConfigureServices((_, services) =>
        {
            services.AddTransient<ISaleParser, SaleParser>();

            services.AddSingleton<ICsvReader, CsvReader>();
            services.AddSingleton<IAnalyticService, AnalyticService>();
            services.AddSingleton<IResultWriter, ResultWriter>();
        })
        .Build();

    var reader = host.Services.GetRequiredService<ICsvReader>();
    var analytics = host.Services.GetRequiredService<IAnalyticService>();
    var writer = host.Services.GetRequiredService<IResultWriter>();

    List<Sale> sales;
    AnalyticsResult result;

    if (opts.Mode == "full")
    {
        sales = await reader.ReadSalesAsync(opts.InputFilePath);
        result = await Task.Run(() => analytics.CalculateAnalyticsParallel(sales, opts.StartDate, opts.EndDate));

        if (!string.IsNullOrWhiteSpace(opts.OutputFilePath))
            await writer.WriteJsonAsync(result, opts.OutputFilePath);
        else
            writer.WriteConsole(result);
    }
    else // di mode
    {
        sales = reader.ReadSales(opts.InputFilePath);
        result = analytics.CalculateAnalytics(sales, opts.StartDate, opts.EndDate);

        if (!string.IsNullOrWhiteSpace(opts.OutputFilePath))
            writer.WriteJson(result, opts.OutputFilePath);
        else
            writer.WriteConsole(result);
    }
}

void PrintHelp()
{
    Console.WriteLine("""
    Sales Analytics CLI (.NET 10)
    
    Использование:
      dotnet run --mode=<MODE> --input=<PATH> [опции]

    Режимы (--mode / -m):
      console   Синхронная обработка и вывод в консоль (по умолчанию)
      file      Сохранение результата в JSON (требуется --output)
      async     Асинхронный ввод/вывод (ReadLineAsync, WriteJsonAsync)
      parallel  Параллельная вычисление аналитики (Parallel.ForEach / PLINQ)
      di        Запуск с использованием Microsoft.Extensions.DependencyInjection
      full      Комбинированный режим: Async I/O + Parallel + DI

    Параметры:
      --input       (Обязательный) Путь к входному CSV-файлу
      --output      Путь к выходному JSON-файлу
      --start_date  Начальная дата фильтрации (включительно, напр. 2022-01-01)
      --end_date    Конечная дата фильтрации (исключительно, напр. 2022-02-01)
      --help, -h    Справка
    """);
}