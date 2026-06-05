using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Core.DTOs.Auth;

namespace Core.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto request);
        Task<AuthResponseDto> RegisterAsync(RegisterDto request);
        Task<AuthResponseDto> LogoutAsync(string token);
        Task<AuthResponseDto> ForgotPasswordAsync(ForgotPasswordDto request);
        Task<AuthResponseDto> ResetPasswordAsync(ResetPasswordDto request);
        Task<AuthResponseDto> ChangePasswordAsync(int userId, ChangePasswordDto request);
    }
}