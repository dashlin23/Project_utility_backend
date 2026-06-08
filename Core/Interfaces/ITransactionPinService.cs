using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DTOs.TransactionPin;

namespace Core.Interfaces
{
    public interface ITransactionPinService
    {
        Task<PinResponseDto> CreatePinAsync(int userId, CreatePinDto request);
        Task<PinResponseDto> ChangePinAsync(int userId, ChangePinDto request);
        Task<PinResponseDto> VerifyPinAsync(int userId, VerifyPinDto request);
    }
}
