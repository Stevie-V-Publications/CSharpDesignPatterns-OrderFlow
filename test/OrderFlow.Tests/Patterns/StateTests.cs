using OrderFlow.Domain;
using OrderFlow.Patterns.Builder;
using OrderFlow.Patterns.State;
using Xunit;

namespace OrderFlow.Tests.Patterns;

/// <summary>
/// Pattern 9: State. Legal transitions live in the state classes, and an
/// illegal one is the <em>absence</em> of an override — so it fails
/// loudly through the base class instead of falling through a missing
/// <c>switch</c> arm and silently doing nothing.
/// </summary>
public class StateTests
{
    private static Order NewOrder() =>
        new OrderBuilder()
            .ForCustomer(Sample.Customer())
            .FromRestaurant(Sample.Restaurant())
            .AddItem(Sample.OrderItem())
            .AsPickup()
            .Build();

    // tag::state-lifecycle[]
    [Fact]
    public void Advance_walks_the_lifecycle_one_state_at_a_time()
    {
        var machine = new OrderStateMachine(NewOrder());

        Assert.Equal(OrderStatus.Placed, machine.Current.Status);
        machine.Advance();
        Assert.Equal(OrderStatus.Confirmed, machine.Current.Status);
        machine.Advance();
        Assert.Equal(OrderStatus.Preparing, machine.Current.Status);
    }

    [Fact]
    public void An_order_on_the_line_can_no_longer_be_cancelled()
    {
        var machine = new OrderStateMachine(NewOrder());
        machine.Advance();  // Confirmed
        machine.Advance();  // Preparing

        Assert.False(machine.Current.CanCancel);
        Assert.Throws<InvalidOperationException>(() => machine.Cancel());
    }
    // end::state-lifecycle[]

    [Theory]
    [InlineData(0)]  // Placed
    [InlineData(1)]  // Confirmed
    public void A_freshly_placed_or_confirmed_order_can_be_cancelled(int advances)
    {
        var machine = new OrderStateMachine(NewOrder());
        for (var i = 0; i < advances; i++) machine.Advance();

        machine.Cancel();

        Assert.Equal(OrderStatus.Cancelled, machine.Current.Status);
    }

    [Fact]
    public void The_machine_keeps_the_plain_status_label_in_lockstep()
    {
        var order = NewOrder();
        var machine = new OrderStateMachine(order);

        machine.Advance();

        Assert.Equal(OrderStatus.Confirmed, order.Status);
    }

    [Fact]
    public void ForStatus_rehydrates_the_matching_state_object()
    {
        Assert.IsType<ReadyState>(OrderStateMachine.ForStatus(OrderStatus.Ready));
    }

    [Fact]
    public void A_terminal_state_refuses_to_advance()
    {
        var completed = OrderStateMachine.ForStatus(OrderStatus.Completed);

        Assert.Throws<InvalidOperationException>(() => completed.Advance());
    }
}
