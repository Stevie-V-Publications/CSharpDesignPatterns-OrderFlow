using OrderFlow.Domain;

namespace OrderFlow.Patterns.Builder;

/// <summary>
/// Pattern 1: Builder.
///
/// The Problem: an Order has several required pieces (customer,
/// restaurant, at least one item) and several optional/conditional
/// ones (delivery address — required only if OrderType is Delivery;
/// special instructions; more items added one at a time as a customer
/// browses a menu). A single constructor covering every combination
/// either explodes into a "telescoping constructor" with a dozen
/// nullable parameters, or accepts an invalid partial state.
///
/// The Builder pattern instead exposes small, named, chainable steps
/// that mirror how a customer actually assembles an order in the UI:
/// pick a restaurant, add items one at a time, choose delivery or
/// pickup, add notes, then build. Each step returns the builder itself
/// (fluent interface) so calls can be chained.
/// </summary>
public interface IOrderBuilder
{
    // tag::fluent-contract[]
    IOrderBuilder ForCustomer(Customer customer);
    IOrderBuilder FromRestaurant(Restaurant restaurant);
    IOrderBuilder AddItem(IOrderItem item);
    IOrderBuilder AsDelivery(Address deliveryAddress);
    IOrderBuilder AsPickup();
    IOrderBuilder WithSpecialInstructions(string instructions);

    Order Build();
    // end::fluent-contract[]
}
