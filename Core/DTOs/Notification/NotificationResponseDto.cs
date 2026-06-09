using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Notification
{
    public class NotificationResponseDto
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<NotificationDto>? Data { get; set; }
    }
}
