using System;
using Core.DTOs.TransactionPin;
using Core.Interfaces;
using Core.Models;
using Logic.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Logic.Services
{
    public class TransactionPinService : ITransactionPinService
    {
        private readonly AppDbContext _context;

        public TransactionPinService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PinResponseDto> CreatePinAsync(int userId, CreatePinDto request)
        {
            if (request.Pin != request.ConfirmPin)
                return new PinResponseDto { Success = false, Message = "PINs do not match" };

            if (request.Pin?.Length != 4 || !request.Pin.All(char.IsDigit))
                return new PinResponseDto { Success = false, Message = "PIN must be exactly 4 digits" };

            var existingPin = await _context.TransactionPins
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (existingPin != null)
                return new PinResponseDto { Success = false, Message = "PIN already exists. Use change PIN instead" };

            _context.TransactionPins.Add(new TransactionPin
            {
                UserId = userId,
                PinHash = HashPin(request.Pin),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return new PinResponseDto { Success = true, Message = "PIN created successfully" };
        }

        public async Task<PinResponseDto> ChangePinAsync(int userId, ChangePinDto request)
        {
            if (request.NewPin != request.ConfirmNewPin)
                return new PinResponseDto { Success = false, Message = "New PINs do not match" };

            if (request.NewPin?.Length != 4 || !request.NewPin.All(char.IsDigit))
                return new PinResponseDto { Success = false, Message = "PIN must be exactly 4 digits" };

            var transactionPin = await _context.TransactionPins
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (transactionPin == null)
                return new PinResponseDto { Success = false, Message = "PIN not found. Create a PIN first" };

            if (transactionPin.PinHash != HashPin(request.CurrentPin))
                return new PinResponseDto { Success = false, Message = "Current PIN is incorrect" };

            transactionPin.PinHash = HashPin(request.NewPin);
            transactionPin.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new PinResponseDto { Success = true, Message = "PIN changed successfully" };
        }

        public async Task<PinResponseDto> VerifyPinAsync(int userId, VerifyPinDto request)
        {
            var transactionPin = await _context.TransactionPins
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (transactionPin == null)
                return new PinResponseDto { Success = false, Message = "PIN not found. Create a PIN first" };

            if (transactionPin.PinHash != HashPin(request.Pin))
                return new PinResponseDto { Success = false, Message = "Incorrect PIN" };

            return new PinResponseDto { Success = true, Message = "PIN verified successfully" };
        }

        private string HashPin(string? pin)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(pin ?? "");
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}