namespace OrderFlow.Patterns.Strategy;

/// <summary>
/// Pattern 10: Strategy.
///
/// Delivery pricing rules vary by context and by business mood: a flat
/// fee this month, distance-based next month, surge multipliers on a
/// rainy Friday night. The checkout code that shows a delivery fee
/// shouldn't care which rule is in force -- it should ask a strategy.
///
/// Each rule is its own class implementing this interface; swapping the
/// rule is swapping the object, not editing an <c>if</c> ladder on
/// <c>DeliveryType</c>.
/// </summary>
// tag::pricing-strategy[]
public interface IDeliveryPricingStrategy
{
    /// <summary>Human-readable name, e.g. for a settings dropdown.</summary>
    string Name { get; }

    DeliveryQuote Quote(DeliveryContext context);
}

/// <summary>Everything a pricing rule might need to do its job.</summary>
public record DeliveryContext(
    decimal OrderSubtotal,
    double DistanceMiles,
    double DemandMultiplier = 1.0);

public record DeliveryQuote(decimal Fee, string Explanation);
// end::pricing-strategy[]
