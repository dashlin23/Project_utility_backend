using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Core.DTOs.Electricity
{
    public class PurchaseElectricityDto
    {
        public string? DiscoProvider { get; set; }
        public string? MeterNumber { get; set; }
        public string? MeterType { get; set; }
        public string? CustomerName { get; set; }
        public decimal Amount { get; set; }
    }
}