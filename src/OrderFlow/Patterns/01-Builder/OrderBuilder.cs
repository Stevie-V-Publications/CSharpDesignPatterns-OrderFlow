using OrderFlow.Domain;

namespace OrderFlow.Patterns.Builder;

/// <summary>
/// Concrete builder. Registered as Transient in Program.cs — a new
/// instance is created per order being assembled (typically per
/// checkout session), not shared. That's a deliberate DI lifetime
/// choice: a Builder holds in-progress, mutable state for exactly one
/// product, so Singleton/Scoped would leak one customer's in-progress
/// order into another's.
/// </summary>
public class OrderBuilder : IOrderBuilder
{
    // tag::builder-state[]
    private readonly List<IOrderItem> _items = [];
    private Customer? _customer;
    private Restaurant? _restaurant;
    private OrderType _type = OrderType.Pickup;
    private Address? _deliveryAddress;
    private string? _specialInstructions;
    // end::builder-state[]

    public IOrderBuilder ForCustomer(Customer customer)
    {
        _customer = customer;
        return this;
    }

    public IOrderBuilder FromRestaurant(Restaurant restaurant)
    {
        _restaurant = restaurant;
        return this;
    }

    // tag::fluent-steps[]
    public IOrderBuilder AddItem(IOrderItem item)
    {
        _items.Add(item);
        return this;
    }

    public IOrderBuilder AsDelivery(Address deliveryAddress)
    {
        _type = OrderType.Delivery;
        _deliveryAddress = deliveryAddress;
        return this;
    }
    // end::fluent-steps[]

    public IOrderBuilder AsPickup()
    {
        _type = OrderType.Pickup;
        _deliveryAddress = null;
        return this;
    }

    public IOrderBuilder WithSpecialInstructions(string instructions)
    {
        _specialInstructions = instructions;
        return this;
    }

    // tag::build-method[]
    public Order Build()
    {
        if (_customer is null)
            throw new InvalidOperationException("Call ForCustomer(...) before Build().");

        if (_restaurant is null)
            throw new InvalidOperationException("Call FromRestaurant(...) before Build().");

        // Order's own constructor re-validates item count and the
        // delivery/address invariant too — belt-and-braces on purpose.
        // The builder fails fast with a builder-specific message; Order
        // guards itself against ever being constructed invalidly by any
        // other path.
        return new Order(
            id: Guid.NewGuid(),
            customer: _customer,
            restaurant: _restaurant,
            type: _type,
            items: _items.ToList(),
            deliveryAddress: _deliveryAddress,
            specialInstructions: _specialInstructions,
            createdAt: DateTimeOffset.UtcNow);
    }
    // end::build-method[]
}
