using OrderFlow.Domain;

namespace OrderFlow.Patterns.Decorator;

/// <summary>
/// Pattern 5: Decorator.
///
/// Base class for anything that wraps an <see cref="IOrderItem"/> to
/// add to its price and description without touching
/// <see cref="OrderItem"/> itself. Every concrete add-on
/// (<see cref="ExtraCheese"/>, <see cref="GlutenFreeCrust"/>,
/// <see cref="GiftWrap"/>) derives from this and overrides only what it
/// changes.
///
/// The key move: a decorator both <em>implements</em> IOrderItem and
/// <em>holds</em> an IOrderItem. That's what lets add-ons stack —
/// wrapping a decorator in another decorator is just as valid as
/// wrapping the plain item.
/// </summary>
// tag::decorator-base[]
public abstract class OrderItemDecorator(IOrderItem inner) : IOrderItem
{
    protected IOrderItem Inner { get; } = inner;

    // Default behavior is pass-through. A concrete decorator overrides
    // one or both of these and typically calls the base to fold its
    // own contribution on top of whatever it wraps.
    public virtual decimal GetPrice() => Inner.GetPrice();

    public virtual string GetDescription() => Inner.GetDescription();
}
// end::decorator-base[]
