namespace ECommerceOS.AuthService.Application.Identity.Query.User;

public class UserQueryHandler(
    IUserRepository userRepository) 
    : IQueryHandler<UserQuery, UserQueryResponse>
{
    public async Task<Result<UserQueryResponse>> Handle(
        UserQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

        return user.Match(
            success => Result<UserQueryResponse>
                .Success(new UserQueryResponse(success.Email, success.Name)),
            Result<UserQueryResponse>.Failure);
    }
}