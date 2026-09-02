using OrderFlow.Domain;

namespace OrderFlow.Patterns.FactoryMethod;

/// <summary>
/// Concrete product for mobile push. A real implementation would look
/// up the customer's registered device tokens; here we stand in with
/// the customer id so the demo has something concrete to show.
/// </summary>
public class PushNotifier : INotifier
{
    public NotificationPreference Channel => NotificationPreference.Push;

    public NotificationReceipt Send(Customer customer, string message) =>
        new(Channel, $"device:{customer.Id:N}", message, DateTimeOffset.UtcNow);
}
