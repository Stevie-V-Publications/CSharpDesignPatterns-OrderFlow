using OrderFlow.Domain;

namespace OrderFlow.Patterns.State;

// tag::concrete-states[]
/// <summary>Just placed. Not yet accepted by the restaurant; freely cancellable.</summary>
public sealed class PlacedState : OrderState
{
    public override OrderStatus Status => OrderStatus.Placed;
    public override string CustomerMessage => "Order placed — waiting for the restaurant to confirm.";
    public override bool CanCancel => true;

    public override IOrderState Advance() => new ConfirmedState();
    public override IOrderState Cancel() => new CancelledState();
}

/// <summary>Restaurant accepted it. Still cancellable — the kitchen hasn't started.</summary>
public sealed class ConfirmedState : OrderState
{
    public override OrderStatus Status => OrderStatus.Confirmed;
    public override string CustomerMessage => "Confirmed! The restaurant has your order.";
    public override bool CanCancel => true;

    public override IOrderState Advance() => new PreparingState();
    public override IOrderState Cancel() => new CancelledState();
}

/// <summary>On the line. Too late to cancel — ingredients are already committed.</summary>
public sealed class PreparingState : OrderState
{
    public override OrderStatus Status => OrderStatus.Preparing;
    public override string CustomerMessage => "The kitchen is preparing your food.";

    public override IOrderState Advance() => new ReadyState();
}

/// <summary>Food's up. Next stop is a driver (or the counter).</summary>
public sealed class ReadyState : OrderState
{
    public override OrderStatus Status => OrderStatus.Ready;
    public override string CustomerMessage => "Your order is ready.";

    public override IOrderState Advance() => new OutForDeliveryState();
}

public sealed class OutForDeliveryState : OrderState
{
    public override OrderStatus Status => OrderStatus.OutForDelivery;
    public override string CustomerMessage => "Your driver is on the way.";

    public override IOrderState Advance() => new CompletedState();
}

/// <summary>Terminal. Both Advance and Cancel inherit the base "no" from OrderState.</summary>
public sealed class CompletedState : OrderState
{
    public override OrderStatus Status => OrderStatus.Completed;
    public override string CustomerMessage => "Delivered. Enjoy!";
}

/// <summary>Terminal.</summary>
public sealed class CancelledState : OrderState
{
    public override OrderStatus Status => OrderStatus.Cancelled;
    public override string CustomerMessage => "This order was cancelled.";
}
// end::concrete-states[]
