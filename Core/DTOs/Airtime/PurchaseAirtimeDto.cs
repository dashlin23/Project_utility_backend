using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Airtime
{
    public class PurchaseAirtimeDto
    {
        public string? Network { get; set; }
        public string? PhoneNumber { get; set; }
        public decimal Amount { get; set; }
    }
}
