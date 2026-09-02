namespace OrderFlow.Patterns.Command;

/// <summary>
/// The receiver in the Command pattern: a mutable, in-memory view of an
/// order that the admin dashboard edits. It's deliberately separate from
/// the immutable <c>Domain.Order</c> that the Builder produces --
/// commands need something they can change and change back.
/// </summary>
public sealed class OrderTicket
{
    public OrderTicket(string reference, IEnumerable<string> lineItems)
    {
        Reference = reference;
        LineItems = [.. lineItems];
    }

    public string Reference { get; }

    public List<string> LineItems { get; }

    public decimal Tip { get; set; }

    public bool IsCancelled { get; private set; }

    public void Cancel() => IsCancelled = true;

    public void Restore() => IsCancelled = false;

    public decimal Total(decimal perItem = 8.00m) =>
        IsCancelled ? 0m : LineItems.Count * perItem + Tip;
}
