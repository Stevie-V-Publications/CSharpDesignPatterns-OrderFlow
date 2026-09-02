using OrderFlow.Domain;

namespace OrderFlow.Patterns.State;

/// <summary>
/// The context in the State pattern: holds the current
/// <see cref="IOrderState"/> and delegates every lifecycle action to it.
/// Callers ask the machine to <c>Advance()</c> or <c>Cancel()</c>; the
/// machine never decides what's legal — the current state object does,
/// and hands back the state to become.
///
/// It also keeps the order's plain <see cref="OrderStatus"/> label in
/// sync, so the rest of the app can still read <c>order.Status</c>
/// without knowing the State pattern exists.
/// </summary>
// tag::state-machine[]
public sealed class OrderStateMachine
{
    private readonly Order _order;

    public IOrderState Current { get; private set; }

    public OrderStateMachine(Order order)
    {
        _order = order;
        Current = ForStatus(order.Status);
    }

    public void Advance() => TransitionTo(Current.Advance());

    public void Cancel() => TransitionTo(Current.Cancel());

    private void TransitionTo(IOrderState next)
    {
        Current = next;
        _order.Status = next.Status; // keep the plain label in lockstep
    }

    /// <summary>Rehydrate the right state object from a stored enum value.</summary>
    public static IOrderState ForStatus(OrderStatus status) => status switch
    {
        OrderStatus.Placed => new PlacedState(),
        OrderStatus.Confirmed => new ConfirmedState(),
        OrderStatus.Preparing => new PreparingState(),
        OrderStatus.Ready => new ReadyState(),
        OrderStatus.OutForDelivery => new OutForDeliveryState(),
        OrderStatus.Completed => new CompletedState(),
        OrderStatus.Cancelled => new CancelledState(),
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Unknown order status."),
    };
}
// end::state-machine[]
