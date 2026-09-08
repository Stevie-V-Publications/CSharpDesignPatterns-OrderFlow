using OrderFlow.Patterns.Strategy;
using Xunit;

namespace OrderFlow.Tests.Patterns;

/// <summary>
/// Pattern 10: Strategy. Each pricing rule is tested on its own, with no
/// giant <c>switch</c> to construct around it. The caller holds an
/// <see cref="IDeliveryPricingStrategy"/> and never learns which one.
/// </summary>
public class StrategyTests
{
    // tag::strategy-interchangeable[]
    [Fact]
    public void Flat_rate_ignores_distance_entirely()
    {
        IDeliveryPricingStrategy strategy = new FlatRateStrategy();

        var near = strategy.Quote(new DeliveryContext(OrderSubtotal: 20m, DistanceMiles: 1));
        var far = strategy.Quote(new DeliveryContext(OrderSubtotal: 20m, DistanceMiles: 25));

        Assert.Equal(4.99m, near.Fee);
        Assert.Equal(near.Fee, far.Fee);
    }

    [Fact]
    public void Distance_based_charges_a_base_fee_plus_per_mile()
    {
        IDeliveryPricingStrategy strategy = new DistanceBasedStrategy();

        var quote = strategy.Quote(new DeliveryContext(OrderSubtotal: 20m, DistanceMiles: 4));

        Assert.Equal(7.00m, quote.Fee);   // 2.00 base + 1.25 * 4
    }

    [Fact]
    public void Surge_multiplies_the_distance_fee_by_live_demand()
    {
        IDeliveryPricingStrategy strategy = new SurgeStrategy();

        var quote = strategy.Quote(new DeliveryContext(
            OrderSubtotal: 20m, DistanceMiles: 4, DemandMultiplier: 2.0));

        Assert.Equal(14.00m, quote.Fee);  // (2.00 + 1.25 * 4) * 2.0
    }
    // end::strategy-interchangeable[]

    [Fact]
    public void Surge_still_honors_the_free_delivery_promo_over_the_threshold()
    {
        var quote = new SurgeStrategy().Quote(new DeliveryContext(
            OrderSubtotal: 45m, DistanceMiles: 10, DemandMultiplier: 3.0));

        Assert.Equal(0m, quote.Fee);
    }
}
