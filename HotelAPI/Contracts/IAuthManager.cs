using HotelAPI.Models.User;
using Microsoft.AspNetCore.Identity;

namespace HotelAPI.Contracts
{
    public interface IAuthManager
    {
        Task<IEnumerable<IdentityError>> Register(ApiUserDto userDto);
        Task<AuthResponseDto> Login(LoginDto loginDto);
    }
}
