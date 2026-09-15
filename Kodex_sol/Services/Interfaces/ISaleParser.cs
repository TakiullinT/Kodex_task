using Kodex_task.Models;

namespace Kodex_task.Services.Interfaces;

public interface ISaleParser
{
    Sale? ParseRow(string csvLine);
}