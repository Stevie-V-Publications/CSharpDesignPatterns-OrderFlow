namespace OrderFlow.Patterns.Composite;

/// <summary>
/// Also a branch — a combo meal is "these components, sold together
/// for less". It's a <see cref="MenuComposite"/> that overrides
/// <see cref="Price"/> to knock a fixed discount off the child total.
///
/// The point: client code still just calls <c>.Price</c>. It has no
/// idea a discount happened, the same way it has no idea whether it's
/// looking at one item or fifty.
/// </summary>
// tag::combo-meal[]
public sealed class ComboMeal(string name, decimal discount) : MenuComposite(name)
{
    public decimal Discount => discount;

    public override decimal Price => base.Price - discount;
}
// end::combo-meal[]
