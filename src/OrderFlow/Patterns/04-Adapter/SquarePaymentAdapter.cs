using OrderFlow.Patterns.Adapter.ThirdParty;

namespace OrderFlow.Patterns.Adapter;

/// <summary>
/// Adapts <see cref="SquareLikeClient"/> to <see cref="IPaymentProcessor"/>.
/// Square keeps money as a decimal inside its own <c>SquareMoney</c>
/// struct and calls the token a "nonce" — the adapter knows that, the
/// rest of OrderFlow doesn't.
/// </summary>
public sealed class SquarePaymentAdapter(SquareLikeClient square) : IPaymentProcessor
{
    public string ProviderName => "Square";

    public PaymentResult Charge(PaymentRequest request)
    {
        var result = square.Pay(
            new SquareMoney(request.Amount, request.Currency.ToUpperInvariant()),
            nonce: request.PaymentToken);

        return result.Approved
            ? PaymentResult.Ok(ProviderName, result.PaymentId)
            : PaymentResult.Failed(ProviderName, result.DeclineReason ?? "Square declined the payment.");
    }
}
