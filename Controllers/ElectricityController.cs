using Core.DTOs.Electricity;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PaymentUtility.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ElectricityController : ControllerBase
    {
        private readonly IElectricityService _electricityService;

        public ElectricityController(IElectricityService electricityService)
        {
            _electricityService = electricityService;
        }

        [HttpGet("providers")]
        public async Task<IActionResult> GetDiscoProviders()
        {
            var result = await _electricityService.GetDiscoProvidersAsync();
            return Ok(result);
        }

        [HttpPost("verify-meter")]
        public async Task<IActionResult> VerifyMeter([FromBody] VerifyMeterDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ElectricityResponseDto { Success = false, Message = "Invalid request" });

            var result = await _electricityService.VerifyMeterAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("purchase")]
        public async Task<IActionResult> PurchaseElectricity([FromBody] PurchaseElectricityDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ElectricityResponseDto { Success = false, Message = "Invalid request" });

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _electricityService.PurchaseElectricityAsync(userId, request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetElectricityHistory()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _electricityService.GetElectricityHistoryAsync(userId);
            return Ok(result);
        }
    }
}