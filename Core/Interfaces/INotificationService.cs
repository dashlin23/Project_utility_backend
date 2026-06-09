using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DTOs.Notification;

namespace Core.Interfaces
{
    public interface INotificationService
    {
        Task<NotificationResponseDto> GetNotificationsAsync(int userId);
        Task<NotificationResponseDto> MarkAsReadAsync(int userId, int notificationId);
    }
}
