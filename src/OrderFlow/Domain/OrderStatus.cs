namespace OrderFlow.Domain;

public enum OrderType
{
    Delivery,
    Pickup
}

/// <summary>
/// Deliberately a plain enum for now. Pattern 9 (State) replaces the
/// naive "big switch statement on this enum" approach — used to drive
/// transition logic throughout the app — with an IOrderState hierarchy
/// that enforces legal transitions and encapsulates per-state behavior.
/// This enum still exists afterward purely as a simple readable label;
/// the *behavior* moves into the State pattern's classes.
/// </summary>
public enum OrderStatus
{
    Placed,
    Confirmed,
    Preparing,
    Ready,
    OutForDelivery,
    Completed,
    Cancelled
}
