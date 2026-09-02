using System.Threading;

namespace OrderFlow.Patterns.Observer;

/// <summary>
/// Pattern 8: Observer — the subject (publisher) side.
///
/// The order lifecycle code calls <see cref="Publish"/> once. Every
/// screen that called <see cref="Subscribe"/> hears about it, in the
/// order it subscribed. The publisher has no compile-time knowledge of
/// what a "kitchen display" or a "dispatch board" is — it only holds
/// <see cref="IOrderStatusObserver"/> references.
///
/// Without this, the code that changes an order's status would grow a
/// hard-coded call list — <c>kitchenScreen.Refresh(); customerTracker
/// .Refresh(); dispatchBoard.Refresh();</c> — and every new surface would
/// mean editing the order code again.
///
/// In the running app this same idea is pushed across the network by
/// SignalR (Blazor Server): the "observers" are browser circuits, and
/// the hub broadcast is the notify loop.
/// </summary>
// tag::publish-status[]
public sealed class OrderStatusPublisher
{
    private readonly List<IOrderStatusObserver> _observers = [];
    private readonly Lock _gate = new();

    /// <summary>
    /// Register an observer. The returned token unsubscribes when
    /// disposed, so a Blazor component can subscribe in
    /// <c>OnInitialized</c> and drop off cleanly in <c>Dispose</c>
    /// without the publisher leaking a reference to a dead circuit.
    /// </summary>
    public IDisposable Subscribe(IOrderStatusObserver observer)
    {
        lock (_gate)
            _observers.Add(observer);

        return new Subscription(this, observer);
    }

    public void Publish(OrderStatusChange change)
    {
        // Snapshot under the lock, then notify outside it: an observer's
        // handler must never be able to deadlock (or re-enter Subscribe)
        // while we hold the gate.
        IOrderStatusObserver[] current;
        lock (_gate)
            current = [.. _observers];

        foreach (var observer in current)
            observer.OnOrderStatusChanged(change);
    }

    public int ObserverCount
    {
        get { lock (_gate) return _observers.Count; }
    }

    private void Unsubscribe(IOrderStatusObserver observer)
    {
        lock (_gate)
            _observers.Remove(observer);
    }

    private sealed class Subscription(OrderStatusPublisher publisher, IOrderStatusObserver observer) : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            publisher.Unsubscribe(observer);
        }
    }
}
// end::publish-status[]
