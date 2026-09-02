using OrderFlow.Domain;

namespace OrderFlow.Patterns.Composite;

/// <summary>
/// The leaf: exactly one <see cref="MenuItem"/>. It has no children,
/// so <see cref="Describe"/> is a single line and <see cref="Price"/>
/// is just the item's price.
/// </summary>
// tag::menu-leaf[]
public sealed class MenuLeaf(MenuItem item) : IMenuComponent
{
    /// <summary>The sellable item this leaf stands for — the menu UI
    /// needs it to build a cart line once the customer picks this node.</summary>
    public MenuItem Item => item;

    public string Name => item.Name;

    public decimal Price => item.BasePrice;

    public IEnumerable<string> Describe(int depth = 0) =>
        [$"{new string(' ', depth * 2)}- {Name} ({Price:C})"];
}
// end::menu-leaf[]
