using Kodex_task.Models;
using Kodex_task.Services;

namespace Kodex_tests;

public class AnalyticsTests
{
    private readonly AnalyticService _service = new();

    private readonly List<Sale> _sampleSales = new()
    {
        new(1, new DateTime(2022, 1, 1), 101, "Beauty", "South", 2, 100m, 0.1m, "Card", 3, 4.5, 180m),
        new(2, new DateTime(2022, 1, 15), 102, "Clothing", "North", 5, 50m, 0.2m, "Wallet", 5, 3.5, 200m),
        new(3, new DateTime(2022, 2, 1), 101, "Beauty", "East", 1, 100m, 0.0m, "COD", 2, 5.0, 100m),
    };

    [Fact]
    public void CalculateAnalytics_ValidData_CalculatesCorrectTotals()
    {
        var result = _service.CalculateAnalytics(_sampleSales, null, null);

        Assert.Equal(2, result.RevenueByCategory.Count);
        Assert.Equal(280m, result.RevenueByCategory.First(c => c.Category == "Beauty").TotalRevenue);
        Assert.Equal(200m, result.RevenueByCategory.First(c => c.Category == "Clothing").TotalRevenue);
        Assert.Equal(3.33, result.AverageDeliveryDays);
    }

    [Fact]
    public void CalculateAnalytics_EmptyList_ReturnsEmptyAnalyticsWithoutCrashing()
    {
        var emptySales = new List<Sale>();

        var result = _service.CalculateAnalytics(emptySales, null, null);

        Assert.NotNull(result);
        Assert.Empty(result.RevenueByCategory);
        Assert.Empty(result.TopCategoriesByQuantity);
        Assert.Equal(0, result.AverageDeliveryDays);
    }

    [Fact]
    public void CalculateAnalytics_WithDateFilter_FiltersCorrectly()
    {
        var startDate = new DateTime(2022, 1, 1);
        var endDate = new DateTime(2022, 1, 31);

        var result = _service.CalculateAnalytics(_sampleSales, startDate, endDate);

        Assert.Equal(2, result.RevenueByCategory.Count);
        Assert.Single(result.RevenueByCategory, c => c.Category == "Beauty");
    }

    [Fact]
    public void CalculateAnalytics_DateFilterOutOfRange_ReturnsEmptyCollections()
    {
        var startDate = new DateTime(2025, 1, 1);
        var endDate = new DateTime(2025, 12, 31);

        var result = _service.CalculateAnalytics(_sampleSales, startDate, endDate);

        Assert.Empty(result.RevenueByCategory);
        Assert.Equal(0, result.AverageDeliveryDays);
    }

    [Fact]
    public void CalculateAnalytics_TopCustomersByRating_OrdersCorrectly()
    {
        var result = _service.CalculateAnalytics(_sampleSales, null, null);
        
        Assert.NotEmpty(result.TopCustomersByRating);
        Assert.Equal(101, result.TopCustomersByRating.First().CustomerId);
        Assert.Equal(4.75, result.TopCustomersByRating.First().AverageRating);
    }

    [Fact]
    public void CalculateAnalyticsParallel_MatchesSequentialResult()
    {
        var syncResult = _service.CalculateAnalytics(_sampleSales, null, null);
        var parallelResult = _service.CalculateAnalyticsParallel(_sampleSales, null, null);

        Assert.Equal(syncResult.AverageDeliveryDays, parallelResult.AverageDeliveryDays);
        Assert.Equal(syncResult.RevenueByCategory.Count, parallelResult.RevenueByCategory.Count);
    }
}