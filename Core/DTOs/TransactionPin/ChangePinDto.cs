using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.TransactionPin
{
    public class ChangePinDto
    {
        public string? CurrentPin { get; set; }
        public string? NewPin { get; set; }
        public string? ConfirmNewPin { get; set; }
    }
}
