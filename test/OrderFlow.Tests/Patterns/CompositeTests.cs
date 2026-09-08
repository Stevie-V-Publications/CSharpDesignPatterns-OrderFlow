using OrderFlow.Patterns.Composite;
using Xunit;

namespace OrderFlow.Tests.Patterns;

/// <summary>
/// Pattern 7: Composite. One call on the root recurses the whole tree,
/// and the caller never branches on "leaf or branch". A combo meal is
/// just a branch that prices itself differently — and nobody asking for
/// its price can tell.
/// </summary>
public class CompositeTests
{
    private static MenuComposite SampleMenu() =>
        new MenuComposite("Menu")
            .Add(new MenuComposite("Pizzas")
                .Add(new MenuLeaf(Sample.MenuItem("Margherita Pizza", 14.00m)))
                .Add(new MenuLeaf(Sample.MenuItem("Pepperoni Pizza", 15.50m))))
            .Add(new MenuComposite("Combos")
                .Add(new ComboMeal("Pizza Night", discount: 4.00m)
                    .Add(new MenuLeaf(Sample.MenuItem("Pepperoni Pizza", 15.50m)))
                    .Add(new MenuLeaf(Sample.MenuItem("Caesar Salad", 9.00m)))));

    // tag::composite-recursion[]
    [Fact]
    public void Price_on_the_root_sums_every_leaf_in_the_tree()
    {
        var menu = SampleMenu();

        // 14.00 + 15.50 (pizzas) + (15.50 + 9.00 - 4.00) (combo) = 50.00
        Assert.Equal(50.00m, menu.Price);
    }

    [Fact]
    public void A_combo_subtracts_its_discount_and_the_caller_never_knows()
    {
        var combo = new ComboMeal("Pizza Night", discount: 4.00m)
            .Add(new MenuLeaf(Sample.MenuItem("Pepperoni Pizza", 15.50m)))
            .Add(new MenuLeaf(Sample.MenuItem("Caesar Salad", 9.00m)));

        // Same property the menu UI reads for a plain category.
        Assert.Equal(20.50m, combo.Price);
    }
    // end::composite-recursion[]

    [Fact]
    public void Describe_emits_one_line_per_node_indented_by_depth()
    {
        var lines = SampleMenu().Describe().ToList();

        Assert.StartsWith("Menu (", lines[0]);          // depth 0: no indent
        Assert.StartsWith("  Pizzas (", lines[1]);      // depth 1: two spaces
        Assert.StartsWith("    - Margherita Pizza", lines[2]); // depth 2: four spaces
    }

    [Fact]
    public void A_leaf_prices_itself_without_recursing()
    {
        var leaf = new MenuLeaf(Sample.MenuItem("Garlic Knots", 6.50m));

        Assert.Equal(6.50m, leaf.Price);
        Assert.Single(leaf.Describe());
    }
}
