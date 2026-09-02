using OrderFlow.Domain;

namespace OrderFlow.Services;

/// <summary>
/// One of the subsystems the checkout Facade (Pattern 6) coordinates.
/// Not a pattern itself — just a plain "can the kitchen actually make
/// this right now?" check. In a real app it would hit stock levels;
/// here it's an in-memory set of "86'd" (sold-out) item names the
/// Pattern Playground can toggle.
/// </summary>
public interface IInventoryService
{
    InventoryCheck CheckAvailability(Restaurant restaurant, IEnumerable<IOrderItem> items);
}

public record InventoryCheck(bool AllAvailable, IReadOnlyList<string> Unavailable)
{
    public static readonly InventoryCheck Ok = new(true, []);
}

public sealed class InMemoryInventoryService : IInventoryService
{
    /// <summary>Item names currently sold out. Mutable so the demo can poke it.</summary>
    public ISet<string> SoldOut { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public InventoryCheck CheckAvailability(Restaurant restaurant, IEnumerable<IOrderItem> items)
    {
        var unavailable = items
            .Select(i => i.GetDescription())
            .Where(desc => SoldOut.Any(name => desc.Contains(name, StringComparison.OrdinalIgnoreCase)))
            .Distinct()
            .ToList();

        return unavailable.Count == 0 ? InventoryCheck.Ok : new InventoryCheck(false, unavailable);
    }
}
