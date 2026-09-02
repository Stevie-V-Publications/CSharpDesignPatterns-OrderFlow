namespace OrderFlow.Patterns.Strategy;

// tag::pricing-strategies[]
/// <summary>One price, every order. Distance and demand are ignored.</summary>
public sealed class FlatRateStrategy : IDeliveryPricingStrategy
{
    private const decimal Fee_ = 4.99m;

    public string Name => "Flat rate";

    public DeliveryQuote Quote(DeliveryContext context) =>
        new(Fee_, $"Flat delivery fee of {Fee_:C}, regardless of distance.");
}

/// <summary>A base fee plus a per-mile charge.</summary>
public sealed class DistanceBasedStrategy : IDeliveryPricingStrategy
{
    private const decimal BaseFee = 2.00m;
    private const decimal PerMile = 1.25m;

    public string Name => "Distance-based";

    public DeliveryQuote Quote(DeliveryContext context)
    {
        var miles = (decimal)context.DistanceMiles;
        var fee = BaseFee + PerMile * miles;
        return new DeliveryQuote(fee,
            $"{BaseFee:C} base + {PerMile:C}/mi x {miles:0.0} mi = {fee:C}.");
    }
}

/// <summary>
/// Distance-based, then multiplied by a live demand factor. Free
/// delivery over a spend threshold still applies -- surge doesn't
/// override a promo.
/// </summary>
public sealed class SurgeStrategy : IDeliveryPricingStrategy
{
    private const decimal BaseFee = 2.00m;
    private const decimal PerMile = 1.25m;
    private const decimal FreeOver = 40.00m;

    public string Name => "Surge";

    public DeliveryQuote Quote(DeliveryContext context)
    {
        if (context.OrderSubtotal >= FreeOver)
            return new DeliveryQuote(0m, $"Free delivery on orders over {FreeOver:C}.");

        var miles = (decimal)context.DistanceMiles;
        var baseline = BaseFee + PerMile * miles;
        var multiplier = (decimal)context.DemandMultiplier;
        var fee = baseline * multiplier;
        return new DeliveryQuote(fee,
            $"({baseline:C} distance fee) x {multiplier:0.0} surge = {fee:C}.");
    }
}
// end::pricing-strategies[]
