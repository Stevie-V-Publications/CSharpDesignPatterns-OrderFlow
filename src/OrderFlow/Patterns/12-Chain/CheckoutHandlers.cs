namespace OrderFlow.Patterns.ChainOfResponsibility;

// tag::concrete-handlers[]
/// <summary>First gate: known-bad customers and implausibly large orders.</summary>
public sealed class FraudCheckHandler : CheckoutHandler
{
    private const decimal ReviewThreshold = 500.00m;

    public override string Name => "Fraud check";

    protected override CheckoutDecision Check(CheckoutContext context)
    {
        if (context.CustomerRiskFlag == "chargeback-history")
            return CheckoutDecision.Reject(Name, "Customer has a prior chargeback.");

        if (context.OrderTotal > ReviewThreshold)
            return CheckoutDecision.Reject(Name, $"Order over {ReviewThreshold:C} needs manual review.");

        return CheckoutDecision.Pass;
    }
}

/// <summary>Second gate: can the kitchen actually make all of this right now?</summary>
public sealed class InventoryCheckHandler(ISet<string> soldOut) : CheckoutHandler
{
    public override string Name => "Inventory check";

    protected override CheckoutDecision Check(CheckoutContext context)
    {
        var missing = context.Items.Where(soldOut.Contains).Distinct().ToList();
        return missing.Count == 0
            ? CheckoutDecision.Pass
            : CheckoutDecision.Reject(Name, $"Sold out: {string.Join(", ", missing)}");
    }
}

/// <summary>Last gate: is the card good for it?</summary>
public sealed class PaymentAuthorizationHandler : CheckoutHandler
{
    public override string Name => "Payment authorization";

    protected override CheckoutDecision Check(CheckoutContext context) =>
        context.CardPreAuthorized
            ? CheckoutDecision.Pass
            : CheckoutDecision.Reject(Name, "Card authorization was declined.");
}
// end::concrete-handlers[]
