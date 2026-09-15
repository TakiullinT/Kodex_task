using Kodex_task.Models;

namespace Kodex_task.Services.Interfaces;

public interface IAnalyticService
{
    AnalyticsResult CalculateAnalytics(IReadOnlyList<Sale> sales, DateTime? startDate, DateTime? endDate);
    AnalyticsResult CalculateAnalyticsParallel(IReadOnlyList<Sale> sales, DateTime? startDate, DateTime? endDate);
}