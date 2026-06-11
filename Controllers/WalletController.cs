using Core.DTOs;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PaymentUtility.Controllers
{
    [ApiController]
    [Route("api/wallet")]
    [Authorize]
    [Produces("application/json")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly ILogger<WalletController> _logger;

        public WalletController(IWalletService walletService, ILogger<WalletController> logger)
        {
            _walletService = walletService;
            _logger = logger;
        }

        [HttpGet("balance")]
        public async Task<IActionResult> GetBalance()
        {
            try
            {
                var result = await _walletService.GetBalanceAsync(GetUserId());
                return Ok(new { success = true, message = "Balance retrieved.", data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetBalance error");
                return StatusCode(500, new { success = false, message = "An error occurred." });
            }
        }

        [HttpPost("fund")]
        public async Task<IActionResult> FundWallet([FromBody] FundWalletDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { success = false, message = "Validation failed.", errors });
            }

            try
            {
                var result = await _walletService.FundWalletAsync(GetUserId(), dto);
                return Ok(new { success = true, message = $"Wallet funded with ₦{dto.Amount:N2}.", data = result });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "FundWallet error");
                return StatusCode(500, new { success = false, message = "An error occurred." });
            }
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactions([FromQuery] TransactionFilterDto filter)
        {
            filter.PageSize = Math.Clamp(filter.PageSize, 1, 100);
            filter.Page = Math.Max(filter.Page, 1);

            try
            {
                var result = await _walletService.GetTransactionHistoryAsync(GetUserId(), filter);
                return Ok(new { success = true, message = "Transactions retrieved.", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetTransactions error");
                return StatusCode(500, new { success = false, message = "An error occurred." });
            }
        }

        private int GetUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? User.FindFirstValue("sub");

            if (string.IsNullOrEmpty(claim))
                throw new UnauthorizedAccessException("User ID not found in token.");

            if (!int.TryParse(claim, out var userId))
                throw new UnauthorizedAccessException("Invalid user ID format in token.");

            return userId;
        }
    }
}