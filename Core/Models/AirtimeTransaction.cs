using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class AirtimeTransaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? Network { get; set; }
        public string? PhoneNumber { get; set; }
        public decimal Amount { get; set; }
        public string? Status { get; set; } = "Pending";
        public string? Reference { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User? User { get; set; }
    }
}