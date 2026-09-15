using Kodex_task.Models;
using Kodex_task.Services;
using Kodex_task.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Kodex.Tests;

public class DependencyInjectionTests
{
    [Fact]
    public void ServiceCollection_ResolvesAllDependenciesCorrectly()
    {
        var services = new ServiceCollection();
        services.AddTransient<ISaleParser, SaleParser>();
        services.AddSingleton<ICsvReader, CsvReader>();
        services.AddSingleton<IAnalyticService, AnalyticService>();
        services.AddSingleton<IResultWriter, ResultWriter>();

        var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetService<ISaleParser>());
        Assert.NotNull(provider.GetService<ICsvReader>());
        Assert.NotNull(provider.GetService<IAnalyticService>());
        Assert.NotNull(provider.GetService<IResultWriter>());
    }

    [Fact]
    public async Task CsvReaderAsync_MockedParser_ReturnsExpectedSales()
    {
        var mockParser = new Mock<ISaleParser>();
        mockParser.Setup(p => p.ParseRow(It.IsAny<string>()))
            .Returns(new Sale(1, DateTime.Now, 100, "Beauty", "South", 1, 10m, 0m, "Card", 2, 5.0, 10m));

        var reader = new CsvReader(mockParser.Object);

        var tempFile = Path.GetTempFileName();
        await File.WriteAllLinesAsync(tempFile, new[] { "Header", "1,2022-01-01,100,Beauty,South,1,10,0,Card,2,5,10" });

        try
        {
            var result = await reader.ReadSalesAsync(tempFile);
            Assert.Single(result);
            mockParser.Verify(p => p.ParseRow(It.IsAny<string>()), Times.Once);
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }
}