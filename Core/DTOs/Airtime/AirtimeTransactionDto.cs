using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Airtime
{
    public class AirtimeTransactionDto
    {
        public int Id { get; set; }
        public string? Network { get; set; }
        public string? PhoneNumber { get; set; }
        public decimal Amount { get; set; }
        public string? Status { get; set; }
        public string? Reference { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
