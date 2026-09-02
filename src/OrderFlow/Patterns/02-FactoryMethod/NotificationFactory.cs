using OrderFlow.Domain;

namespace OrderFlow.Patterns.FactoryMethod;

/// <summary>
/// The Creator. Its single job is the <c>which concrete class?</c>
/// decision — every other class in the app asks this for an
/// <see cref="INotifier"/> and never names <see cref="EmailNotifier"/>
/// and friends itself.
///
/// Without this, every place that sends a notification (checkout,
/// dispatch, delivery updates) grows its own copy of the same
/// <c>switch (customer.PreferredNotification)</c>, and adding a channel
/// means hunting down all of them. With it, a new channel is one new
/// class plus one new arm here.
/// </summary>
public class NotificationFactory
{
    // tag::notification-factory[]
    /// <summary>
    /// The factory method. <c>virtual</c> on purpose: a test run or a
    /// regional deployment can subclass <see cref="NotificationFactory"/>
    /// and swap in a fake SMS gateway or an extra channel without any
    /// calling code changing — callers only ever see <see cref="INotifier"/>.
    /// </summary>
    public virtual INotifier Create(NotificationPreference preference) => preference switch
    {
        NotificationPreference.Email => new EmailNotifier(),
        NotificationPreference.Sms => new SmsNotifier(),
        NotificationPreference.Push => new PushNotifier(),
        _ => throw new ArgumentOutOfRangeException(
            nameof(preference), preference, "No notifier is registered for this preference."),
    };
    // end::notification-factory[]

    // tag::notification-factory-for[]
    /// <summary>
    /// Convenience overload: the common case is "notify this customer
    /// however they asked to be notified."
    /// </summary>
    public INotifier CreateFor(Customer customer) => Create(customer.PreferredNotification);
    // end::notification-factory-for[]
}
