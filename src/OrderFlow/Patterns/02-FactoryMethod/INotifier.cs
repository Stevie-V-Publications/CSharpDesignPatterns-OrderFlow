using OrderFlow.Domain;

namespace OrderFlow.Patterns.FactoryMethod;

/// <summary>
/// Pattern 2: Factory Method.
///
/// The product the factory hands back. Every calling site in OrderFlow
/// that wants to tell a customer something ("your order is confirmed",
/// "your driver is 2 minutes away") depends only on this interface —
/// never on <see cref="EmailNotifier"/>, <see cref="SmsNotifier"/>, or
/// <see cref="PushNotifier"/> directly. That's what lets the "which
/// channel?" decision live in exactly one place (the factory) instead
/// of being re-decided with a <c>switch</c> at every call site.
/// </summary>
public interface INotifier
{
    NotificationPreference Channel { get; }

    // tag::notifier-contract[]
    /// <summary>
    /// Delivers <paramref name="message"/> to <paramref name="customer"/>
    /// over this notifier's channel and returns a receipt describing
    /// what was sent and where.
    /// </summary>
    NotificationReceipt Send(Customer customer, string message);
    // end::notifier-contract[]
}

/// <summary>
/// A record of one delivered notification — enough for the UI and the
/// (future) audit log to show what happened without re-sending anything.
/// </summary>
public record NotificationReceipt(
    NotificationPreference Channel,
    string SentTo,
    string Message,
    DateTimeOffset SentAt);
