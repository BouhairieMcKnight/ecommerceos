using MassTransit;

namespace ECommerceOS.AuthService.Infrastructure.Saga;

public class OnBoardingStateMachine : MassTransitStateMachine<OnBoardingState>
{
    public OnBoardingStateMachine()
    {
        Event(() => UserRegistered, x =>
        {
            x.CorrelateById(context => context.Message.UserId!.Value);
            x.SelectId(context => context.Message.UserId!.Value);
        });
        Event(() => WelcomeEmail, x => x.CorrelateById(context => context.Message.UserId.Value));
        Event(() => UserVerified, x => x.CorrelateById(context => context.Message.UserId!.Value));
        Event(() => VerifyUser, x => x.CorrelateById(context => context.Message.UserId.Value));

        InstanceState(x => x.CurrentState);

        Initially(
            When(UserRegistered)
                .Then(context =>
                {
                    context.Saga.UserId = context.Message.UserId!;
                    context.Saga.Email = context.Message.Email;
                    context.Saga.Name = context.Message.Name;
                    context.Saga.RegisteredAt = context.Message.CreatedAt;
                    context.Saga.IsVerified = false;
                })
                .PublishAsync(context => context.Init<VerifyUser>(new VerifyUser
                {
                    UserId = context.Message.UserId!,
                    EmailAddress = context.Message.Email,
                    Name = context.Message.Name,
                }))
                .TransitionTo(Verifying)
        );

        During(Verifying,
            Ignore(UserRegistered),
            When(UserVerified)
                .PublishAsync(context => context.Init<WelcomeUser>(new WelcomeUser
                {
                    UserId = context.Message.UserId!,
                    EmailAddress = context.Message.Email,
                    Name = context.Message.Name
                }))
                .Finalize()
        );

        SetCompletedWhenFinalized();
    }
    
    
    public State? Verifying { get; private set; }
    
    public Event<VerifyUser> VerifyUser { get; private set; }
    public Event<WelcomeUser> WelcomeEmail { get; private set; }
    public Event<UserRegistered>? UserRegistered { get; private set; }
    public Event<UserEmailVerified>? UserVerified { get; private set; }
}
