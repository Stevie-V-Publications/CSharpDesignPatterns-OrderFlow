namespace OrderFlow.Domain;

public class Customer
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Name { get; init; }
    public required string Email { get; init; }
    public string? Phone { get; init; }
    public Address? DefaultDeliveryAddress { get; init; }

    /// <summary>
    /// Used by the Factory Method pattern (Pattern 2) to decide which
    /// concrete INotifier to hand back. Lives here as plain data —
    /// the "which class do I build" decision belongs in the factory,
    /// not in this POCO.
    /// </summary>
    public NotificationPreference PreferredNotification { get; init; } = NotificationPreference.Email;
}

public enum NotificationPreference
{
    Email,
    Sms,
    Push
}
