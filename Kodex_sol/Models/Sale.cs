namespace Kodex_task.Models;

public record Sale(
    long OrderId,
    DateTime OrderDate,
    long CustomerId,
    string ProductCategory,
    string Region,
    int Quantity,
    decimal UnitPrice,
    decimal Discount,
    string PaymentMethod,
    int DeliveryDays,
    double CustomerRating,
    decimal Revenue
);