namespace OrderFlow.Domain;

/// <summary>
/// The product that Pattern 1 (Builder) assembles.
///
/// Notice the constructor is <c>internal</c>, not <c>public</c>. An
/// Order has too many interdependent, partly-optional pieces (order
/// type, items, delivery address only-if-delivery, special
/// instructions...) to construct safely with a single public
/// constructor without either a telescoping-constructor mess or a
/// caller forgetting to set something required. <see cref="OrderBuilder"/>
/// (Patterns/01-Builder) is the only supported way to produce a valid
/// instance, and its <c>Build()</c> method is where the validation
/// invariants below actually get enforced step by step.
///
/// In a larger solution, Domain would live in its own class library so
/// "internal" meaningfully blocked external callers. Here, in a single
/// Blazor project, treat the internal constructor as a strong signal
/// rather than a hard wall — the point for readers is the *design*,
/// not assembly boundaries.
/// </summary>
public class Order
{
    public Guid Id { get; }
    public Customer Customer { get; }
    public Restaurant Restaurant { get; }
    public OrderType Type { get; }
    public IReadOnlyList<IOrderItem> Items { get; }
    public Address? DeliveryAddress { get; }
    public string? SpecialInstructions { get; }
    public DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// Settable internally so Pattern 9 (State) can drive transitions
    /// through this order without exposing a public setter that would
    /// let any caller jump straight from Placed to Completed.
    /// </summary>
    public OrderStatus Status { get; internal set; } = OrderStatus.Placed;

    internal Order(
        Guid id,
        Customer customer,
        Restaurant restaurant,
        OrderType type,
        IReadOnlyList<IOrderItem> items,
        Address? deliveryAddress,
        string? specialInstructions,
        DateTimeOffset createdAt)
    {
        if (items.Count == 0)
            throw new InvalidOperationException("An order must contain at least one item.");

        if (type == OrderType.Delivery && deliveryAddress is null)
            throw new InvalidOperationException("A delivery order requires a delivery address.");

        Id = id;
        Customer = customer;
        Restaurant = restaurant;
        Type = type;
        Items = items;
        DeliveryAddress = deliveryAddress;
        SpecialInstructions = specialInstructions;
        CreatedAt = createdAt;
    }

    public decimal Subtotal => Items.Sum(i => i.GetPrice());
}
