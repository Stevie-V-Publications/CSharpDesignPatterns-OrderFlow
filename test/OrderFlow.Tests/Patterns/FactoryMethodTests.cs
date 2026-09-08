using OrderFlow.Domain;
using OrderFlow.Patterns.FactoryMethod;
using Xunit;

namespace OrderFlow.Tests.Patterns;

/// <summary>
/// Pattern 2: Factory Method. The naive per-call-site <c>switch</c> is
/// untestable in isolation — you'd assert against a real gateway. Here
/// the decision lives in one overridable method, so a test (or a
/// region) can substitute a fake and the calling code never knows.
/// </summary>
public class FactoryMethodTests
{
    [Theory]
    [InlineData(NotificationPreference.Email, typeof(EmailNotifier))]
    [InlineData(NotificationPreference.Sms, typeof(SmsNotifier))]
    [InlineData(NotificationPreference.Push, typeof(PushNotifier))]
    public void Create_maps_each_preference_to_its_concrete_notifier(
        NotificationPreference preference, Type expected)
    {
        var notifier = new NotificationFactory().Create(preference);

        Assert.IsType(expected, notifier);
        Assert.Equal(preference, notifier.Channel);
    }

    [Fact]
    public void CreateFor_reads_the_channel_off_the_customer()
    {
        var customer = Sample.Customer(NotificationPreference.Push);

        var notifier = new NotificationFactory().CreateFor(customer);

        Assert.IsType<PushNotifier>(notifier);
    }

    [Fact]
    public void Sms_notifier_fails_loudly_when_the_customer_has_no_phone()
    {
        var notifier = new NotificationFactory().Create(NotificationPreference.Sms);
        var customer = Sample.Customer(NotificationPreference.Sms, phone: null);

        Assert.Throws<InvalidOperationException>(() => notifier.Send(customer, "hi"));
    }

    // tag::factory-method-substitution[]
    private sealed class RecordingNotifier : INotifier
    {
        public NotificationPreference Channel => NotificationPreference.Email;
        public string? LastMessage { get; private set; }

        public NotificationReceipt Send(Customer customer, string message)
        {
            LastMessage = message;
            return new NotificationReceipt(Channel, customer.Email, message, DateTimeOffset.UtcNow);
        }
    }

    /// <summary>A test-only Creator: override the one factory method and
    /// every caller that depends on <see cref="INotifier"/> now talks to
    /// the fake, with no change to the calling code.</summary>
    private sealed class StubFactory(RecordingNotifier stub) : NotificationFactory
    {
        public override INotifier Create(NotificationPreference preference) => stub;
    }

    [Fact]
    public void A_subclassed_factory_substitutes_a_fake_without_touching_callers()
    {
        var stub = new RecordingNotifier();
        NotificationFactory factory = new StubFactory(stub);

        // This line is exactly what the checkout code runs.
        factory.CreateFor(Sample.Customer()).Send(Sample.Customer(), "Order confirmed!");

        Assert.Equal("Order confirmed!", stub.LastMessage);
    }
    // end::factory-method-substitution[]
}
