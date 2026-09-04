using OrderFlow.Domain;
using OrderFlow.Patterns.Adapter;
using OrderFlow.Patterns.Builder;
using OrderFlow.Patterns.Facade;
using OrderFlow.Patterns.FactoryMethod;
using OrderFlow.Patterns.Singleton;
using OrderFlow.Services;
using Xunit;

namespace OrderFlow.Tests.Patterns;

/// <summary>
/// Pattern 6: Facade. The naive version put this whole sequence in a
/// Blazor click handler holding four subsystems — untestable without
/// standing up a UI. Here one call, one result, and the ordering
/// guarantee (a declined card never routes food to the kitchen) is
/// asserted directly.
/// </summary>
public class FacadeTests
{
    /// <summary>A payment processor we can point at "approve" or "decline".</summary>
    private sealed class StubProcessor(bool succeeds) : IPaymentProcessor
    {
        public string ProviderName => "Stub";
        public int ChargeCalls { get; private set; }

        public PaymentResult Charge(PaymentRequest request)
        {
            ChargeCalls++;
            return succeeds
                ? PaymentResult.Ok(ProviderName, "txn_1")
                : PaymentResult.Failed(ProviderName, "Card declined.");
        }
    }

    private static Order OrderFor(Restaurant restaurant, string item = "Margherita Pizza") =>
        new OrderBuilder()
            .ForCustomer(Sample.Customer(NotificationPreference.Email))
            .FromRestaurant(restaurant)
            .AddItem(new OrderItem { MenuItem = Sample.MenuItem(item) })
            .AsPickup()
            .Build();

    private static CheckoutFacade FacadeWith(IPaymentProcessor processor, InMemoryInventoryService inventory) =>
        new(inventory, [processor], new NotificationFactory(), KitchenDisplayService.Instance);

    // tag::facade-one-call[]
    [Fact]
    public void A_clean_order_comes_back_confirmed_with_every_stage_run()
    {
        var restaurant = Sample.Restaurant();
        var facade = FacadeWith(new StubProcessor(succeeds: true), new InMemoryInventoryService());

        var result = facade.PlaceOrder(new PlaceOrderRequest(OrderFor(restaurant), "Stub", "tok"));

        Assert.True(result.Success);
        Assert.Null(result.FailedStage);
        Assert.NotNull(result.Notification);
        Assert.Equal(1, KitchenDisplayService.Instance.QueueFor(restaurant).Count);
    }
    // end::facade-one-call[]

    // tag::facade-short-circuit[]
    [Fact]
    public void A_declined_card_stops_before_the_kitchen_is_touched()
    {
        var restaurant = Sample.Restaurant();
        var processor = new StubProcessor(succeeds: false);
        var facade = FacadeWith(processor, new InMemoryInventoryService());

        var result = facade.PlaceOrder(new PlaceOrderRequest(OrderFor(restaurant), "Stub", "tok"));

        Assert.False(result.Success);
        Assert.Equal("Payment", result.FailedStage);
        Assert.Equal(1, processor.ChargeCalls);
        Assert.Equal(0, KitchenDisplayService.Instance.QueueFor(restaurant).Count);
    }

    [Fact]
    public void A_sold_out_item_stops_before_any_money_moves()
    {
        var restaurant = Sample.Restaurant();
        var inventory = new InMemoryInventoryService();
        inventory.SoldOut.Add("Margherita Pizza");
        var processor = new StubProcessor(succeeds: true);
        var facade = FacadeWith(processor, inventory);

        var result = facade.PlaceOrder(new PlaceOrderRequest(OrderFor(restaurant), "Stub", "tok"));

        Assert.False(result.Success);
        Assert.Equal("Inventory", result.FailedStage);
        Assert.Equal(0, processor.ChargeCalls);
    }
    // end::facade-short-circuit[]

    [Fact]
    public void An_unknown_payment_provider_is_rejected_at_the_payment_stage()
    {
        var facade = FacadeWith(new StubProcessor(succeeds: true), new InMemoryInventoryService());

        var result = facade.PlaceOrder(
            new PlaceOrderRequest(OrderFor(Sample.Restaurant()), "NotRegistered", "tok"));

        Assert.False(result.Success);
        Assert.Equal("Payment", result.FailedStage);
    }
}
