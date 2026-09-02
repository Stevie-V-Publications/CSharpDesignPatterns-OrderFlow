using OrderFlow.Domain;

namespace OrderFlow.Patterns.Decorator;

/// <summary>
/// Adds extra cheese: a flat surcharge and a note on the description.
/// Because it folds onto whatever it wraps, "extra cheese on extra
/// cheese" is legal and costs twice — which is exactly how the real
/// counter works.
/// </summary>
// tag::extra-cheese[]
public sealed class ExtraCheese(IOrderItem inner) : OrderItemDecorator(inner)
{
    private const decimal Surcharge = 1.50m;

    public override decimal GetPrice() => Inner.GetPrice() + Surcharge;

    public override string GetDescription() => $"{Inner.GetDescription()} + extra cheese";
}
// end::extra-cheese[]
