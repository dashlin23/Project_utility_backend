using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DTOs.Airtime;

namespace Core.Interfaces
{
    public interface IAirtimeService
    {
        Task<AirtimeResponseDto> GetNetworksAsync();
        Task<AirtimeResponseDto> PurchaseAirtimeAsync(int userId, PurchaseAirtimeDto request);
        Task<AirtimeResponseDto> GetAirtimeHistoryAsync(int userId);
    }
}
