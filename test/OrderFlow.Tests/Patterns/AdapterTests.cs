using OrderFlow.Patterns.Adapter;
using OrderFlow.Patterns.Adapter.ThirdParty;
using Xunit;

namespace OrderFlow.Tests.Patterns;

/// <summary>
/// Pattern 4: Adapter. Every adapter is tested the same way through the
/// one interface OrderFlow owns — <see cref="IPaymentProcessor"/> — even
/// though the three SDKs behind them share no type, no method name, and
/// no money representation. That uniformity is the whole point.
/// </summary>
public class AdapterTests
{
    public static IEnumerable<object[]> AllAdapters()
    {
        yield return [new StripePaymentAdapter(new StripeLikeClient()), "Stripe"];
        yield return [new SquarePaymentAdapter(new SquareLikeClient()), "Square"];
        yield return [new PayPalPaymentAdapter(new PayPalLikeClient()), "PayPal"];
    }

    // tag::adapter-uniform-result[]
    [Theory]
    [MemberData(nameof(AllAdapters))]
    public void Every_adapter_reports_success_the_same_way(IPaymentProcessor processor, string name)
    {
        var result = processor.Charge(new PaymentRequest(28.50m, "USD", "order-1", "tok_ok"));

        Assert.True(result.Succeeded);
        Assert.Equal(name, result.Provider);
        Assert.NotNull(result.ProviderTransactionId);
        Assert.Null(result.FailureReason);
    }

    [Theory]
    [MemberData(nameof(AllAdapters))]
    public void A_declined_card_is_a_result_not_an_exception(IPaymentProcessor processor, string name)
    {
        var result = processor.Charge(new PaymentRequest(28.50m, "USD", "order-1", "decline"));

        Assert.False(result.Succeeded);
        Assert.Equal(name, result.Provider);
        Assert.NotNull(result.FailureReason);
    }
    // end::adapter-uniform-result[]

    [Fact]
    public void Checkout_code_only_ever_sees_the_target_interface()
    {
        // The compiler proof: this method names no SDK type. Swapping
        // providers is choosing a different element of this collection.
        IReadOnlyList<IPaymentProcessor> processors =
        [
            new StripePaymentAdapter(new StripeLikeClient()),
            new SquarePaymentAdapter(new SquareLikeClient()),
            new PayPalPaymentAdapter(new PayPalLikeClient()),
        ];

        foreach (var processor in processors)
        {
            var result = processor.Charge(new PaymentRequest(10m, "USD", "o", "tok_ok"));
            Assert.True(result.Succeeded);
        }
    }
}
