using System.Globalization;
using Kodex_task.Models;
using Kodex_task.Services.Interfaces;

namespace Kodex_task.Services;

public class SaleParser : ISaleParser
{
    public Sale? ParseRow(string csvLine)
    {
        if (string.IsNullOrWhiteSpace(csvLine))
        {
            return null;
        }

        var parts = csvLine.Split(',');
        if (parts.Length < 12)
        {
            return null;
        }

        try
        {
            var culture = CultureInfo.InvariantCulture;

            if (!long.TryParse(parts[0].Trim(), culture, out var orderId)) return null;
            if (!DateTime.TryParse(parts[1].Trim(), culture, out var orderDate)) return null;
            if (!long.TryParse(parts[2].Trim(), culture, out var customerId)) return null;

            var productCategory = parts[3].Trim();
            var region = parts[4].Trim();

            if (!int.TryParse(parts[5].Trim(), culture, out var quantity)) return null;
            if (!decimal.TryParse(parts[6].Trim(), culture, out var unitPrice)) return null;
            if (!decimal.TryParse(parts[7].Trim(), culture, out var discount)) return null;

            var paymentMethod = parts[8].Trim();

            if (!int.TryParse(parts[9].Trim(), culture, out var deliveryDays)) return null;
            if (!double.TryParse(parts[10].Trim(), culture, out var customerRating)) return null;
            if (!decimal.TryParse(parts[11].Trim(), culture, out var revenue)) return null;

            return new Sale(
                orderId, orderDate, customerId, productCategory, region,
                quantity, unitPrice, discount, paymentMethod, deliveryDays,
                customerRating, revenue
            );
        }
        catch
        {
            return null;
        }
    }
}