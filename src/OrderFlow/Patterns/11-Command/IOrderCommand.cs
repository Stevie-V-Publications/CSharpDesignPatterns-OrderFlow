namespace OrderFlow.Patterns.Command;

/// <summary>
/// Bonus Pattern: Command.
///
/// In the admin dashboard, a support agent adjusts a live order: add a
/// forgotten side, comp an item, apply a tip, cancel outright. Every one
/// of those actions needs to be undoable and needs to land in an audit
/// trail ("who did what, when").
///
/// Command turns each action into an object with two methods --
/// <see cref="Execute"/> and <see cref="Undo"/> -- plus enough captured
/// state to reverse itself. The dashboard (the invoker) just keeps a
/// stack of them; it never contains a giant switch on "action type".
/// </summary>
// tag::command-contract[]
public interface IOrderCommand
{
    /// <summary>Audit-log-friendly summary, e.g. "Add 'Garlic Knots'".</summary>
    string Description { get; }

    void Execute();

    /// <summary>Reverse <see cref="Execute"/>, using state captured at execute time.</summary>
    void Undo();
}
// end::command-contract[]
