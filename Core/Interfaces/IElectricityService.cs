using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DTOs.Electricity;

namespace Core.Interfaces
{
    public interface IElectricityService
    {
        Task<ElectricityResponseDto> GetDiscoProvidersAsync();
        Task<ElectricityResponseDto> VerifyMeterAsync(VerifyMeterDto request);
        Task<ElectricityResponseDto> PurchaseElectricityAsync(int userId, PurchaseElectricityDto request);
        Task<ElectricityResponseDto> GetElectricityHistoryAsync(int userId);
    }
}
