using OrderFlow.Patterns.Adapter.ThirdParty;

namespace OrderFlow.Patterns.Adapter;

/// <summary>
/// Adapts <see cref="PayPalLikeClient"/> to <see cref="IPaymentProcessor"/>.
/// PayPal wants a <c>double</c> total and reports outcome as a "state"
/// string ("approved" / "declined"). The adapter absorbs both quirks.
/// </summary>
public sealed class PayPalPaymentAdapter(PayPalLikeClient payPal) : IPaymentProcessor
{
    public string ProviderName => "PayPal";

    public PaymentResult Charge(PaymentRequest request)
    {
        var execution = payPal.ExecutePayment(
            payerToken: request.PaymentToken,
            totalAmount: (double)request.Amount,
            currencyCode: request.Currency.ToUpperInvariant());

        return execution.State == "approved"
            ? PaymentResult.Ok(ProviderName, execution.TransactionId)
            : PaymentResult.Failed(ProviderName, $"PayPal payment state was '{execution.State}'.");
    }
}
