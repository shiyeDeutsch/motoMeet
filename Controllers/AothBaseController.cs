using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace motoMeet
{
    [Authorize] // Ensures all derived controllers require authentication
    public class AothBaseController : ControllerBase
    {
            private readonly UserManager _userManager;
             public AothBaseController(UserManager userManager)
        {
            _userManager = userManager;
        }
        protected async Task<Guid?> GetUserIdAsync( )
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(userIdString))
                {
                    return null;
                }

                if (!Guid.TryParse(userIdString, out var userId))
                {
                    return null;
                }

                var user = await _userManager.UserExists(userId);

                return user != null ? userId : null;
            }
            catch
            {
                return null;
            }
        }
    }
}
