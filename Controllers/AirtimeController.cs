using Core.DTOs.Airtime;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PaymentUtility.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AirtimeController : ControllerBase
    {
        private readonly IAirtimeService _airtimeService;

        public AirtimeController(IAirtimeService airtimeService)
        {
            _airtimeService = airtimeService;
        }

        [HttpGet("networks")]
        public async Task<IActionResult> GetNetworks()
        {
            var result = await _airtimeService.GetNetworksAsync();
            return Ok(result);
        }

        [HttpPost("purchase")]
        public async Task<IActionResult> PurchaseAirtime([FromBody] PurchaseAirtimeDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new AirtimeResponseDto { Success = false, Message = "Invalid request" });

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _airtimeService.PurchaseAirtimeAsync(userId, request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetAirtimeHistory()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _airtimeService.GetAirtimeHistoryAsync(userId);
            return Ok(result);
        }
    }
}
