namespace OrderFlow.Domain;

/// <summary>
/// A single sellable item on a restaurant's menu. This is the plain,
/// un-decorated base item. Pattern 5 (Decorator) wraps instances of
/// order items built from this to add toppings/add-ons at order time
/// without modifying this class. Pattern 7 (Composite) later organizes
/// many of these into a category/menu tree — again without changing
/// this class.
/// </summary>
public class MenuItem
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required decimal BasePrice { get; init; }
    public required string Category { get; init; }
}
