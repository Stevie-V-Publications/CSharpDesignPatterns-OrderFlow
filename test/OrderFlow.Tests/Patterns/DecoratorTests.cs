using OrderFlow.Domain;
using OrderFlow.Patterns.Decorator;
using Xunit;

namespace OrderFlow.Tests.Patterns;

/// <summary>
/// Pattern 5: Decorator. Add-ons stack in any combination and any order,
/// and the thing underneath never changes. The naive boolean-flags
/// version can't express "extra cheese twice" at all, and its parallel
/// price/description ladders drift out of sync — neither problem here.
/// </summary>
public class DecoratorTests
{
    private static IOrderItem Pizza() =>
        new OrderItem { MenuItem = Sample.MenuItem("Margherita Pizza", 14.00m) };

    // tag::decorator-stacking[]
    [Fact]
    public void Each_add_on_folds_its_own_contribution_onto_whatever_it_wraps()
    {
        IOrderItem item = Pizza();                 // 14.00
        item = new ExtraCheese(item);              // + 1.50  -> 15.50
        item = new GlutenFreeCrust(item);          // + 2.00  -> 17.50

        Assert.Equal(17.50m, item.GetPrice());
        Assert.Equal("Margherita Pizza + extra cheese (gluten-free crust)", item.GetDescription());
    }

    [Fact]
    public void The_same_add_on_can_be_applied_more_than_once()
    {
        IOrderItem item = new ExtraCheese(new ExtraCheese(Pizza()));

        Assert.Equal(17.00m, item.GetPrice());     // 14.00 + 1.50 + 1.50
    }

    [Fact]
    public void Stacking_order_changes_the_total_when_a_percentage_add_on_is_involved()
    {
        // GiftWrap is 10% of whatever's inside it.
        var cheeseThenWrap = new GiftWrap(new ExtraCheese(Pizza())).GetPrice();   // (14 + 1.50) * 1.10
        var wrapThenCheese = new ExtraCheese(new GiftWrap(Pizza())).GetPrice();   // (14 * 1.10) + 1.50

        Assert.Equal(17.05m, cheeseThenWrap);
        Assert.Equal(16.90m, wrapThenCheese);
        Assert.NotEqual(cheeseThenWrap, wrapThenCheese);
    }
    // end::decorator-stacking[]

    [Fact]
    public void A_bare_item_is_unchanged_by_being_wrappable()
    {
        var bare = Pizza();

        Assert.Equal(14.00m, bare.GetPrice());
        Assert.Equal("Margherita Pizza", bare.GetDescription());
    }
}
