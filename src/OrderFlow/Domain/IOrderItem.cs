namespace OrderFlow.Domain;

/// <summary>
/// Contract for anything that can appear as a line item on an order.
/// This interface is the seam Pattern 5 (Decorator) hooks into:
/// toppings and add-ons are implemented as decorators that wrap an
/// IOrderItem and adjust <see cref="GetPrice"/> / <see cref="GetDescription"/>
/// without modifying <see cref="OrderItem"/> itself.
/// </summary>
// tag::order-item-contract[]
public interface IOrderItem
{
    decimal GetPrice();
    string GetDescription();
}
// end::order-item-contract[]
