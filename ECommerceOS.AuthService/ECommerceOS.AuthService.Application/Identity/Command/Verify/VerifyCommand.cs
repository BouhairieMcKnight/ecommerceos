namespace ECommerceOS.AuthService.Application.Identity.Command.Verify;

public class VerifyCommand : ICommand
{
    public UserId UserId { get; set; }
    public string Email { get; set; }
}