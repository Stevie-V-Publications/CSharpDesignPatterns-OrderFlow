using OrderFlow.Patterns.Adapter.ThirdParty;

namespace OrderFlow.Patterns.Adapter;

/// <summary>
/// Adapts <see cref="StripeLikeClient"/> to <see cref="IPaymentProcessor"/>.
/// All the Stripe-specific awkwardness — money in integer cents, a
/// "source" parameter, a string status field — is contained here and
/// nowhere else.
/// </summary>
public sealed class StripePaymentAdapter(StripeLikeClient stripe) : IPaymentProcessor
{
    public string ProviderName => "Stripe";

    // tag::stripe-adapter[]
    public PaymentResult Charge(PaymentRequest request)
    {
        // Translate OrderFlow's request into what Stripe expects...
        var response = stripe.CreateCharge(
            amountInCents: (long)Math.Round(request.Amount * 100m),
            currency: request.Currency.ToLowerInvariant(),
            source: request.PaymentToken);

        // ...and translate Stripe's response back into OrderFlow's shape.
        return response.Status == "succeeded"
            ? PaymentResult.Ok(ProviderName, response.Id)
            : PaymentResult.Failed(ProviderName, $"Stripe returned status '{response.Status}'.");
    }
    // end::stripe-adapter[]
}
