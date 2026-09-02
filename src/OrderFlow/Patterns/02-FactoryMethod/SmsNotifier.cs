using OrderFlow.Domain;

namespace OrderFlow.Patterns.FactoryMethod;

/// <summary>
/// Concrete product for SMS. Note it needs a phone number: if the
/// customer picked SMS but never gave one, that's a data problem the
/// factory surfaces early rather than a null reference deep inside a
/// send call.
/// </summary>
// tag::sms-notifier[]
public class SmsNotifier : INotifier
{
    public NotificationPreference Channel => NotificationPreference.Sms;

    public NotificationReceipt Send(Customer customer, string message)
    {
        if (string.IsNullOrWhiteSpace(customer.Phone))
            throw new InvalidOperationException(
                $"Customer '{customer.Name}' prefers SMS but has no phone number on file.");

        return new NotificationReceipt(Channel, customer.Phone, message, DateTimeOffset.UtcNow);
    }
}
// end::sms-notifier[]
