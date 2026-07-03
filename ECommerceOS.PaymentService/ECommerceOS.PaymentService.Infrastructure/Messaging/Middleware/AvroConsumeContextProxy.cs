using MassTransit;
using MassTransit.Context;

namespace ECommerceOS.PaymentService.Infrastructure.Messaging.Middleware;

public class AvroConsumeContextProxy<TAvro>(ConsumeContext context, Func<TAvro, object> propertySelector)
    :
        ConsumeContextProxy(context)
    where TAvro : class
{
    public override bool TryGetMessage<T>(out ConsumeContext<T> consumeContext)
    {
        if (base.TryGetMessage(out consumeContext))
            return true;

        if (base.TryGetMessage<TAvro>(out ConsumeContext<TAvro>? messageContext))
        {
            var messageProperty = propertySelector(messageContext.Message);
            if (messageProperty is T message)
            {
                consumeContext = new MessageConsumeContext<T>(this, message);
                return true;
            }
        }

        return false;
    }
}