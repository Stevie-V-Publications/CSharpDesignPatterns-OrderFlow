using OrderFlow.Domain;

namespace OrderFlow.Patterns.Observer;

/// <summary>
/// The payload every observer receives when an order moves through its
/// lifecycle. Immutable on purpose — the publisher hands the same record
/// to every subscriber, and none of them should be able to mutate what
/// the next one sees.
/// </summary>
public record OrderStatusChange(
    Guid OrderId,
    OrderStatus Previous,
    OrderStatus Current,
    DateTimeOffset ChangedAt)
{
    public static OrderStatusChange Between(Guid orderId, OrderStatus previous, OrderStatus current) =>
        new(orderId, previous, current, DateTimeOffset.Now);
}
