using System.Net;
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
            UserDto useDto;
            Console.WriteLine(model.Email + "  " + model.Password);
            try
            {
                var authResult = await _authManager.Authenticate(model.Email, model.Password);
                if (authResult.IsSuccess)
                {
                    // Get the full user data including related routes, groups, etc.
        var userDto = await _userManager.GetFullUserData(model.Email);
        userDto.Token = authResult.Token;  // attach the token

        // Optionally, if you want to separately return additional data that isn’t part of the user DTO,
        // you could do so here. But with the above mapping, all data is already in userDto.
        return Ok(userDto);
                }

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
                await _authManager.SendVerificationEmail(userResult.Result.Id);
                return Ok(new { User = userResult.Result, Token = userResult.Result.Token });
            }
            else
                return BadRequest(new { ErrorMessage = userResult.ErrorMessage });
        }

        [HttpGet("verify")]
        public async Task<IActionResult> VerifyToken(string token, int userId)
        {
            try
            {
                var result = await _authManager.ValidateToken(token, userId);
                if (result)

                {
                    var htmlContent = System.IO.File.ReadAllText("C:/src/.net core/motoMeet/Views/VerificationSuccess.html");
                    return new ContentResult
                    {
                        ContentType = "text/html",
                        StatusCode = (int)HttpStatusCode.OK,
                        Content = htmlContent
                    };
                }
                else
                    return BadRequest("Invalid or expired token.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { ErrorMessage = $"An error occurred: {ex.Message}" });
            }
        }


    }


}