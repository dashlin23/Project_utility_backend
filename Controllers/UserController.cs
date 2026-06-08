using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PaymentUtility.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { Success = false, Message = "Unauthorized" });

            var userId = int.Parse(userIdClaim);
            var result = await _userService.GetUserProfileAsync(userId);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { Success = false, Message = "Unauthorized" });

            var userId = int.Parse(userIdClaim);
            var result = await _userService.DeleteAccountAsync(userId);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
    }
}
