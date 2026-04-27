using ECommerceOS.Shared.Contracts.Messaging.Identity;
using Mapster;

namespace ECommerceOS.AuthService.Application.Identity.Mapping;

public class UserRegisteredEventRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<UserRegisteredDomainEvent, UserRegistered>()
            .Map(integration => integration.CreatedAt, domain => domain.OccurredOn)
            .Map(integration => integration.UserId, domain => domain.UserId)
            .Map(integration => integration.Email, domain => domain.Email)
            .Map(integration => integration.Version, domain => 1);
    }
}