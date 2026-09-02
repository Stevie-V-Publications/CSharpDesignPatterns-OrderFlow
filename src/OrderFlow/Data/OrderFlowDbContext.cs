using Microsoft.EntityFrameworkCore;
using OrderFlow.Domain;

namespace OrderFlow.Data;

/// <summary>
/// Deliberately small. Persistence is not the point of this book, so the
/// context holds only what the app actually reads back:
///
/// <list type="bullet">
///   <item><description><see cref="Restaurants"/> / <see cref="MenuItems"/> —
///   reference data, resynced from <c>SampleData</c> at startup.</description></item>
///   <item><description><see cref="PlacedOrders"/> — an append-only history
///   row written after each successful checkout.</description></item>
/// </list>
///
/// The schema is created with <c>EnsureCreated()</c> (no migrations on
/// purpose). If you add a table here, delete <c>orderflow.db</c> so it's
/// recreated.
/// </summary>
public class OrderFlowDbContext(DbContextOptions<OrderFlowDbContext> options)
    : DbContext(options)
{
    public DbSet<Restaurant> Restaurants => Set<Restaurant>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<PlacedOrderRecord> PlacedOrders => Set<PlacedOrderRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Restaurant>().OwnsOne(r => r.Address);
    }
}
