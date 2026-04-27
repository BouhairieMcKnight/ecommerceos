using MassTransit;

namespace ECommerceOS.AuthService.Infrastructure.Saga;

public class OnBoardingState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public UserId UserId { get; set; } = new(Guid.Empty);
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public string CurrentState { get; set; } = string.Empty;
    public DateTimeOffset RegisteredAt { get; set; }
}
