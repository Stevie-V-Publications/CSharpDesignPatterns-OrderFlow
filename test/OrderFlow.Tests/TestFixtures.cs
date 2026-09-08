using OrderFlow.Domain;

namespace OrderFlow.Tests;

/// <summary>
/// Small, shared fixture helpers so each pattern's tests can focus on
/// the pattern and not on re-typing a customer and a pizza every time.
/// Nothing here is a pattern — it's the plain data the patterns act on.
/// </summary>
internal static class Sample
{
    internal static Customer Customer(
        NotificationPreference channel = NotificationPreference.Email,
        string? phone = "555-0100") =>
        new()
        {
            Name = "Test Customer",
            Email = "test@example.com",
            Phone = phone,
            PreferredNotification = channel,
        };

    internal static readonly Address Address =
        new("1 Test St", "Denver", "CO", "80202");

    /// <summary>A restaurant with a unique id per call — important for the
    /// Singleton tests, whose one shared service would otherwise leak
    /// queue state between test cases.</summary>
    internal static Restaurant Restaurant(string name = "Test Pizzeria") =>
        new() { Name = name, Address = Address };

    internal static MenuItem MenuItem(string name = "Margherita Pizza", decimal price = 14.00m) =>
        new() { Name = name, BasePrice = price, Category = "Pizza" };

    internal static OrderItem OrderItem(string name = "Margherita Pizza", decimal price = 14.00m) =>
        new() { MenuItem = MenuItem(name, price) };
}
