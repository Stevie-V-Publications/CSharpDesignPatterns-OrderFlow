using OrderFlow.Domain;

namespace OrderFlow.Patterns.Decorator;

/// <summary>
/// Gift-wraps the item. Unlike the others this is a percentage of
/// whatever it wraps (fancier box for a fancier order), which is why
/// the <em>order</em> you stack decorators in can change the total:
/// gift-wrapping then adding cheese vs. adding cheese then gift-wrapping
/// give different prices. The Pattern Playground lets you see that.
/// </summary>
// tag::gift-wrap[]
public sealed class GiftWrap(IOrderItem inner) : OrderItemDecorator(inner)
{
    private const decimal Rate = 0.10m;

    public override decimal GetPrice() => Inner.GetPrice() * (1 + Rate);

    public override string GetDescription() => $"{Inner.GetDescription()} [gift-wrapped]";
}
// end::gift-wrap[]
