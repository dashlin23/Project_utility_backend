using Core.DTOs.TransactionPin;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PaymentUtility.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionPinController : ControllerBase
    {
        private readonly ITransactionPinService _transactionPinService;

        public TransactionPinController(ITransactionPinService transactionPinService)
        {
            _transactionPinService = transactionPinService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePin([FromBody] CreatePinDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new PinResponseDto { Success = false, Message = "Invalid request" });

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _transactionPinService.CreatePinAsync(userId, request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("change")]
        public async Task<IActionResult> ChangePin([FromBody] ChangePinDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new PinResponseDto { Success = false, Message = "Invalid request" });

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _transactionPinService.ChangePinAsync(userId, request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyPin([FromBody] VerifyPinDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new PinResponseDto { Success = false, Message = "Invalid request" });

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _transactionPinService.VerifyPinAsync(userId, request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
