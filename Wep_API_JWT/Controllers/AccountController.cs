using Microsoft.AspNetCore.Mvc;
using Shared_ClassLibrary.Contracts;
using Shared_ClassLibrary.DTOs;
namespace Wep_API_JWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(IUserAccount userAccount) : ControllerBase
    {
        [HttpPost("Register")]
        public  async Task<IActionResult> Register(UserDTOs userDTOs)
        {
            var result = await userAccount.CreateAccountAsync(userDTOs);
            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<IActionResult>Login(LoginDTO loginDTO)
        {
            var result = await userAccount.LoginAccount(loginDTO);
            return Ok(result);
        }
    }
}
