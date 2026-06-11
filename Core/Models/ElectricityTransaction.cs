using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class ElectricityTransaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? DiscoProvider { get; set; }
        public string? MeterNumber { get; set; }
        public string? MeterType { get; set; }
        public string? CustomerName { get; set; }
        public decimal Amount { get; set; }
        public string? Token { get; set; }
        public string? Status { get; set; } = "Pending";
        public string? Reference { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User? User { get; set; }
    }
}