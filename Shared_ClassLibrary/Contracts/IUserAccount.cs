using Shared_ClassLibrary.DTOs;
using static Shared_ClassLibrary.DTOs.ServicesResponses;
namespace Shared_ClassLibrary.Contracts
{
    public interface IUserAccount
    {
        Task<GeneralResponse> CreateAccountAsync(UserDTOs createDTO);
        Task<LoginResponse> LoginAccount(LoginDTO loginDTO);
    }
}
