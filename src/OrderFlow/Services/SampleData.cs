using OrderFlow.Domain;

namespace OrderFlow.Services;

/// <summary>
/// Fixture data for the Pattern Playground demos. Not a pattern itself
/// — just enough sample customers/restaurants/menu items so each
/// pattern demo has something realistic to work with without requiring
/// the database to be seeded first.
/// </summary>
public static class SampleData
{
    public static readonly Customer Customer = new()
    {
        Name = "Jamie Rivera",
        Email = "jamie@example.com",
        Phone = "555-0142",
        DefaultDeliveryAddress = new Address("742 Evergreen Terrace", "Denver", "CO", "80203"),
        PreferredNotification = NotificationPreference.Sms
    };

    public static readonly Restaurant Restaurant = new()
    {
        Name = "Mountain Pie Pizza Co.",
        Address = new Address("100 Larimer St", "Denver", "CO", "80202")
    };

    public static readonly IReadOnlyList<MenuItem> MenuItems =
    [
        new MenuItem { Name = "Margherita Pizza", Description = "San Marzano tomato, fresh mozzarella, basil", BasePrice = 14.00m, Category = "Pizza" },
        new MenuItem { Name = "Pepperoni Pizza", Description = "Classic pepperoni, mozzarella, tomato sauce", BasePrice = 15.50m, Category = "Pizza" },
        new MenuItem { Name = "Caesar Salad", Description = "Romaine, parmesan, croutons, Caesar dressing", BasePrice = 9.00m, Category = "Salads" },
        new MenuItem { Name = "Garlic Knots", Description = "Six knots, garlic butter, parmesan", BasePrice = 6.50m, Category = "Sides" },
    ];
}
