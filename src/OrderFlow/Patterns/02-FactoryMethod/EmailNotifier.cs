using OrderFlow.Domain;

namespace OrderFlow.Patterns.FactoryMethod;

/// <summary>
/// Concrete product. In a real deployment this would wrap an email
/// provider SDK (SendGrid, SES, ...) — see Pattern 4 (Adapter) for how
/// OrderFlow keeps a third-party SDK behind an interface. Here it just
/// records what it would have sent so the pattern stays the focus.
/// </summary>
public class EmailNotifier : INotifier
{
    public NotificationPreference Channel => NotificationPreference.Email;

    public NotificationReceipt Send(Customer customer, string message) =>
        new(Channel, customer.Email, message, DateTimeOffset.UtcNow);
}
