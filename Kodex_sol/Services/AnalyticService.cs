using System.Collections.Concurrent;
using Kodex_task.Models;
using Kodex_task.Services.Interfaces;

namespace Kodex_task.Services;

public class AnalyticService : IAnalyticService
{
    public AnalyticsResult CalculateAnalytics(IReadOnlyList<Sale> sales, DateTime? startDate, DateTime? endDate)
    {
        var filteredSales = FilterSales(sales,startDate, endDate).ToList();
        
        var revenueByCategory = filteredSales
            .GroupBy(s => s.ProductCategory)
            .Select(g => new CategorySales(g.Key, g.Sum(s => s.Revenue)))
            .OrderByDescending(c => c.TotalRevenue)
            .ToList();
        
        var topCategories = filteredSales
            .GroupBy(s => s.ProductCategory)
            .Select(g => new TopProductQuantity(g.Key, g.Sum(s => s.Quantity)))
            .OrderByDescending(g => g.TotalQuantity)
            .Take(5)
            .ToList();

        var avgPriceByMonth = filteredSales
            .GroupBy(s => new { s.OrderDate.Year, s.OrderDate.Month })
            .Select(g => new MonthlyAveragePrice(
                $"{g.Key.Year}-{g.Key.Month:D2}",
                Math.Round(g.Average(s => s.UnitPrice), 2)))
            .OrderBy(m => m.MonthYear)
            .ToList();

        var topCustomers = filteredSales
            .GroupBy(s => s.CustomerId)
            .Select(g => new TopCustomerRating(g.Key, Math.Round(g.Average(s => s.CustomerRating), 2)))
            .OrderByDescending(c => c.AverageRating)
            .Take(5)
            .ToList();

        var avgDelivery = filteredSales.Count > 0
            ? Math.Round(filteredSales.Average(s => s.DeliveryDays), 2)
            : 0;

        var avgDiscount = filteredSales
            .GroupBy(s => new { s.OrderDate.Year, s.OrderDate.Month, s.ProductCategory })
            .Select(g => new MonthlyCategoryDiscount(
                $"{g.Key.Year}-{g.Key.Month:D2}",
                g.Key.ProductCategory,
                Math.Round(g.Average(s => s.Discount), 4)))
            .OrderBy(d => d.MonthYear)
            .ThenBy(d => d.Category)
            .ToList();

        return new AnalyticsResult(
            revenueByCategory, topCategories, avgPriceByMonth,
            topCustomers, avgDelivery, avgDiscount);
    }

    public AnalyticsResult CalculateAnalyticsParallel(IReadOnlyList<Sale> sales, DateTime? startDate, DateTime? endDate)
    {
        var filteredSales = FilterSales(sales, startDate, endDate).ToList();

        var categoryTotals = new ConcurrentDictionary<string, decimal>();

        Parallel.ForEach(filteredSales, sale =>
        {
            categoryTotals.AddOrUpdate(
                sale.ProductCategory,
                sale.Revenue,
                (_, currentVal) => currentVal + sale.Revenue);
        });

        var revenueByCategory = categoryTotals
            .Select(kvp => new CategorySales(kvp.Key, kvp.Value))
            .OrderByDescending(c => c.TotalRevenue)
            .ToList();

        var topCategories = filteredSales.AsParallel()
            .GroupBy(s => s.ProductCategory)
            .Select(g => new TopProductQuantity(g.Key, g.Sum(s => s.Quantity)))
            .OrderByDescending(g => g.TotalQuantity)
            .Take(5)
            .ToList();

        var avgPriceByMonth = filteredSales.AsParallel()
            .GroupBy(s => new { s.OrderDate.Year, s.OrderDate.Month })
            .Select(g => new MonthlyAveragePrice(
                $"{g.Key.Year}-{g.Key.Month:D2}",
                Math.Round(g.Average(s => s.UnitPrice), 2)))
            .OrderBy(m => m.MonthYear)
            .ToList();

        var topCustomers = filteredSales.AsParallel()
            .GroupBy(s => s.CustomerId)
            .Select(g => new TopCustomerRating(g.Key, Math.Round(g.Average(s => s.CustomerRating), 2)))
            .OrderByDescending(c => c.AverageRating)
            .Take(5)
            .ToList();

        var avgDelivery = filteredSales.Count > 0
            ? Math.Round(filteredSales.AsParallel().Average(s => s.DeliveryDays), 2)
            : 0;

        var avgDiscount = filteredSales.AsParallel()
            .GroupBy(s => new { s.OrderDate.Year, s.OrderDate.Month, s.ProductCategory })
            .Select(g => new MonthlyCategoryDiscount(
                $"{g.Key.Year}-{g.Key.Month:D2}",
                g.Key.ProductCategory,
                Math.Round(g.Average(s => s.Discount), 4)))
            .OrderBy(d => d.MonthYear)
            .ThenBy(d => d.Category)
            .ToList();

        return new AnalyticsResult(
            revenueByCategory, topCategories, avgPriceByMonth,
            topCustomers, avgDelivery, avgDiscount);
    }
    
    private static IEnumerable<Sale> FilterSales(IEnumerable<Sale> sales, DateTime? startDate, DateTime? endDate)
    {
        var result = sales;
        if (startDate.HasValue)
            result = result.Where(s => s.OrderDate >= startDate.Value);
        if (endDate.HasValue)
            result = result.Where(s => s.OrderDate < endDate.Value);
        return result;
    }
}