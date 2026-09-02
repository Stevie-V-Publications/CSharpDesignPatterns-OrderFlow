namespace OrderFlow.Patterns.Adapter.ThirdParty;

// These types stand in for three real payment SDKs. The point is that
// OrderFlow does NOT own them: it can't make them share an interface,
// rename a method, or agree on how to represent money. Pretend each of
// these came from a different NuGet package. (Amounts here always
// "succeed" unless the token is the literal string "decline".)

// --- "Stripe" ---------------------------------------------------------
public sealed class StripeLikeClient
{
    public StripeChargeResponse CreateCharge(long amountInCents, string currency, string source) =>
        new(
            Id: $"ch_{Guid.NewGuid():N}"[..24],
            Status: source == "decline" ? "failed" : "succeeded",
            AmountCents: amountInCents,
            Currency: currency);
}

public record StripeChargeResponse(string Id, string Status, long AmountCents, string Currency);

// --- "Square" --------------------------------------------------------
// tag::square-sdk[]
public sealed class SquareLikeClient
{
    public SquarePaymentResult Pay(SquareMoney money, string nonce) => new(
        Approved: nonce != "decline",
        PaymentId: $"sq:{Guid.NewGuid():N}"[..20],
        DeclineReason: nonce == "decline" ? "CARD_DECLINED" : null);
}

public readonly record struct SquareMoney(decimal Amount, string CurrencyCode);

public record SquarePaymentResult(bool Approved, string PaymentId, string? DeclineReason);
// end::square-sdk[]

// --- "PayPal" -------------------------------------------------------
public sealed class PayPalLikeClient
{
    // PayPal-style: you create an order elsewhere, then execute it here.
    public PayPalExecution ExecutePayment(string payerToken, double totalAmount, string currencyCode) =>
        new(
            State: payerToken == "decline" ? "declined" : "approved",
            TransactionId: $"PAYID-{Guid.NewGuid():N}"[..18].ToUpperInvariant());
}

public record PayPalExecution(string State, string TransactionId);
