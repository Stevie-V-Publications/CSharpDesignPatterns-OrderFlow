using OrderFlow.Domain;

namespace OrderFlow.Patterns.State;

/// <summary>
/// Pattern 9: State.
///
/// An order's lifecycle is a strict progression -- Placed, Confirmed,
/// Preparing, Ready, OutForDelivery, Completed -- with a couple of
/// branches (you can cancel while it's Placed or Confirmed, not once
/// the kitchen has started). Each state allows different actions and
/// says different things to the customer.
///
/// Instead of a growing <c>switch (order.Status)</c> in every method
/// that touches an order, each state is its own class that knows only
/// its own rules and which state comes next.
/// </summary>
// tag::state-contract[]
public interface IOrderState
{
    /// <summary>The plain enum label this state corresponds to.</summary>
    OrderStatus Status { get; }

    /// <summary>Per-state, customer-facing line (State's "behavior varies by state").</summary>
    string CustomerMessage { get; }

    bool CanCancel { get; }

    /// <summary>Move to the next state in the lifecycle, or throw if this state is terminal.</summary>
    IOrderState Advance();

    /// <summary>Cancel the order, or throw if it's too late to cancel.</summary>
    IOrderState Cancel();
}
// end::state-contract[]

/// <summary>
/// Base class: every transition is illegal until a concrete state says
/// otherwise. Subclasses override only the moves they permit, so an
/// unhandled transition fails loudly instead of silently doing nothing.
/// </summary>
// tag::state-base[]
public abstract class OrderState : IOrderState
{
    public abstract OrderStatus Status { get; }
    public abstract string CustomerMessage { get; }
    public virtual bool CanCancel => false;

    public virtual IOrderState Advance() =>
        throw new InvalidOperationException($"An order that is {Status} can't be advanced further.");

    public virtual IOrderState Cancel() =>
        throw new InvalidOperationException($"An order that is {Status} can no longer be cancelled.");
}
// end::state-base[]
