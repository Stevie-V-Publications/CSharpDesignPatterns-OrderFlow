using OrderFlow.Domain;

namespace OrderFlow.Patterns.Decorator;

/// <summary>
/// Swaps in a gluten-free crust: a flat upcharge and a parenthetical
/// on the description.
/// </summary>
public sealed class GlutenFreeCrust(IOrderItem inner) : OrderItemDecorator(inner)
{
    private const decimal Upcharge = 2.00m;

    public override decimal GetPrice() => Inner.GetPrice() + Upcharge;

    public override string GetDescription() => $"{Inner.GetDescription()} (gluten-free crust)";
}
