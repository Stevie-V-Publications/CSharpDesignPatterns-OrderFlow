using OrderFlow.Domain;
using OrderFlow.Patterns.Adapter;
using OrderFlow.Patterns.FactoryMethod;
using OrderFlow.Patterns.Singleton;
using OrderFlow.Patterns.State;
using OrderFlow.Services;

namespace OrderFlow.Patterns.Facade;

/// <summary>
/// Pattern 6: Facade.
///
/// "Place Order" is one button, but behind it: check the kitchen can
/// make everything, take payment, put the ticket on the right
/// restaurant's kitchen queue, and tell the customer. Four subsystems,
/// each with its own failure modes and its own home elsewhere in the
/// codebase (Adapter, Singleton, Factory Method, plus inventory).
///
/// The Facade gives callers <em>one</em> method and one result type,
/// runs the steps in the right order, and stops at the first failure
/// (a declined card must not still route food to the kitchen). It adds
/// no new business rules — it just knows the dance.
/// </summary>
public sealed class CheckoutFacade(
    IInventoryService inventory,
    IEnumerable<IPaymentProcessor> paymentProcessors,
    NotificationFactory notificationFactory,
    KitchenDisplayService kitchenDisplay)
{
    // tag::place-order[]
    public PlaceOrderResult PlaceOrder(PlaceOrderRequest request)
    {
        var order = request.Order;

        // 1. Inventory — bail before touching the customer's card.
        var stock = inventory.CheckAvailability(order.Restaurant, order.Items);
        if (!stock.AllAvailable)
            return PlaceOrderResult.Rejected("Inventory",
                $"Sold out: {string.Join(", ", stock.Unavailable)}");

        // 2. Payment — via whichever provider adapter was asked for.
        var processor = paymentProcessors.FirstOrDefault(p =>
            p.ProviderName.Equals(request.PaymentProvider, StringComparison.OrdinalIgnoreCase));
        if (processor is null)
            return PlaceOrderResult.Rejected("Payment",
                $"No payment provider named '{request.PaymentProvider}'.");

        var payment = processor.Charge(new PaymentRequest(
            order.Subtotal, "USD", order.Id.ToString(), request.PaymentToken));
        if (!payment.Succeeded)
            return PlaceOrderResult.Rejected("Payment", payment.FailureReason ?? "Card declined.", payment);

        // 3. Kitchen — money's in, send the ticket to the line.
        var queue = kitchenDisplay.QueueFor(order.Restaurant);
        var summary = string.Join(", ", order.Items.Select(i => i.GetDescription()));
        queue.Enqueue(KitchenTicket.For(order.Id, summary));

        // Pattern 9 (State) owns this transition: Placed -> Confirmed,
        // via the state object, not a raw status assignment.
        new OrderStateMachine(order).Advance();

        // 4. Notify — however this customer asked to hear from us.
        var receipt = notificationFactory
            .CreateFor(order.Customer)
            .Send(order.Customer, $"Order confirmed at {order.Restaurant.Name}!");

        return PlaceOrderResult.Confirmed(payment, queue.Count, receipt);
    }
    // end::place-order[]
}

// tag::facade-dtos[]
public record PlaceOrderRequest(Order Order, string PaymentProvider, string PaymentToken);

public record PlaceOrderResult(
    bool Success,
    string? FailedStage,
    string? FailureReason,
    PaymentResult? Payment,
    int KitchenQueuePosition,
    NotificationReceipt? Notification)
{
    public static PlaceOrderResult Rejected(string stage, string reason, PaymentResult? payment = null) =>
        new(false, stage, reason, payment, 0, null);

    public static PlaceOrderResult Confirmed(PaymentResult payment, int queuePosition, NotificationReceipt receipt) =>
        new(true, null, null, payment, queuePosition, receipt);
}
// end::facade-dtos[]
