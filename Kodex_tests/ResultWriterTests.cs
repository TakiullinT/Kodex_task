using Kodex_task.Models;
using Kodex_task.Services;

namespace Kodex_tests;

public class ResultWriterTests : IDisposable
{
    private readonly string _tempOutputJson = Path.Combine(Path.GetTempPath(), "test_output.json");

    [Fact]
    public async Task WriteJsonAsync_ValidAnalyticsResult_CreatesFileAndWritesValidJson()
    {
        var writer = new ResultWriter();
        var dummyAnalytics = new AnalyticsResult(
            RevenueByCategory: new() { new("Electronics", 1500m) },
            TopCategoriesByQuantity: new() { new("Electronics", 10) },
            AveragePriceByMonth: new() { new("2022-01", 150m) },
            TopCustomersByRating: new() { new(1001, 4.9) },
            AverageDeliveryDays: 4.2,
            AverageDiscountByMonthAndCategory: new() { new("2022-01", "Electronics", (decimal)0.1) }
        );

        await writer.WriteJsonAsync(dummyAnalytics, _tempOutputJson);

        Assert.True(File.Exists(_tempOutputJson));
        var content = await File.ReadAllTextAsync(_tempOutputJson);
        Assert.Contains("Electronics", content);
        Assert.Contains("1500", content);
    }

    public void Dispose()
    {
        if (File.Exists(_tempOutputJson)) File.Delete(_tempOutputJson);
    }
}