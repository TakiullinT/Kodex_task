using Kodex_task.Services;

namespace Kodex_tests;

public class ParserTests
{
    private readonly SaleParser _parser = new();

    [Fact]
    public void ParseRow_ValidCsvRow_ReturnsSaleObject()
    {
        var csvLine = "10001,2022-01-01,1102,Beauty,South,7,373.65,0.28,Wallet,10,4.7,1883.2";

        var result = _parser.ParseRow(csvLine);

        Assert.NotNull(result);
        Assert.Equal(10001, result.OrderId);
        Assert.Equal("Beauty", result.ProductCategory);
        Assert.Equal(7, result.Quantity);
        Assert.Equal(373.65m, result.UnitPrice);
    }

    [Fact]
    public void ParseRow_RowWithExcessiveSpaces_TrimsAndParsesCorrectly()
    {
        var csvLine = " 10001 , 2022-01-01 , 1102 , Beauty , South , 7 , 373.65 , 0.28 , Wallet , 10 , 4.7 , 1883.2 ";

        var result = _parser.ParseRow(csvLine);

        Assert.NotNull(result);
        Assert.Equal(10001, result.OrderId);
        Assert.Equal("Beauty", result.ProductCategory);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("invalid,csv,data")]
    [InlineData("10001,invalid_date,1102,Beauty,South,7,373.65,0.28,Wallet,10,4.7,1883.2")]
    [InlineData("10001,2022-01-01,1102,Beauty,South,NOT_NUM,373.65,0.28,Wallet,10,4.7,1883.2")]
    [InlineData("10001,2022-01-01,1102,Beauty,South,7,373.65,0.28,Wallet,10")] 
    public void ParseRow_InvalidOrMalformedRow_ReturnsNull(string invalidLine)
    {
        var result = _parser.ParseRow(invalidLine);
        Assert.Null(result);
    }
    
    [Fact]
    public void ParseRow_WithEscapedCommas_ParsesCorrectlyAndStripsQuotes()
    {
        var parser = new SaleParser();
        var csvLine = "1,2024-01-01,123,\"Phones, Mobile\",\"North, America\",2,1500.00,0.1,Card,3,4.8,2700.00";

        var result = parser.ParseRow(csvLine);

        Assert.NotNull(result);
        Assert.Equal("Phones, Mobile", result.ProductCategory);
        Assert.Equal("North, America", result.Region);
        Assert.Equal(1500.00m, result.UnitPrice);
    }
    
    [Fact]
    public void ParseRow_InvalidRowMissingColumns_ReturnsNull()
    {
        var parser = new SaleParser();
        var csvLine = "1,2024-01-01,123,\"Phones, Mobile\""; 

        var result = parser.ParseRow(csvLine);

        Assert.Null(result);
    }
}