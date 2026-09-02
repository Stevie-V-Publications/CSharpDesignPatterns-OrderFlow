namespace OrderFlow.Patterns.Observer;

/// <summary>
/// Pattern 8: Observer — the observer (subscriber) side of the contract.
///
/// Anything that needs to react to an order changing status implements
/// this: the kitchen display, the customer's live tracker, the dispatch
/// board. The publisher knows nothing about them beyond this one method,
/// so new screens can be added without touching the order code.
/// </summary>
// tag::observer-contract[]
public interface IOrderStatusObserver
{
    void OnOrderStatusChanged(OrderStatusChange change);
}
// end::observer-contract[]
