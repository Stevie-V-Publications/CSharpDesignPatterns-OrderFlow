namespace OrderFlow.Patterns.Composite;

/// <summary>
/// Pattern 7: Composite.
///
/// The single type that both a leaf (one sellable item) and a branch
/// (a category, or a combo meal) implement. Client code — the menu UI,
/// a "what does this cost?" call — walks a tree of these without ever
/// asking "is this one thing or many?".
/// </summary>
// tag::menu-component[]
public interface IMenuComponent
{
    string Name { get; }

    /// <summary>
    /// Price of this node. For a leaf it's the item price; for a
    /// branch it's however that branch chooses to total its children
    /// (a plain sum for a category, sum-minus-discount for a combo).
    /// </summary>
    decimal Price { get; }

    /// <summary>
    /// Emits one text line per node, indented by depth — the uniform
    /// operation that works the same on a leaf and a whole subtree.
    /// </summary>
    IEnumerable<string> Describe(int depth = 0);
}
// end::menu-component[]
