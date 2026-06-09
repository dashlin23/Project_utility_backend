using System;
using System.Collections.Generic;
using Core.DTOs.Notification;
using Core.Interfaces;
using Logic.Data;
using Microsoft.EntityFrameworkCore;

namespace Logic.Services
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _context;

        public NotificationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<NotificationResponseDto> GetNotificationsAsync(int userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                })
                .ToListAsync();

            return new NotificationResponseDto
            {
                Success = true,
                Message = "Notifications retrieved successfully",
                Data = notifications
            };
        }

        public async Task<NotificationResponseDto> MarkAsReadAsync(int userId, int notificationId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification == null)
                return new NotificationResponseDto
                {
                    Success = false,
                    Message = "Notification not found"
                };

            if (notification.IsRead)
                return new NotificationResponseDto
                {
                    Success = false,
                    Message = "Notification already marked as read"
                };

            notification.IsRead = true;
            await _context.SaveChangesAsync();

            return new NotificationResponseDto
            {
                Success = true,
                Message = "Notification marked as read"
            };
        }
    }
}
