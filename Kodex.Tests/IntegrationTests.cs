using Kodex_task.Services;

namespace Kodex.Tests;

public class IntegrationTests : IDisposable
{
    private readonly string _testCsvPath = "integration_test_sales.csv";
    private readonly string _testJsonPath = "integration_test_result.json";

    [Fact]
    public async Task FullPipeline_FromCsvToJson_WorksCorrectly()
    {
        var csvLines = new[]
        {
            "OrderId,OrderDate,CustomerId,ProductCategory,Region,Quantity,UnitPrice,Discount,PaymentMethod,DeliveryDays,CustomerRating,Revenue",
            "1,2022-01-15,1001,Electronics,North,2,500.00,0.1,Credit Card,3,4.8,900.00",
            "2,2022-01-16,1002,Clothing,South,1,50.00,0.0,Cash,5,4.5,50.00"
        };
        await File.WriteAllLinesAsync(_testCsvPath, csvLines);

        var parser = new SaleParser();
        var reader = new CsvReader(parser);
        var analyticsService = new AnalyticService();
        var writer = new ResultWriter();

        var sales = (await reader.ReadSalesAsync(_testCsvPath)).ToList();
        
        Assert.NotEmpty(sales);
        Assert.Equal(2, sales.Count);

        var analyticsResult = analyticsService.CalculateAnalytics(sales, null, null);
        await writer.WriteJsonAsync(analyticsResult, _testJsonPath);

        Assert.True(File.Exists(_testJsonPath), "JSON файл должен был создаться.");
        
        var jsonContent = await File.ReadAllTextAsync(_testJsonPath);
        
        Assert.Contains("Electronics", jsonContent);
        Assert.Contains("Clothing", jsonContent);
    }

    public void Dispose()
    {
        if (File.Exists(_testCsvPath)) File.Delete(_testCsvPath);
        if (File.Exists(_testJsonPath)) File.Delete(_testJsonPath);
    }
}