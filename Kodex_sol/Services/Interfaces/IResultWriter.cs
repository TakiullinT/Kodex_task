using Kodex_task.Models;

namespace Kodex_task.Services.Interfaces;

public interface IResultWriter
{
    void WriteConsole(AnalyticsResult result);
    void WriteJson(AnalyticsResult result, string outputPath);
    Task WriteJsonAsync(AnalyticsResult result, string outputPath, CancellationToken cancellationToken = default);
}