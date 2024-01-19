using motoMeet;

public class AuthManager
{
    private readonly IAuthService _authService;
    public AuthManager(IAuthService authService)
    {
        //   _userService = userService;
        _authService = authService;
    }

    public async Task<AuthenticationResult> Authenticate(string email, string password)
    {
        return await _authService.AuthenticateAsync(email, password);
    }
    
}