using Kodex_task.Services;

namespace Kodex.Tests;

public class CsvReaderTests : IDisposable
{
    private readonly string _tempFilePath = Path.GetTempFileName();

    [Fact]
    public async Task ReadSalesAsync_FileNotFound_ThrowsFileNotFoundException()
    {
        var parser = new SaleParser();
        var reader = new CsvReader(parser);

        await Assert.ThrowsAsync<FileNotFoundException>(() => 
            reader.ReadSalesAsync("non_existent_file.csv"));
    }

    [Fact]
    public async Task ReadSalesAsync_FileWithHeaderAndCorruptedRows_FiltersOutBadRows()
    {
        var fileContent = new[]
        {
            "OrderId,OrderDate,CustomerId,ProductCategory,Region,Quantity,UnitPrice,Discount,PaymentMethod,DeliveryDays,CustomerRating,Revenue",
            "1,2022-01-15,1001,Electronics,North,2,500.00,0.1,Credit Card,3,4.8,900.00", 
            "CORRUPTED_LINE_THAT_SHOULD_BE_SKIPPED",                                   
            "2,2022-01-16,1002,Clothing,South,1,50.00,0.0,Cash,5,4.5,50.00"          
        };
        await File.WriteAllLinesAsync(_tempFilePath, fileContent);

        var reader = new CsvReader(new SaleParser());
        var sales = (await reader.ReadSalesAsync(_tempFilePath)).ToList();

        Assert.Equal(2, sales.Count);
        Assert.Equal(1001, sales[0].CustomerId);
        Assert.Equal(1002, sales[1].CustomerId);
    }

    public void Dispose()
    {
        if (File.Exists(_tempFilePath)) File.Delete(_tempFilePath);
    }
}