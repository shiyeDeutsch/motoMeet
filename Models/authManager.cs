using motoMeet;

public class AuthManager
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;
    private readonly IMailingService _mailingService;
    public AuthManager(IAuthService authService, IUserService userService, IMailingService mailingService)
    {
        _userService = userService;
        _authService = authService;
        _mailingService = mailingService;
    }

    public async Task<AuthenticationResult> Authenticate(string email, string password)
    {
        return await _authService.AuthenticateAsync(email, password);
    }
    public async Task SendVerificationEmail(int userId)
    {
        var link = _authService.GenerateVerificationLink(userId);
        var user = await _userService.GetUser(userId);
        _mailingService.SendVerificationEmailAsync(user.Email, link);

    }

 
}