using ECommerceOS.PaymentService.Domain.Payments;
using ECommerceOS.Shared.ValueObjects;
using Xunit;

namespace ECommerceOS.PaymentService.Domain.Tests.Payments;

public class PaymentTests
{
    [Fact]
    public void Create_ShouldInitializeAndRaisePaymentAddedEvent()
    {
        var userId = new UserId(Guid.NewGuid());
        var metadata = BuildPaymentMetadata();

        var result = Payment.Create(userId, metadata);

        Assert.True(result.IsSuccess);
        var payment = Assert.IsType<Payment>(result.Value);
        Assert.Equal(PaymentStatus.Pending, payment.PaymentStatus);
        Assert.Equal(metadata.PaymentMethod, payment.Name);
        Assert.Equal(metadata, payment.PaymentMetadata);

        var @event = Assert.IsType<PaymentAddedDomainEvent>(Assert.Single(payment.DomainEvents));
        Assert.Equal(payment.Id, @event.PaymentId);
        Assert.Equal(userId, @event.UserId);
        Assert.Equal(metadata, @event.PaymentMetadata);
    }

    [Fact]
    public void ChangePaymentStatus_WithInvalidStatus_ShouldFailAndNotRaiseStatusChangedEvent()
    {
        var payment = CreatePayment();

        var result = payment.ChangePaymentStatus("NotARealStatus");

        Assert.False(result.IsSuccess);
        Assert.Equal(PaymentStatus.Pending, payment.PaymentStatus);
        Assert.DoesNotContain(payment.DomainEvents, e => e is PaymentStatusChangedDomainEvent);
    }

    [Fact]
    public void ChangePaymentStatus_WithValidStatus_ShouldUpdateAndRaiseStatusChangedEvent()
    {
        var payment = CreatePayment();

        var result = payment.ChangePaymentStatus(nameof(PaymentStatus.Validated));

        Assert.True(result.IsSuccess);
        Assert.Equal(PaymentStatus.Validated, payment.PaymentStatus);

        var @event = Assert.IsType<PaymentStatusChangedDomainEvent>(
            payment.DomainEvents.Last(e => e is PaymentStatusChangedDomainEvent));
        Assert.Equal(payment.Id, @event.PaymentId);
        Assert.Equal(payment.UserId, @event.UserId);
        Assert.Equal(PaymentStatus.Validated, @event.PaymentStatus);
    }

    private static Payment CreatePayment()
    {
        var result = Payment.Create(new UserId(Guid.NewGuid()), BuildPaymentMetadata());
        return Assert.IsType<Payment>(result.Value);
    }

    private static PaymentMetadata BuildPaymentMetadata()
    {
        return new FakePaymentMetadata(
            "pm_metadata_1",
            "Card",
            new PaymentId(Guid.NewGuid()),
            "Stripe");
    }

    private sealed class FakePaymentMetadata : PaymentMetadata
    {
        public FakePaymentMetadata(string id, string paymentMethod, PaymentId paymentId, string type)
        {
            Id = id;
            PaymentMethod = paymentMethod;
            PaymentId = paymentId;
            Type = type;
        }
    }
}
