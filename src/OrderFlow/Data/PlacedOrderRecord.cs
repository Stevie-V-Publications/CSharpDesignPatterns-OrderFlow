namespace OrderFlow.Data;

/// <summary>
/// A flat, persisted snapshot of an order that cleared checkout.
///
/// This is a persistence concern, not a domain type: it lives in
/// <c>/Data</c>, never in <c>/Domain</c>, and no pattern class
/// references it. The rich <c>Order</c> aggregate the Builder assembles
/// stays in memory and drives the patterns; this row is just written
/// afterward so there's a durable order history to show.
/// </summary>
public class PlacedOrderRecord
{
    public Guid Id { get; init; }
    public required string CustomerName { get; init; }
    public required string RestaurantName { get; init; }

    /// <summary>Comma-joined line descriptions, decorators and all.</summary>
    public required string Items { get; init; }

    public decimal Subtotal { get; init; }
    public required string Fulfillment { get; init; }   // Pickup / Delivery
    public required string Status { get; init; }

    /// <summary>UTC. Stored as <see cref="DateTime"/>, not
    /// <see cref="DateTimeOffset"/>, because SQLite can't ORDER BY the latter.</summary>
    public DateTime PlacedAtUtc { get; init; }

    public required string PlacedVia { get; init; }      // customer / admin
}
