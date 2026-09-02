namespace OrderFlow.Domain;

/// <summary>
/// The plain, un-decorated line item: one menu item, a quantity, and
/// optional free-text instructions. Toppings/add-ons are NOT modeled
/// as fields here — see Pattern 5 (Decorator), which wraps instances
/// of this class rather than growing this class with an ever-expanding
/// list of optional topping flags.
/// </summary>
public class OrderItem : IOrderItem
{
    public required MenuItem MenuItem { get; init; }
    public int Quantity { get; init; } = 1;
    public string? SpecialInstructions { get; init; }

    public decimal GetPrice() => MenuItem.BasePrice * Quantity;

    public string GetDescription() =>
        Quantity > 1 ? $"{Quantity}x {MenuItem.Name}" : MenuItem.Name;
}
