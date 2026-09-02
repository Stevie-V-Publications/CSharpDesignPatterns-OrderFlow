namespace OrderFlow.Patterns.Adapter;

/// <summary>
/// Pattern 4: Adapter.
///
/// The interface OrderFlow <em>wishes</em> every payment provider had.
/// Checkout depends only on this: one method, money in OrderFlow's own
/// terms, a plain yes/no result. The fact that Stripe wants cents and a
/// "source", Square wants a nonce and its own Money struct, and PayPal
/// speaks in "execute this order id" is not checkout's problem — each
/// provider gets an adapter that speaks this language on its behalf.
/// </summary>
public interface IPaymentProcessor
{
    string ProviderName { get; }

    // tag::processor-contract[]
    /// <summary>
    /// Attempts to charge <paramref name="request"/> and returns a
    /// provider-agnostic result. Implementations do not throw for a
    /// declined card — that's a <see cref="PaymentResult"/> with
    /// <see cref="PaymentResult.Succeeded"/> false, not an exception.
    /// </summary>
    PaymentResult Charge(PaymentRequest request);
    // end::processor-contract[]
}

// tag::payment-dtos[]
/// <summary>What OrderFlow knows about a payment, in its own terms.</summary>
public record PaymentRequest(
    decimal Amount,
    string Currency,
    string OrderReference,
    string PaymentToken);

/// <summary>The only shape checkout ever has to understand.</summary>
public record PaymentResult(
    bool Succeeded,
    string Provider,
    string? ProviderTransactionId,
    string? FailureReason)
{
    public static PaymentResult Ok(string provider, string transactionId) =>
        new(true, provider, transactionId, null);

    public static PaymentResult Failed(string provider, string reason) =>
        new(false, provider, null, reason);
}
// end::payment-dtos[]
