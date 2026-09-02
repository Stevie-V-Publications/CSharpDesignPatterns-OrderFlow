namespace OrderFlow.Patterns.Composite;

/// <summary>
/// A branch node: a named group of child components, which may be
/// leaves or other branches. This is a menu category ("Pizzas",
/// "Sides") or any nesting of them.
///
/// The recursion lives here: <see cref="Price"/> and
/// <see cref="Describe"/> just fold over the children, and because a
/// child might itself be a <see cref="MenuComposite"/>, one call on the
/// root walks the whole tree.
/// </summary>
// tag::menu-composite[]
public class MenuComposite(string name) : IMenuComponent
{
    private readonly List<IMenuComponent> _children = [];

    public string Name => name;

    public IReadOnlyList<IMenuComponent> Children => _children;

    public MenuComposite Add(IMenuComponent child)
    {
        _children.Add(child);
        return this;
    }

    public virtual decimal Price => _children.Sum(c => c.Price);

    public IEnumerable<string> Describe(int depth = 0)
    {
        yield return $"{new string(' ', depth * 2)}{Name} ({Price:C})";
        foreach (var line in _children.SelectMany(c => c.Describe(depth + 1)))
            yield return line;
    }
}
// end::menu-composite[]
