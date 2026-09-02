using OrderFlow.Domain;

namespace OrderFlow.Patterns.Singleton;

/// <summary>
/// One line on the kitchen display: an order that's been sent to the
/// kitchen and is waiting to be made. The ticket carries a status
/// <em>label</em>, but the rules for moving between statuses are still
/// Pattern 9's job (State) — the kitchen display asks a state object for
/// the next status and just stores the result here.
/// </summary>
public record KitchenTicket(Guid OrderId, string Summary, DateTimeOffset ReceivedAt)
{
    /// <summary>Mutable on purpose: the kitchen advances this in place as
    /// the line cook works the ticket. It lands on the queue already
    /// <see cref="OrderStatus.Confirmed"/> (payment cleared upstream).</summary>
    public OrderStatus Status { get; set; } = OrderStatus.Confirmed;

    public static KitchenTicket For(Guid orderId, string summary) =>
        new(orderId, summary, DateTimeOffset.UtcNow);
}
