using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DTOs.User;

namespace Core.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto> GetUserProfileAsync(int userId);
        Task<UserResponseDto> DeleteAccountAsync(int userId);
    }
}
