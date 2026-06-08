using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.TransactionPin
{
    public class CreatePinDto
    {
        public string? Pin { get; set; }
        public string? ConfirmPin { get; set; }
    }
}
