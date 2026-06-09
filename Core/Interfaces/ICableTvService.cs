using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DTOs.CableTV;

namespace Core.Interfaces
{
    public interface ICableTvService
    {
        Task<CableTvResponseDto> GetProvidersAsync();
        Task<CableTvResponseDto> GetPackagesAsync(string provider);
        Task<CableTvResponseDto> SubscribeAsync(int userId, SubscribeCableDto request);
        Task<CableTvResponseDto> GetCableTvHistoryAsync(int userId);
    }
}