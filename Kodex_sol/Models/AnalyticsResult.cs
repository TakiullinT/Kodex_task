namespace Kodex_task.Models;

public record CategorySales(string Category, decimal TotalRevenue);

public record TopProductQuantity(string Category, int TotalQuantity);

public record MonthlyAveragePrice(string MonthYear, decimal AveragePrice);

public record TopCustomerRating(long CustomerId, double AverageRating);

public record MonthlyCategoryDiscount(string MonthYear, string Category, decimal AverageDiscount);

public record AnalyticsResult(
    List<CategorySales> RevenueByCategory,
    List<TopProductQuantity> TopCategoriesByQuantity,
    List<MonthlyAveragePrice> AveragePriceByMonth,
    List<TopCustomerRating> TopCustomersByRating,
    double AverageDeliveryDays,
    List<MonthlyCategoryDiscount> AverageDiscountByMonthAndCategory
);
