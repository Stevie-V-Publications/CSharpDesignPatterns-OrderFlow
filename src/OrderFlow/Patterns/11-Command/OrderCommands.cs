namespace OrderFlow.Patterns.Command;

// tag::concrete-commands[]
/// <summary>Add a line item. Undo removes the one we added.</summary>
public sealed class AddLineItemCommand(OrderTicket ticket, string item) : IOrderCommand
{
    public string Description => $"Add \"{item}\"";

    public void Execute() => ticket.LineItems.Add(item);

    public void Undo()
    {
        // Remove the last matching entry, not every copy — Execute added
        // exactly one.
        var index = ticket.LineItems.LastIndexOf(item);
        if (index >= 0)
            ticket.LineItems.RemoveAt(index);
    }
}

/// <summary>
/// Comp (remove) a line item. Undo has to put it back where it was, so
/// Execute captures the index.
/// </summary>
public sealed class RemoveLineItemCommand(OrderTicket ticket, string item) : IOrderCommand
{
    private int _removedFrom = -1;

    public string Description => $"Comp \"{item}\"";

    public void Execute()
    {
        _removedFrom = ticket.LineItems.IndexOf(item);
        if (_removedFrom >= 0)
            ticket.LineItems.RemoveAt(_removedFrom);
    }

    public void Undo()
    {
        if (_removedFrom >= 0)
            ticket.LineItems.Insert(_removedFrom, item);
    }
}

/// <summary>Set the tip. Undo restores whatever it was before.</summary>
public sealed class ApplyTipCommand(OrderTicket ticket, decimal amount) : IOrderCommand
{
    private decimal _previous;

    public string Description => $"Apply tip {amount:C}";

    public void Execute()
    {
        _previous = ticket.Tip;
        ticket.Tip = amount;
    }

    public void Undo() => ticket.Tip = _previous;
}

/// <summary>Cancel the order outright. Fully reversible here.</summary>
public sealed class CancelOrderCommand(OrderTicket ticket) : IOrderCommand
{
    public string Description => "Cancel order";

    public void Execute() => ticket.Cancel();

    public void Undo() => ticket.Restore();
}
// end::concrete-commands[]
