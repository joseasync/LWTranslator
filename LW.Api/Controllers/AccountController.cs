using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LW.Api.Controllers
{
    [Authorize]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        public AccountController(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [Authorize]
        [HttpPost("account/logout")]
        public async Task<IActionResult> Logout()
        {
            
                await _signInManager.SignOutAsync();
            
            return Ok();
        }
    }
}
