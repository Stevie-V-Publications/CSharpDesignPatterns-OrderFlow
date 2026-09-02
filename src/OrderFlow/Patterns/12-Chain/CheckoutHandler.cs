namespace OrderFlow.Patterns.ChainOfResponsibility;

/// <summary>
/// Bonus Pattern: Chain of Responsibility.
///
/// Before an order is accepted it has to clear a gauntlet: a fraud
/// screen, an inventory check, and a payment authorization. The checks
/// are ordered (no point authorizing a card for items that are sold
/// out), any one of them can reject the order, and the list of checks
/// changes over time (add a delivery-radius check, a first-order
/// promo-abuse check...).
///
/// Chain of Responsibility strings the checks into a linked list. Each
/// handler either rejects and stops the chain, or passes the request to
/// the next link. The caller just hands the request to the head of the
/// chain and reads one decision back.
/// </summary>
// tag::chain-context[]
/// <summary>Everything the checks need, gathered once up front.</summary>
public record CheckoutContext(
    string OrderReference,
    decimal OrderTotal,
    IReadOnlyList<string> Items,
    string CustomerRiskFlag,     // "none", "chargeback-history", "new-account"
    bool CardPreAuthorized);

/// <summary>The single decision the whole chain produces.</summary>
public record CheckoutDecision(bool Approved, string? RejectedBy, string? Reason)
{
    public static CheckoutDecision Pass { get; } = new(true, null, null);

    public static CheckoutDecision Reject(string by, string reason) => new(false, by, reason);
}
// end::chain-context[]

// tag::chain-handler[]
public abstract class CheckoutHandler
{
    private CheckoutHandler? _next;

    public abstract string Name { get; }

    /// <summary>Wire the next link. Returns <paramref name="next"/> so a
    /// chain can be built fluently: <c>a.SetNext(b).SetNext(c)</c>.</summary>
    public CheckoutHandler SetNext(CheckoutHandler next)
    {
        _next = next;
        return next;
    }

    public CheckoutDecision Handle(CheckoutContext context)
    {
        var decision = Check(context);
        if (!decision.Approved)
            return decision;                 // reject: stop here

        return _next?.Handle(context) ?? CheckoutDecision.Pass;  // pass it along
    }

    protected abstract CheckoutDecision Check(CheckoutContext context);
}
// end::chain-handler[]
