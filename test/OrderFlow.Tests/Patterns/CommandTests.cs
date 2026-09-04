using OrderFlow.Patterns.Command;
using Xunit;

namespace OrderFlow.Tests.Patterns;

/// <summary>
/// Bonus Pattern: Command. Undo and the audit trail are written once, in
/// the invoker; each command only knows how to reverse itself. The naive
/// per-handler <c>_lastAction</c> gives you one level of undo and audit
/// strings that drift — neither is true here.
/// </summary>
public class CommandTests
{
    private static OrderTicket Ticket() =>
        new("order-1", ["Margherita Pizza", "Caesar Salad"]);

    // tag::command-undo[]
    [Fact]
    public void Undo_reverses_the_last_command_and_leaves_an_audit_entry()
    {
        var ticket = Ticket();
        var invoker = new OrderCommandInvoker();

        invoker.Do(new AddLineItemCommand(ticket, "Garlic Knots"));
        Assert.Equal(3, ticket.LineItems.Count);

        invoker.Undo();
        Assert.Equal(2, ticket.LineItems.Count);

        Assert.Collection(invoker.AuditTrail,
            e => Assert.Equal("Did", e.Verb),
            e => Assert.Equal("Undid", e.Verb));
    }

    [Fact]
    public void Comping_an_item_then_undoing_puts_it_back_in_its_original_position()
    {
        var ticket = Ticket();   // [Margherita Pizza, Caesar Salad]
        var invoker = new OrderCommandInvoker();

        invoker.Do(new RemoveLineItemCommand(ticket, "Margherita Pizza"));
        invoker.Undo();

        Assert.Equal(0, ticket.LineItems.IndexOf("Margherita Pizza"));
    }
    // end::command-undo[]

    [Fact]
    public void Undo_is_multi_level()
    {
        var ticket = Ticket();
        var invoker = new OrderCommandInvoker();

        invoker.Do(new ApplyTipCommand(ticket, 3m));
        invoker.Do(new ApplyTipCommand(ticket, 5m));
        Assert.Equal(5m, ticket.Tip);

        invoker.Undo();
        Assert.Equal(3m, ticket.Tip);
        invoker.Undo();
        Assert.Equal(0m, ticket.Tip);
    }

    [Fact]
    public void Undo_with_nothing_on_the_stack_is_a_no_op()
    {
        var invoker = new OrderCommandInvoker();

        invoker.Undo();

        Assert.False(invoker.CanUndo);
        Assert.Empty(invoker.AuditTrail);
    }
}
