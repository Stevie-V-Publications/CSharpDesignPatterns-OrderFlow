using OrderFlow.Domain;
using OrderFlow.Services;

namespace OrderFlow.Data;

/// <summary>
/// Creates the SQLite schema and resyncs reference data at startup.
/// Kept as plain, boring infrastructure so the pattern chapters don't
/// have to explain it.
/// </summary>
public static class OrderFlowDbInitializer
{
    public static void Initialize(OrderFlowDbContext db)
    {
        db.Database.EnsureCreated();

        // Restaurants + menu items mirror SampleData exactly. They're
        // tiny and their in-memory GUIDs change per run, so the simplest
        // correct thing is to clear and re-add every startup.
        db.Restaurants.RemoveRange(db.Restaurants);
        db.MenuItems.RemoveRange(db.MenuItems);
        db.SaveChanges();

        db.Restaurants.Add(new Restaurant
        {
            Id = SampleData.Restaurant.Id,
            Name = SampleData.Restaurant.Name,
            Address = SampleData.Restaurant.Address,
        });

        foreach (var item in SampleData.MenuItems)
        {
            db.MenuItems.Add(new MenuItem
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                BasePrice = item.BasePrice,
                Category = item.Category,
            });
        }

        db.SaveChanges();
    }
}
