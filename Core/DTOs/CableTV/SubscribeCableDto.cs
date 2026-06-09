using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.CableTV
{
    public class SubscribeCableDto
    {
        public string? Provider { get; set; }
        public string? PackageCode { get; set; }
        public string? SmartCardNumber { get; set; }
    }
}
