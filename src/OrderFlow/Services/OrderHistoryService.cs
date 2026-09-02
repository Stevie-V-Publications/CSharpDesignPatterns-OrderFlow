using Microsoft.EntityFrameworkCore;
using OrderFlow.Data;
using OrderFlow.Domain;

namespace OrderFlow.Services;

/// <summary>
/// The one place the app writes to and reads from the database. Uses
/// <see cref="IDbContextFactory{TContext}"/> so each call gets a fresh,
/// short-lived context — the safe pattern for Blazor Server, where a
/// single long-lived scoped context would be shared across overlapping
/// renders.
/// </summary>
public sealed class OrderHistoryService(IDbContextFactory<OrderFlowDbContext> contextFactory)
{
    public async Task RecordAsync(Order order, string placedVia)
    {
        await using var db = await contextFactory.CreateDbContextAsync();

        db.PlacedOrders.Add(new PlacedOrderRecord
        {
            Id = order.Id,
            CustomerName = order.Customer.Name,
            RestaurantName = order.Restaurant.Name,
            Items = string.Join(", ", order.Items.Select(i => i.GetDescription())),
            Subtotal = order.Subtotal,
            Fulfillment = order.Type.ToString(),
            Status = order.Status.ToString(),
            PlacedAtUtc = order.CreatedAt.UtcDateTime,
            PlacedVia = placedVia,
        });

        await db.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<PlacedOrderRecord>> RecentAsync(int take = 10)
    {
        await using var db = await contextFactory.CreateDbContextAsync();

        return await db.PlacedOrders
            .OrderByDescending(o => o.PlacedAtUtc)
            .Take(take)
            .ToListAsync();
    }
}
