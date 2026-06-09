using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DTOs.Electricity;
using Core.Interfaces;
using Core.Models;
using Logic.Data;
using Microsoft.EntityFrameworkCore;

namespace Logic.Services
{
    public class ElectricityService : IElectricityService
    {
        private readonly AppDbContext _context;

        public ElectricityService(AppDbContext context)
        {
            _context = context;
        }

        public Task<ElectricityResponseDto> GetDiscoProvidersAsync()
        {
            var providers = new List<object>
            {
                new { Id = 1,  Name = "Ikeja Electric",           Code = "ikeja-electric"    },
                new { Id = 2,  Name = "Eko Electric",             Code = "eko-electric"      },
                new { Id = 3,  Name = "Abuja Electric",           Code = "abuja-electric"    },
                new { Id = 4,  Name = "Kano Electric",            Code = "kano-electric"     },
                new { Id = 5,  Name = "Port Harcourt Electric",   Code = "phed"              },
                new { Id = 6,  Name = "Enugu Electric",           Code = "enugu-electric"    },
                new { Id = 7,  Name = "Ibadan Electric",          Code = "ibadan-electric"   },
                new { Id = 8,  Name = "Kaduna Electric",          Code = "kaduna-electric"   },
                new { Id = 9,  Name = "Jos Electric",             Code = "jos-electric"      },
                new { Id = 10, Name = "Benin Electric",           Code = "benin-electric"    }
            };

            return Task.FromResult(new ElectricityResponseDto
            {
                Success = true,
                Message = "Disco providers retrieved successfully",
                Data = providers
            });
        }

        public Task<ElectricityResponseDto> VerifyMeterAsync(VerifyMeterDto request)
        {
            if (string.IsNullOrEmpty(request.DiscoProvider))
                return Task.FromResult(new ElectricityResponseDto
                {
                    Success = false,
                    Message = "Disco provider is required"
                });

            if (string.IsNullOrEmpty(request.MeterNumber))
                return Task.FromResult(new ElectricityResponseDto
                {
                    Success = false,
                    Message = "Meter number is required"
                });

            if (string.IsNullOrEmpty(request.MeterType))
                return Task.FromResult(new ElectricityResponseDto
                {
                    Success = false,
                    Message = "Meter type is required"
                });

            return Task.FromResult(new ElectricityResponseDto
            {
                Success = true,
                Message = "Meter verified successfully",
                Data = new
                {
                    MeterNumber = request.MeterNumber,
                    MeterType = request.MeterType,
                    DiscoProvider = request.DiscoProvider,
                    CustomerName = "John Doe",
                    Address = "123 Test Street, Lagos"
                }
            });
        }

        public async Task<ElectricityResponseDto> PurchaseElectricityAsync(int userId, PurchaseElectricityDto request)
        {
            if (string.IsNullOrEmpty(request.DiscoProvider))
                return new ElectricityResponseDto { Success = false, Message = "Disco provider is required" };

            if (string.IsNullOrEmpty(request.MeterNumber))
                return new ElectricityResponseDto { Success = false, Message = "Meter number is required" };

            if (string.IsNullOrEmpty(request.MeterType))
                return new ElectricityResponseDto { Success = false, Message = "Meter type is required" };

            if (request.Amount < 100)
                return new ElectricityResponseDto { Success = false, Message = "Minimum amount is ₦100" };

            var reference = $"ELECT-{DateTime.UtcNow.Ticks}";
            var token = GenerateElectricityToken();

            var transaction = new ElectricityTransaction
            {
                UserId = userId,
                DiscoProvider = request.DiscoProvider,
                MeterNumber = request.MeterNumber,
                MeterType = request.MeterType,
                CustomerName = request.CustomerName,  
                Amount = request.Amount,
                Token = token,
                Status = "Success",
                Reference = reference,
                CreatedAt = DateTime.UtcNow
            };

            _context.ElectricityTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            return new ElectricityResponseDto
            {
                Success = true,
                Message = "Electricity purchase successful",
                Data = new ElectricityTransactionDto
                {
                    Id = transaction.Id,
                    DiscoProvider = transaction.DiscoProvider,
                    MeterNumber = transaction.MeterNumber,
                    MeterType = transaction.MeterType,
                    CustomerName = transaction.CustomerName,
                    Amount = transaction.Amount,
                    Token = transaction.Token,
                    Status = transaction.Status,
                    Reference = transaction.Reference,
                    CreatedAt = transaction.CreatedAt
                }
            };
        }

        public async Task<ElectricityResponseDto> GetElectricityHistoryAsync(int userId)
        {
            var history = await _context.ElectricityTransactions
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.CreatedAt)
                .Select(e => new ElectricityTransactionDto
                {
                    Id = e.Id,
                    DiscoProvider = e.DiscoProvider,
                    MeterNumber = e.MeterNumber,
                    MeterType = e.MeterType,
                    CustomerName = e.CustomerName,
                    Amount = e.Amount,
                    Token = e.Token,
                    Status = e.Status,
                    Reference = e.Reference,
                    CreatedAt = e.CreatedAt
                })
                .ToListAsync();

            return new ElectricityResponseDto
            {
                Success = true,
                Message = "Electricity history retrieved successfully",
                Data = history
            };
        }

        private string GenerateElectricityToken()
        {
            var random = new Random();
            return string.Join("-", Enumerable.Range(0, 5)
                .Select(_ => random.Next(1000, 9999).ToString()));
        }
    }
}