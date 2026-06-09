using Core.DTOs.Data;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PaymentUtility.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DataController : ControllerBase
    {
        private readonly IDataService _dataService;

        public DataController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [HttpGet("networks")]
        public async Task<IActionResult> GetNetworks()
        {
            var result = await _dataService.GetNetworksAsync();
            return Ok(result);
        }

        [HttpGet("plans/{network}")]
        public async Task<IActionResult> GetDataPlans(string network)
        {
            var result = await _dataService.GetDataPlansAsync(network);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("purchase")]
        public async Task<IActionResult> PurchaseData([FromBody] PurchaseDataDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new DataResponseDto { Success = false, Message = "Invalid request" });

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _dataService.PurchaseDataAsync(userId, request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetDataHistory()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _dataService.GetDataHistoryAsync(userId);
            return Ok(result);
        }
    }
}
