using OrderFlow.Domain;
using OrderFlow.Patterns.Builder;
using Xunit;

namespace OrderFlow.Tests.Patterns;

/// <summary>
/// Pattern 1: Builder. The payoff the naive telescoping constructor
/// can't give you: half-built state is a compile-and-run error with a
/// message that names the missing step, and the finished product is
/// always valid.
/// </summary>
public class BuilderTests
{
    // tag::builder-happy-path[]
    [Fact]
    public void Build_with_all_required_steps_produces_a_valid_order()
    {
        var order = new OrderBuilder()
            .ForCustomer(Sample.Customer())
            .FromRestaurant(Sample.Restaurant())
            .AddItem(Sample.OrderItem("Margherita Pizza", 14.00m))
            .AddItem(Sample.OrderItem("Garlic Knots", 6.50m))
            .AsPickup()
            .Build();

        Assert.Equal(2, order.Items.Count);
        Assert.Equal(OrderType.Pickup, order.Type);
        Assert.Equal(20.50m, order.Subtotal);
        Assert.Equal(OrderStatus.Placed, order.Status);
    }
    // end::builder-happy-path[]

    // tag::builder-guards[]
    [Fact]
    public void Build_without_a_customer_fails_fast_and_says_which_step_is_missing()
    {
        var builder = new OrderBuilder()
            .FromRestaurant(Sample.Restaurant())
            .AddItem(Sample.OrderItem());

        var ex = Assert.Throws<InvalidOperationException>(() => builder.Build());
        Assert.Contains("ForCustomer", ex.Message);
    }
    // end::builder-guards[]

    [Fact]
    public void Build_with_no_items_is_rejected_by_the_order_invariant()
    {
        var builder = new OrderBuilder()
            .ForCustomer(Sample.Customer())
            .FromRestaurant(Sample.Restaurant())
            .AsPickup();

        var ex = Assert.Throws<InvalidOperationException>(() => builder.Build());
        Assert.Contains("at least one item", ex.Message);
    }

    [Fact]
    public void A_delivery_order_requires_a_delivery_address()
    {
        var order = new OrderBuilder()
            .ForCustomer(Sample.Customer())
            .FromRestaurant(Sample.Restaurant())
            .AddItem(Sample.OrderItem())
            .AsDelivery(Sample.Address)
            .Build();

        Assert.Equal(OrderType.Delivery, order.Type);
        Assert.NotNull(order.DeliveryAddress);
    }

    [Fact]
    public void Director_replays_a_previous_orders_items_onto_a_fresh_builder()
    {
        var original = new OrderBuilder()
            .ForCustomer(Sample.Customer())
            .FromRestaurant(Sample.Restaurant())
            .AddItem(Sample.OrderItem("Pepperoni Pizza", 15.50m))
            .AddItem(Sample.OrderItem("Caesar Salad", 9.00m))
            .AsPickup()
            .Build();

        var reorder = new OrderDirector(new OrderBuilder()).BuildReorder(original);

        Assert.Equal(original.Items.Count, reorder.Items.Count);
        Assert.Equal(original.Subtotal, reorder.Subtotal);
        Assert.NotEqual(original.Id, reorder.Id);
    }
}
