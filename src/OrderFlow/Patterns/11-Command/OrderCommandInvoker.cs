namespace OrderFlow.Patterns.Command;

/// <summary>
/// The invoker: runs commands, keeps an undo stack, and appends every
/// action (and every undo) to an immutable audit trail. This is the only
/// class that knows commands can be undone — the concrete commands just
/// know how to reverse themselves.
/// </summary>
// tag::invoker[]
public sealed class OrderCommandInvoker
{
    private readonly Stack<IOrderCommand> _undoStack = new();
    private readonly List<AuditEntry> _audit = [];

    public IReadOnlyList<AuditEntry> AuditTrail => _audit;

    public bool CanUndo => _undoStack.Count > 0;

    public void Do(IOrderCommand command)
    {
        command.Execute();
        _undoStack.Push(command);
        _audit.Add(new AuditEntry(DateTimeOffset.Now, "Did", command.Description));
    }

    public void Undo()
    {
        if (_undoStack.Count == 0)
            return;

        var command = _undoStack.Pop();
        command.Undo();
        _audit.Add(new AuditEntry(DateTimeOffset.Now, "Undid", command.Description));
    }
}

/// <summary>One line of the audit trail. Append-only — an undo is a new
/// entry, never an erasure.</summary>
public record AuditEntry(DateTimeOffset At, string Verb, string Description);
// end::invoker[]
