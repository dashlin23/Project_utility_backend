using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DTOs.Data;

namespace Core.Interfaces
{
    public interface IDataService
    {
        Task<DataResponseDto> GetNetworksAsync();
        Task<DataResponseDto> GetDataPlansAsync(string network);
        Task<DataResponseDto> PurchaseDataAsync(int userId, PurchaseDataDto request);
        Task<DataResponseDto> GetDataHistoryAsync(int userId);
    }
}
