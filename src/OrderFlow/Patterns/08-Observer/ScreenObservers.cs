using OrderFlow.Domain;

namespace OrderFlow.Patterns.Observer;

/// <summary>
/// Three concrete observers, one per real screen in OrderFlow. Each does
/// something different with the <em>same</em> notification — which is the
/// whole reason the publisher shouldn't hard-code any of them.
///
/// They keep an in-memory <see cref="Log"/> here purely so the isolated
/// Playground demo can show what each screen "did". In the running app
/// the equivalent action is a SignalR push that re-renders a component.
/// </summary>
public abstract class ScreenObserver(string screenName) : IOrderStatusObserver
{
    public string ScreenName => screenName;

    public List<string> Log { get; } = [];

    public void OnOrderStatusChanged(OrderStatusChange change) =>
        Log.Add(Render(change));

    protected abstract string Render(OrderStatusChange change);
}

// tag::screen-observers[]
/// <summary>Cares only about kitchen-relevant transitions.</summary>
public sealed class KitchenDisplayObserver() : ScreenObserver("Kitchen Display")
{
    protected override string Render(OrderStatusChange change) => change.Current switch
    {
        OrderStatus.Confirmed => $"New ticket: order {Short(change.OrderId)} — start prepping",
        OrderStatus.Preparing => $"Order {Short(change.OrderId)} on the line",
        OrderStatus.Ready => $"Order {Short(change.OrderId)} up — call for pickup/driver",
        _ => $"(ignored {change.Current})",
    };

    private static string Short(Guid id) => id.ToString()[..8];
}

/// <summary>The customer's phone: friendly, plain-language updates.</summary>
public sealed class CustomerTrackerObserver() : ScreenObserver("Customer Tracker")
{
    protected override string Render(OrderStatusChange change) => change.Current switch
    {
        OrderStatus.Confirmed => "We got your order! 🎉",
        OrderStatus.Preparing => "The kitchen is making your food.",
        OrderStatus.Ready => "Your order is ready.",
        OrderStatus.OutForDelivery => "Your driver is on the way. 🚗",
        OrderStatus.Completed => "Delivered. Enjoy!",
        _ => $"Status: {change.Current}",
    };
}

/// <summary>Dispatch board: only wakes up when a driver is needed.</summary>
public sealed class DispatchDashboardObserver() : ScreenObserver("Dispatch Dashboard")
{
    protected override string Render(OrderStatusChange change) => change.Current switch
    {
        OrderStatus.Ready => "Assign a driver — order ready for pickup",
        OrderStatus.OutForDelivery => "Driver en route",
        OrderStatus.Completed => "Route closed",
        _ => "(no dispatch action)",
    };
}
// end::screen-observers[]
