using Kodex_task.Models;

namespace Kodex_task.Services.Interfaces;

public interface ICsvReader
{
    List<Sale> ReadSales(string filePath);
    Task<List<Sale>> ReadSalesAsync(string filePath, CancellationToken cancellationToken = default);
}