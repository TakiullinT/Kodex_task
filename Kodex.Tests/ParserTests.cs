using Kodex_task.Services;

namespace Kodex.Tests;

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
}