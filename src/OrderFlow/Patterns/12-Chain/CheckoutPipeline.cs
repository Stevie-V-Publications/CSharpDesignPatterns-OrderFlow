namespace OrderFlow.Patterns.ChainOfResponsibility;

/// <summary>
/// Builds the standard checkout chain and exposes its head. Keeping the
/// wiring in one place means the <em>order</em> of the checks -- fraud,
/// then inventory, then payment -- is stated once, not re-derived by
/// every caller.
/// </summary>
// tag::pipeline[]
public static class CheckoutPipeline
{
    public static CheckoutHandler Standard(ISet<string> soldOut)
    {
        var fraud = new FraudCheckHandler();
        var inventory = new InventoryCheckHandler(soldOut);
        var payment = new PaymentAuthorizationHandler();

        // fraud -> inventory -> payment. SetNext returns the argument,
        // so this chains, but we hand back the head.
        fraud.SetNext(inventory).SetNext(payment);
        return fraud;
    }
}
// end::pipeline[]
