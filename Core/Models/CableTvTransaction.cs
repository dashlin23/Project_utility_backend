using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Core.Models
{
    public class CableTvTransaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? Provider { get; set; }
        public string? PackageName { get; set; }
        public string? SmartCardNumber { get; set; }
        public decimal Amount { get; set; }
        public string? Status { get; set; } = "Pending";
        public string? Reference { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User? User { get; set; }
    }
}