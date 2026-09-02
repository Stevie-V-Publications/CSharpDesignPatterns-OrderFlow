using OrderFlow.Domain;

namespace OrderFlow.Patterns.Singleton;

/// <summary>
/// The actual FIFO queue of tickets for one restaurant's kitchen. The
/// Singleton (<see cref="KitchenDisplayService"/>) owns exactly one of
/// these per restaurant and hands the same instance to every caller,
/// so the line cook's display, the expo screen, and the admin board
/// are all looking at one queue — not three copies that drift apart.
/// </summary>
public sealed class KitchenQueue(Restaurant restaurant)
{
    private readonly Queue<KitchenTicket> _tickets = new();
    private readonly Lock _gate = new();

    public Restaurant Restaurant { get; } = restaurant;

    public void Enqueue(KitchenTicket ticket)
    {
        lock (_gate)
            _tickets.Enqueue(ticket);
    }

    /// <summary>
    /// Pulls the next ticket off the front ("that order is plated, next!").
    /// Returns false if the kitchen queue is already empty.
    /// </summary>
    public bool TryBumpNext(out KitchenTicket? bumped)
    {
        lock (_gate)
        {
            if (_tickets.Count == 0)
            {
                bumped = null;
                return false;
            }

            bumped = _tickets.Dequeue();
            return true;
        }
    }

    /// <summary>
    /// Pulls a specific ticket ("order 47's out the door") wherever it
    /// sits in the line, preserving the order of everything else.
    /// </summary>
    public bool Remove(Guid orderId)
    {
        lock (_gate)
        {
            var remaining = _tickets.Where(t => t.OrderId != orderId).ToArray();
            if (remaining.Length == _tickets.Count)
                return false;

            _tickets.Clear();
            foreach (var ticket in remaining)
                _tickets.Enqueue(ticket);
            return true;
        }
    }

    public IReadOnlyList<KitchenTicket> Snapshot()
    {
        lock (_gate)
            return _tickets.ToArray();
    }

    public int Count
    {
        get
        {
            lock (_gate)
                return _tickets.Count;
        }
    }
}
