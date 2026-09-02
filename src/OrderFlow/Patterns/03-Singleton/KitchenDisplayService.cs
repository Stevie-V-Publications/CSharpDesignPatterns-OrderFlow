using System.Collections.Concurrent;
using OrderFlow.Domain;

namespace OrderFlow.Patterns.Singleton;

/// <summary>
/// Pattern 3: Singleton.
///
/// The kitchen queue is shared, mutable, in-memory state that every
/// surface in the app must agree on: the moment checkout routes an
/// order to the kitchen, the line cook's display, the expo screen, and
/// the admin dispatch board all need to see the same ticket in the
/// same position. If any two of them held their own queue instance,
/// they'd disagree the first time a ticket was bumped.
///
/// So there is exactly one <see cref="KitchenDisplayService"/> for the
/// whole process, and it owns one <see cref="KitchenQueue"/> per
/// restaurant. The constructor is private; the only way to reach it is
/// <see cref="Instance"/>.
/// </summary>
public sealed class KitchenDisplayService
{
    // tag::singleton-instance[]
    private static readonly Lazy<KitchenDisplayService> _instance =
        new(() => new KitchenDisplayService());

    /// <summary>The one and only instance. Thread-safe and lazy: the
    /// <see cref="Lazy{T}"/> guarantees the constructor runs at most
    /// once even if several threads race to first-touch it.</summary>
    public static KitchenDisplayService Instance => _instance.Value;

    // Private: no other code can call `new KitchenDisplayService()`.
    private KitchenDisplayService() { }
    // end::singleton-instance[]

    private readonly ConcurrentDictionary<Guid, KitchenQueue> _queues = new();

    // tag::queue-for[]
    /// <summary>
    /// The live queue for one restaurant's kitchen — created on first
    /// request, then the same instance forever after. Two callers
    /// asking for the same restaurant get the same object back.
    /// </summary>
    public KitchenQueue QueueFor(Restaurant restaurant) =>
        _queues.GetOrAdd(restaurant.Id, _ => new KitchenQueue(restaurant));
    // end::queue-for[]

    public int RestaurantCount => _queues.Count;
}
