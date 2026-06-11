using Core.DTOs.Auth;
using Core.Interfaces;
using Core.Models;
using Logic.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Logic.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IJwtService _jwtService;

        public AuthService(AppDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return new AuthResponseDto { Success = false, Message = "Invalid email or password" };

            if (!user.IsActive)
                return new AuthResponseDto { Success = false, Message = "Account is deactivated" };

            if (user.PasswordHash != HashPassword(request.Password))
                return new AuthResponseDto { Success = false, Message = "Invalid email or password" };

            var token = _jwtService.GenerateToken(user);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Login successful",
                Token = token
            };
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto request)
        {
            if (request.Password != request.ConfirmPassword)
                return new AuthResponseDto { Success = false, Message = "Passwords do not match" };

            var exists = await _context.Users
                .AnyAsync(u => u.Email == request.Email);

            if (exists)
                return new AuthResponseDto { Success = false, Message = "Email is already registered" };

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var wallet = new Wallet
            {
                UserId = user.Id,
                Balance = 0.00m,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();

            return new AuthResponseDto { Success = true, Message = "Account created successfully" };
        }

        public async Task<AuthResponseDto> LogoutAsync(string token)
        {
            var exists = await _context.TokenBlacklist
                .AnyAsync(t => t.Token == token);

            if (exists)
                return new AuthResponseDto { Success = false, Message = "Token already invalidated" };

            _context.TokenBlacklist.Add(new TokenBlacklist
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return new AuthResponseDto { Success = true, Message = "Logged out successfully" };
        }

        public async Task<AuthResponseDto> ForgotPasswordAsync(ForgotPasswordDto request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return new AuthResponseDto { Success = true, Message = "If your email exists you will receive a reset link" };

            var existingTokens = await _context.PasswordResetTokens
                .Where(t => t.UserId == user.Id && !t.IsUsed)
                .ToListAsync();

            _context.PasswordResetTokens.RemoveRange(existingTokens);

            var resetToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            _context.PasswordResetTokens.Add(new PasswordResetToken
            {
                UserId = user.Id,
                Token = resetToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            
            return new AuthResponseDto
            {
                Success = true,
                Message = "If your email exists you will receive a reset link",
                Token = resetToken
            };
        }

        public async Task<AuthResponseDto> ResetPasswordAsync(ResetPasswordDto request)
        {
            if (request.NewPassword != request.ConfirmPassword)
                return new AuthResponseDto { Success = false, Message = "Passwords do not match" };

            var resetToken = await _context.PasswordResetTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Token == request.Token);

            if (resetToken == null)
                return new AuthResponseDto { Success = false, Message = "Invalid reset token" };

            if (resetToken.IsUsed)
                return new AuthResponseDto { Success = false, Message = "Reset token has already been used" };

            if (resetToken.ExpiresAt < DateTime.UtcNow)
                return new AuthResponseDto { Success = false, Message = "Reset token has expired" };

            resetToken.User!.PasswordHash = HashPassword(request.NewPassword);
            resetToken.IsUsed = true;

            await _context.SaveChangesAsync();

            return new AuthResponseDto { Success = true, Message = "Password reset successfully" };
        }

        public async Task<AuthResponseDto> ChangePasswordAsync(int userId, ChangePasswordDto request)
        {
            if (request.NewPassword != request.ConfirmPassword)
                return new AuthResponseDto { Success = false, Message = "Passwords do not match" };

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return new AuthResponseDto { Success = false, Message = "User not found" };

            if (user.PasswordHash != HashPassword(request.CurrentPassword))
                return new AuthResponseDto { Success = false, Message = "Current password is incorrect" };

            user.PasswordHash = HashPassword(request.NewPassword);
            await _context.SaveChangesAsync();

            return new AuthResponseDto { Success = true, Message = "Password changed successfully" };
        }

        private string HashPassword(string? password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password ?? "");
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}