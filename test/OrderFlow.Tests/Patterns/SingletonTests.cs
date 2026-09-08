using OrderFlow.Patterns.Singleton;
using Xunit;

namespace OrderFlow.Tests.Patterns;

/// <summary>
/// Pattern 3: Singleton. The guarantee under test: one instance for the
/// process, and one queue per restaurant handed back to every caller.
///
/// These tests also demonstrate the pattern's cost the book warns about
/// — the single instance is shared process-wide, so each test has to use
/// a restaurant with a fresh id to avoid leaking queue state into the
/// next test. That friction is exactly why DI-managed lifetimes are
/// usually the better call.
/// </summary>
public class SingletonTests
{
    // tag::singleton-identity[]
    [Fact]
    public void Instance_is_always_the_same_object()
    {
        Assert.Same(KitchenDisplayService.Instance, KitchenDisplayService.Instance);
    }

    [Fact]
    public void The_same_restaurant_always_gets_the_same_queue_back()
    {
        var restaurant = Sample.Restaurant();

        var first = KitchenDisplayService.Instance.QueueFor(restaurant);
        var second = KitchenDisplayService.Instance.QueueFor(restaurant);

        Assert.Same(first, second);
    }
    // end::singleton-identity[]

    [Fact]
    public void Different_restaurants_get_different_queues()
    {
        var a = KitchenDisplayService.Instance.QueueFor(Sample.Restaurant("A"));
        var b = KitchenDisplayService.Instance.QueueFor(Sample.Restaurant("B"));

        Assert.NotSame(a, b);
    }

    [Fact]
    public void A_ticket_enqueued_through_the_shared_service_is_visible_to_the_next_caller()
    {
        var restaurant = Sample.Restaurant();
        KitchenDisplayService.Instance
            .QueueFor(restaurant)
            .Enqueue(KitchenTicket.For(Guid.NewGuid(), "Margherita Pizza"));

        // A second, independent lookup — as a different screen would do.
        var seenByAnotherScreen = KitchenDisplayService.Instance.QueueFor(restaurant);

        Assert.Equal(1, seenByAnotherScreen.Count);
    }
}
