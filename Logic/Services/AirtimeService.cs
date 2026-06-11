using System;
using System.Collections.Generic;
using Core.DTOs.Airtime;
using Core.Interfaces;
using Core.Models;
using Logic.Data;
using Microsoft.EntityFrameworkCore;

namespace Logic.Services
{
    public class AirtimeService : IAirtimeService
    {
        private readonly AppDbContext _context;

        public AirtimeService(AppDbContext context)
        {
            _context = context;
        }

        public Task<AirtimeResponseDto> GetNetworksAsync()
        {
            var networks = new List<object>
            {
                new { Id = 1, Name = "MTN", Code = "mtn" },
                new { Id = 2, Name = "Airtel", Code = "airtel" },
                new { Id = 3, Name = "Glo", Code = "glo" },
                new { Id = 4, Name = "9mobile", Code = "9mobile" }
            };

            return Task.FromResult(new AirtimeResponseDto
            {
                Success = true,
                Message = "Networks retrieved successfully",
                Data = networks
            });
        }

        public async Task<AirtimeResponseDto> PurchaseAirtimeAsync(int userId, PurchaseAirtimeDto request)
        {
            if (request.Amount <= 0)
                return new AirtimeResponseDto { Success = false, Message = "Amount must be greater than zero" };

            if (string.IsNullOrEmpty(request.PhoneNumber))
                return new AirtimeResponseDto { Success = false, Message = "Phone number is required" };

            if (string.IsNullOrEmpty(request.Network))
                return new AirtimeResponseDto { Success = false, Message = "Network is required" };

            var reference = $"AIR-{DateTime.UtcNow.Ticks}";

            var transaction = new AirtimeTransaction
            {
                UserId = userId,
                Network = request.Network,
                PhoneNumber = request.PhoneNumber,
                Amount = request.Amount,
                Status = "Success",
                Reference = reference,
                CreatedAt = DateTime.UtcNow
            };

            _context.AirtimeTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            return new AirtimeResponseDto
            {
                Success = true,
                Message = "Airtime purchased successfully",
                Data = new AirtimeTransactionDto
                {
                    Id = transaction.Id,
                    Network = transaction.Network,
                    PhoneNumber = transaction.PhoneNumber,
                    Amount = transaction.Amount,
                    Status = transaction.Status,
                    Reference = transaction.Reference,
                    CreatedAt = transaction.CreatedAt
                }
            };
        }

        public async Task<AirtimeResponseDto> GetAirtimeHistoryAsync(int userId)
        {
            var history = await _context.AirtimeTransactions
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new AirtimeTransactionDto
                {
                    Id = a.Id,
                    Network = a.Network,
                    PhoneNumber = a.PhoneNumber,
                    Amount = a.Amount,
                    Status = a.Status,
                    Reference = a.Reference,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();

            return new AirtimeResponseDto
            {
                Success = true,
                Message = "Airtime history retrieved successfully",
                Data = history
            };
        }
    }
}
