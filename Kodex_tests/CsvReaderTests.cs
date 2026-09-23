using Kodex_task.Services;

namespace Kodex_tests;

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
    
    [Fact]
    public void ReadSales_EmptyCsvWithOnlyHeaders_ReturnsEmptyListWithoutCrashing()
    {
        var headerOnlyCsv = "OrderId,OrderDate,CustomerId,ProductCategory,Region,Quantity,UnitPrice,Discount,PaymentMethod,DeliveryDays,CustomerRating,Revenue\n";
        File.WriteAllText(_tempFilePath, headerOnlyCsv);
        
        var reader = new CsvReader(new SaleParser());

        var result = reader.ReadSales(_tempFilePath);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void ReadSales_WithInvalidRows_SkipsInvalidAndReturnsValid()
    {
        var csvContent = "OrderId,OrderDate,CustomerId,ProductCategory,Region,Quantity,UnitPrice,Discount,PaymentMethod,DeliveryDays,CustomerRating,Revenue\n" +
                         "1,2024-01-01,101,Laptops,Europe,1,1000,0,Cash,2,5.0,1000\n" +
                         "INVALID_ROW_MISSING_COLUMNS_OR_BAD_DATA\n" +
                         "2,2024-01-02,102,Phones,Asia,2,500,0,Card,1,4.0,1000\n";
        
        File.WriteAllText(_tempFilePath, csvContent);
        var reader = new CsvReader(new SaleParser());

        var result = reader.ReadSales(_tempFilePath);

        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].OrderId);
        Assert.Equal(2, result[1].OrderId);
    }

    public void Dispose()
    {
        if (File.Exists(_tempFilePath)) File.Delete(_tempFilePath);
    }
}