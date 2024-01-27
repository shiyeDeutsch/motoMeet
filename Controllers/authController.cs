using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace motoMeet
{
    [Route("api/auth")]
    [ApiController]

    public class LoginController : ControllerBase
    {
        private readonly AuthManager _authManager;
        private readonly UserManager _userManager;
        public LoginController(AuthManager authManager, UserManager userManager)
        {
            _authManager = authManager;
            _userManager = userManager;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Auth([FromBody] LoginModel model)
        {
            Console.WriteLine(model.Email + "  " + model.Password);
            try
            {
                var authResult = await _authManager.Authenticate(model.Email, model.Password);
                if (authResult.IsSuccess)
                    return Ok(new { jwt = authResult.Token, user = _userManager.GetUserData(model.Email) });
                return Unauthorized(authResult.ErrorMessage);
            }
            catch (Exception ex)
            {
                return BadRequest(new { ErrorMessage = $"An error occurred while processing the request: {ex.Message}" });
            }

        }


        [HttpPost("register")]
        public async Task<IActionResult> Registrate([FromBody] RegistrateModel model)
        {
            var userResult = await _userManager.CreateUser(model);
            if (userResult.IsSuccess)
            {
                _authManager.SendVerificationEmail(userResult.User.Id);
                return Ok(new { User = userResult.User, Token = userResult.Token });
            }
            else
                return BadRequest(new { ErrorMessage = userResult.ErrorMessage });
        }

    
    

    }


}