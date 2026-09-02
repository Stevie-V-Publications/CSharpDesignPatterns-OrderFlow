using OrderFlow.Domain;
using OrderFlow.Patterns.Composite;

namespace OrderFlow.Services;

/// <summary>
/// Builds the restaurant menu as a <see cref="MenuComposite"/> tree
/// (Pattern 7). Shared by the Pattern Playground's isolated demo and the
/// real customer ordering flow, so both walk the exact same structure.
/// </summary>
public static class MenuCatalog
{
    private static MenuLeaf Leaf(string name) =>
        new(SampleData.MenuItems.First(m => m.Name == name));

    public static MenuComposite ForRestaurant(Restaurant restaurant) =>
        new MenuComposite(restaurant.Name)
            .Add(new MenuComposite("Pizzas")
                .Add(Leaf("Margherita Pizza"))
                .Add(Leaf("Pepperoni Pizza")))
            .Add(new MenuComposite("Sides")
                .Add(Leaf("Garlic Knots")))
            .Add(new MenuComposite("Salads")
                .Add(Leaf("Caesar Salad")))
            .Add(new MenuComposite("Combos")
                .Add(new ComboMeal("Pizza Night Combo", discount: 4.00m)
                    .Add(Leaf("Pepperoni Pizza"))
                    .Add(Leaf("Caesar Salad"))
                    .Add(Leaf("Garlic Knots"))));
}
