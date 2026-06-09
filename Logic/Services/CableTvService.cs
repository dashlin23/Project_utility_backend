using Core.DTOs.CableTV;
using Core.Interfaces;
using Core.Models;
using Logic.Data;
using Microsoft.EntityFrameworkCore;

namespace Logic.Services
{
    public class CableTvService : ICableTvService
    {
        private readonly AppDbContext _context;

        private readonly Dictionary<string, List<object>> _packages = new()
        {
            ["dstv"] = new List<object>
            {
                new { Code = "dstv-padi", Name = "DStv Padi", Amount = 2500 },
                new { Code = "dstv-yanga", Name = "DStv Yanga", Amount = 3500 },
                new { Code = "dstv-confam", Name = "DStv Confam", Amount = 6200 },
                new { Code = "dstv-compact", Name = "DStv Compact", Amount = 10500 },
                new { Code = "dstv-premium", Name = "DStv Premium", Amount = 24500 }
            },
            ["gotv"] = new List<object>
            {
                new { Code = "gotv-smallie", Name = "GOtv Smallie", Amount = 1575 },
                new { Code = "gotv-jinja", Name = "GOtv Jinja", Amount = 2715 },
                new { Code = "gotv-jolli", Name = "GOtv Jolli", Amount = 4115 },
                new { Code = "gotv-max", Name = "GOtv Max", Amount = 6000 }
            },
            ["startimes"] = new List<object>
            {
                new { Code = "startimes-nova", Name = "StarTimes Nova", Amount = 1200 },
                new { Code = "startimes-basic", Name = "StarTimes Basic", Amount = 2000 },
                new { Code = "startimes-smart", Name = "StarTimes Smart", Amount = 2800 },
                new { Code = "startimes-classic", Name = "StarTimes Classic", Amount = 3300 }
            }
        };

        public CableTvService(AppDbContext context)
        {
            _context = context;
        }

        public Task<CableTvResponseDto> GetProvidersAsync()
        {
            var providers = new List<object>
            {
                new { Id = 1, Name = "DStv", Code = "dstv" },
                new { Id = 2, Name = "GOtv", Code = "gotv" },
                new { Id = 3, Name = "StarTimes", Code = "startimes" }
            };

            return Task.FromResult(new CableTvResponseDto
            {
                Success = true,
                Message = "Providers retrieved successfully",
                Data = providers
            });
        }

        public Task<CableTvResponseDto> GetPackagesAsync(string provider)
        {
            var key = provider.ToLower();

            if (!_packages.ContainsKey(key))
                return Task.FromResult(new CableTvResponseDto
                {
                    Success = false,
                    Message = "Provider not found"
                });

            return Task.FromResult(new CableTvResponseDto
            {
                Success = true,
                Message = $"Packages for {provider} retrieved successfully",
                Data = _packages[key]
            });
        }

        public async Task<CableTvResponseDto> SubscribeAsync(int userId, SubscribeCableDto request)
        {
            if (string.IsNullOrEmpty(request.Provider))
                return new CableTvResponseDto { Success = false, Message = "Provider is required" };

            if (string.IsNullOrEmpty(request.SmartCardNumber))
                return new CableTvResponseDto { Success = false, Message = "Smart card number is required" };

            if (string.IsNullOrEmpty(request.PackageCode))
                return new CableTvResponseDto { Success = false, Message = "Package is required" };

            var key = request.Provider.ToLower();
            if (!_packages.ContainsKey(key))
                return new CableTvResponseDto { Success = false, Message = "Provider not found" };

            var package = _packages[key]
                .Cast<dynamic>()
                .FirstOrDefault(p => p.Code == request.PackageCode);

            if (package == null)
                return new CableTvResponseDto { Success = false, Message = "Package not found" };

            var reference = $"CABLE-{DateTime.UtcNow.Ticks}";

            var transaction = new CableTvTransaction
            {
                UserId = userId,
                Provider = request.Provider,
                PackageName = package.Name,
                SmartCardNumber = request.SmartCardNumber,
                Amount = (decimal)package.Amount,
                Status = "Success",
                Reference = reference,
                CreatedAt = DateTime.UtcNow
            };

            _context.CableTvTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            return new CableTvResponseDto
            {
                Success = true,
                Message = "Cable TV subscription successful",
                Data = new CableTvTransactionDto
                {
                    Id = transaction.Id,
                    Provider = transaction.Provider,
                    PackageName = transaction.PackageName,
                    SmartCardNumber = transaction.SmartCardNumber,
                    Amount = transaction.Amount,
                    Status = transaction.Status,
                    Reference = transaction.Reference,
                    CreatedAt = transaction.CreatedAt
                }
            };
        }

        public async Task<CableTvResponseDto> GetCableTvHistoryAsync(int userId)
        {
            var history = await _context.CableTvTransactions
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CableTvTransactionDto
                {
                    Id = c.Id,
                    Provider = c.Provider,
                    PackageName = c.PackageName,
                    SmartCardNumber = c.SmartCardNumber,
                    Amount = c.Amount,
                    Status = c.Status,
                    Reference = c.Reference,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return new CableTvResponseDto
            {
                Success = true,
                Message = "Cable TV history retrieved successfully",
                Data = history
            };
        }
    }
}
