using Core.DTOs.CableTV;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PaymentUtility.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CableTvController : ControllerBase
    {
        private readonly ICableTvService _cableTvService;

        public CableTvController(ICableTvService cableTvService)
        {
            _cableTvService = cableTvService;
        }

        [HttpGet("providers")]
        public async Task<IActionResult> GetProviders()
        {
            var result = await _cableTvService.GetProvidersAsync();
            return Ok(result);
        }

        [HttpGet("packages/{provider}")]
        public async Task<IActionResult> GetPackages(string provider)
        {
            var result = await _cableTvService.GetPackagesAsync(provider);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] SubscribeCableDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new CableTvResponseDto { Success = false, Message = "Invalid request" });

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _cableTvService.SubscribeAsync(userId, request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetCableTvHistory()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _cableTvService.GetCableTvHistoryAsync(userId);
            return Ok(result);
        }
    }
}
