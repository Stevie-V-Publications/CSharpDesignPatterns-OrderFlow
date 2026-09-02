using OrderFlow.Domain;

namespace OrderFlow.Patterns.Builder;

/// <summary>
/// The classic GoF Builder pattern often pairs the Builder with a
/// "Director" that knows common recipes for assembling a product,
/// while the Builder itself only knows how to perform individual
/// steps. This keeps reusable construction sequences out of UI code.
///
/// Example recipe here: a one-click "reorder my usual" feature —
/// re-adds every item from a previous order to a new one, keeping the
/// same delivery/pickup choice, without the caller needing to know
/// the individual builder steps involved.
/// </summary>
public class OrderDirector(IOrderBuilder builder)
{
    // tag::reorder-recipe[]
    public Order BuildReorder(Order previousOrder)
    {
        builder
            .ForCustomer(previousOrder.Customer)
            .FromRestaurant(previousOrder.Restaurant);

        foreach (var item in previousOrder.Items)
        {
            builder.AddItem(item);
        }

        if (previousOrder.Type == OrderType.Delivery && previousOrder.DeliveryAddress is not null)
        {
            builder.AsDelivery(previousOrder.DeliveryAddress);
        }
        else
        {
            builder.AsPickup();
        }

        return builder.Build();
    }
    // end::reorder-recipe[]
}
