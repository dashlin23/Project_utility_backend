using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Core.DTOs.Data;
using Core.Interfaces;
using Core.Models;
using Logic.Data;
using Microsoft.EntityFrameworkCore;

namespace Logic.Services
{
    public class DataService : IDataService
    {
        private readonly AppDbContext _context;

        private readonly Dictionary<string, List<object>> _dataPlans = new()
        {
            ["mtn"] = new List<object>
            {
                new { Code = "mtn-100mb-daily", Name = "100MB Daily", Amount = 100 },
                new { Code = "mtn-1gb-daily", Name = "1GB Daily", Amount = 300 },
                new { Code = "mtn-2gb-weekly", Name = "2GB Weekly", Amount = 500 },
                new { Code = "mtn-5gb-monthly", Name = "5GB Monthly", Amount = 1500 },
                new { Code = "mtn-10gb-monthly", Name = "10GB Monthly", Amount = 2500 }
            },
            ["airtel"] = new List<object>
            {
                new { Code = "airtel-100mb-daily", Name = "100MB Daily", Amount = 100 },
                new { Code = "airtel-1gb-daily", Name = "1GB Daily", Amount = 300 },
                new { Code = "airtel-2gb-weekly", Name = "2GB Weekly", Amount = 500 },
                new { Code = "airtel-5gb-monthly", Name = "5GB Monthly", Amount = 1500 },
                new { Code = "airtel-10gb-monthly", Name = "10GB Monthly", Amount = 2500 }
            },
            ["glo"] = new List<object>
            {
                new { Code = "glo-100mb-daily", Name = "100MB Daily", Amount = 50 },
                new { Code = "glo-1gb-daily", Name = "1GB Daily", Amount = 250 },
                new { Code = "glo-2gb-weekly", Name = "2GB Weekly", Amount = 450 },
                new { Code = "glo-5gb-monthly", Name = "5GB Monthly", Amount = 1200 },
                new { Code = "glo-10gb-monthly", Name = "10GB Monthly", Amount = 2000 }
            },
            ["9mobile"] = new List<object>
            {
                new { Code = "9mobile-100mb-daily", Name = "100MB Daily", Amount = 100 },
                new { Code = "9mobile-1gb-daily", Name = "1GB Daily", Amount = 300 },
                new { Code = "9mobile-2gb-weekly", Name = "2GB Weekly", Amount = 500 },
                new { Code = "9mobile-5gb-monthly", Name = "5GB Monthly", Amount = 1500 },
                new { Code = "9mobile-10gb-monthly", Name = "10GB Monthly", Amount = 2500 }
            }
        };

        public DataService(AppDbContext context)
        {
            _context = context;
        }

        public Task<DataResponseDto> GetNetworksAsync()
        {
            var networks = new List<object>
            {
                new { Id = 1, Name = "MTN", Code = "mtn" },
                new { Id = 2, Name = "Airtel", Code = "airtel" },
                new { Id = 3, Name = "Glo", Code = "glo" },
                new { Id = 4, Name = "9mobile", Code = "9mobile" }
            };

            return Task.FromResult(new DataResponseDto
            {
                Success = true,
                Message = "Networks retrieved successfully",
                Data = networks
            });
        }

        public Task<DataResponseDto> GetDataPlansAsync(string network)
        {
            var key = network.ToLower();

            if (!_dataPlans.ContainsKey(key))
                return Task.FromResult(new DataResponseDto
                {
                    Success = false,
                    Message = "Network not found"
                });

            return Task.FromResult(new DataResponseDto
            {
                Success = true,
                Message = $"Data plans for {network} retrieved successfully",
                Data = _dataPlans[key]
            });
        }

        public async Task<DataResponseDto> PurchaseDataAsync(int userId, PurchaseDataDto request)
        {
            if (string.IsNullOrEmpty(request.Network))
                return new DataResponseDto { Success = false, Message = "Network is required" };

            if (string.IsNullOrEmpty(request.PhoneNumber))
                return new DataResponseDto { Success = false, Message = "Phone number is required" };

            if (string.IsNullOrEmpty(request.PlanCode))
                return new DataResponseDto { Success = false, Message = "Plan is required" };

            var key = request.Network.ToLower();
            if (!_dataPlans.ContainsKey(key))
                return new DataResponseDto { Success = false, Message = "Network not found" };

            var plan = _dataPlans[key]
                .Cast<dynamic>()
                .FirstOrDefault(p => p.Code == request.PlanCode);

            if (plan == null)
                return new DataResponseDto { Success = false, Message = "Data plan not found" };

            var reference = $"DATA-{DateTime.UtcNow.Ticks}";

            var transaction = new DataTransaction
            {
                UserId = userId,
                Network = request.Network,
                PhoneNumber = request.PhoneNumber,
                PlanName = plan.Name,
                Amount = (decimal)plan.Amount,
                Status = "Success",
                Reference = reference,
                CreatedAt = DateTime.UtcNow
            };

            _context.DataTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            return new DataResponseDto
            {
                Success = true,
                Message = "Data purchased successfully",
                Data = new DataTransactionDto
                {
                    Id = transaction.Id,
                    Network = transaction.Network,
                    PhoneNumber = transaction.PhoneNumber,
                    PlanName = transaction.PlanName,
                    Amount = transaction.Amount,
                    Status = transaction.Status,
                    Reference = transaction.Reference,
                    CreatedAt = transaction.CreatedAt
                }
            };
        }

        public async Task<DataResponseDto> GetDataHistoryAsync(int userId)
        {
            var history = await _context.DataTransactions
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.CreatedAt)
                .Select(d => new DataTransactionDto
                {
                    Id = d.Id,
                    Network = d.Network,
                    PhoneNumber = d.PhoneNumber,
                    PlanName = d.PlanName,
                    Amount = d.Amount,
                    Status = d.Status,
                    Reference = d.Reference,
                    CreatedAt = d.CreatedAt
                })
                .ToListAsync();

            return new DataResponseDto
            {
                Success = true,
                Message = "Data history retrieved successfully",
                Data = history
            };
        }
    }
}
