using System;
using System.Collections.Generic;
using Core.DTOs.User;
using Core.Interfaces;
using Logic.Data;
using Microsoft.EntityFrameworkCore;

namespace Logic.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserResponseDto> GetUserProfileAsync(int userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return new UserResponseDto
                {
                    Success = false,
                    Message = "User not found"
                };

            return new UserResponseDto
            {
                Success = true,
                Message = "Profile retrieved successfully",
                Data = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    CreatedAt = user.CreatedAt
                }
            };
        }

        public async Task<UserResponseDto> DeleteAccountAsync(int userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return new UserResponseDto
                {
                    Success = false,
                    Message = "User not found"
                };

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return new UserResponseDto
            {
                Success = true,
                Message = "Account deleted successfully"
            };
        }
    }
}
