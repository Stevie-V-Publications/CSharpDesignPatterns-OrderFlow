using OrderFlow.Domain;
using OrderFlow.Patterns.Observer;
using Xunit;

namespace OrderFlow.Tests.Patterns;

/// <summary>
/// Pattern 8: Observer. The subject notifies subscribers it knows
/// nothing about, and a subscriber can drop off at runtime by disposing
/// its subscription. The naive hard-coded call list can't do either
/// without editing the order code.
/// </summary>
public class ObserverTests
{
    private sealed class Spy : IOrderStatusObserver
    {
        public List<OrderStatusChange> Received { get; } = [];
        public void OnOrderStatusChanged(OrderStatusChange change) => Received.Add(change);
    }

    private static OrderStatusChange AChange() =>
        OrderStatusChange.Between(Guid.NewGuid(), OrderStatus.Preparing, OrderStatus.Ready);

    // tag::observer-subscribe-notify[]
    [Fact]
    public void Every_subscriber_hears_one_published_change()
    {
        var publisher = new OrderStatusPublisher();
        var a = new Spy();
        var b = new Spy();
        publisher.Subscribe(a);
        publisher.Subscribe(b);

        var change = AChange();
        publisher.Publish(change);

        Assert.Equal(change, Assert.Single(a.Received));
        Assert.Equal(change, Assert.Single(b.Received));
    }

    [Fact]
    public void Disposing_the_subscription_stops_delivery()
    {
        var publisher = new OrderStatusPublisher();
        var spy = new Spy();
        var subscription = publisher.Subscribe(spy);

        subscription.Dispose();
        publisher.Publish(AChange());

        Assert.Empty(spy.Received);
    }
    // end::observer-subscribe-notify[]

    [Fact]
    public void The_real_screen_observers_each_render_the_same_change_differently()
    {
        var publisher = new OrderStatusPublisher();
        var kitchen = new KitchenDisplayObserver();
        var customer = new CustomerTrackerObserver();
        publisher.Subscribe(kitchen);
        publisher.Subscribe(customer);

        publisher.Publish(OrderStatusChange.Between(Guid.NewGuid(), OrderStatus.Placed, OrderStatus.Confirmed));

        Assert.Contains("start prepping", Assert.Single(kitchen.Log));
        Assert.Contains("🎉", Assert.Single(customer.Log));
    }

    [Fact]
    public void Publishing_with_no_subscribers_is_a_no_op()
    {
        var publisher = new OrderStatusPublisher();

        publisher.Publish(AChange());   // must not throw

        Assert.Equal(0, publisher.ObserverCount);
    }
}
