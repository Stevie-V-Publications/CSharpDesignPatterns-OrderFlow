using OrderFlow.Patterns.ChainOfResponsibility;
using Xunit;

namespace OrderFlow.Tests.Patterns;

/// <summary>
/// Bonus Pattern: Chain of Responsibility. A handler either rejects and
/// stops the chain, or passes the request along. Reordering or inserting
/// a check is a wiring change, not an edit to a growing method.
/// </summary>
public class ChainOfResponsibilityTests
{
    private static CheckoutContext Context(
        string risk = "none", decimal total = 30m, bool cardOk = true, string[]? items = null) =>
        new("order-1", total, items ?? ["Margherita Pizza"], risk, cardOk);

    // tag::chain-short-circuit[]
    [Fact]
    public void A_clean_order_passes_every_gate()
    {
        var chain = CheckoutPipeline.Standard(soldOut: new HashSet<string>());

        var decision = chain.Handle(Context());

        Assert.True(decision.Approved);
    }

    [Fact]
    public void The_fraud_gate_rejects_and_the_later_gates_never_run()
    {
        var soldOut = new HashSet<string> { "Margherita Pizza" };  // would also fail inventory
        var chain = CheckoutPipeline.Standard(soldOut);

        var decision = chain.Handle(Context(risk: "chargeback-history"));

        Assert.False(decision.Approved);
        Assert.Equal("Fraud check", decision.RejectedBy);   // not "Inventory check"
    }
    // end::chain-short-circuit[]

    [Fact]
    public void The_inventory_gate_rejects_a_sold_out_item()
    {
        var chain = CheckoutPipeline.Standard(new HashSet<string> { "Caesar Salad" });

        var decision = chain.Handle(Context(items: ["Caesar Salad"]));

        Assert.False(decision.Approved);
        Assert.Equal("Inventory check", decision.RejectedBy);
    }

    [Fact]
    public void The_payment_gate_is_last_and_rejects_an_unauthorized_card()
    {
        var chain = CheckoutPipeline.Standard(new HashSet<string>());

        var decision = chain.Handle(Context(cardOk: false));

        Assert.False(decision.Approved);
        Assert.Equal("Payment authorization", decision.RejectedBy);
    }
}
