using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using Shared_ClassLibrary.Contracts;
using Shared_ClassLibrary.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Wep_API_JWT.Data;
using static Shared_ClassLibrary.DTOs.ServicesResponses;
namespace Wep_API_JWT.Repository
{
   //  the UserManager<ApplicationUser> class, which provides access to the data in the Identity user store
    public class AccountRepo(UserManager<ApplicationUser> _userManager,RoleManager<IdentityRole> _roleManager,IConfiguration _config) : IUserAccount
    {
        public async Task<GeneralResponse> CreateAccountAsync(UserDTOs userDTO)
        {
            if (userDTO is null) return new GeneralResponse(false,"Model is empty!");

            var newUserCreated = new ApplicationUser()
            {
                Name = userDTO.Name,
                Email = userDTO.Email,
                PasswordHash = userDTO.Password,
                UserName = userDTO.Email
            };

            var CheckUser = await _userManager.FindByEmailAsync(newUserCreated.Email);
            if (CheckUser != null) return new GeneralResponse(false, "User Already Registered!");

            var createUser = await _userManager.CreateAsync(newUserCreated, userDTO.Password!);
            if(!createUser.Succeeded) return new GeneralResponse(false,"Error occurred... please try again later");

            //assign default role
            var checkAdminRole = await _roleManager.FindByNameAsync("Admin");
            if (checkAdminRole is null)
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
                await _userManager.AddToRoleAsync(newUserCreated, "Admin");
                return new GeneralResponse(true, "Account Created");
            }
            else
            {
                var checkUserRole = _roleManager.FindByNameAsync("User");
                if (checkUserRole is null)
                    await _roleManager.CreateAsync(new IdentityRole { Name = "User" });

                await _userManager.AddToRoleAsync(newUserCreated, "User");
                return new GeneralResponse(true,"Account created");
            }

            throw new NotImplementedException();
        }
        public async Task<LoginResponse> LoginAccount(LoginDTO loginDTO)
        {

            if (loginDTO == null) return new LoginResponse(false, null!, "Login Container is empty");

            var getUser = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (getUser is null) return new LoginResponse(false, null!, "User not found");

            bool checkUserPassword = await _userManager.CheckPasswordAsync(getUser, loginDTO.Password);
            if (!checkUserPassword) return new LoginResponse(false, null!, "Invalid email/password");

            var getUserRoles = await _userManager.GetRolesAsync(getUser);
            var userSession = new UserSession(getUser.Id, getUser.Name, getUser.Email, getUserRoles.First());

            string token = GenerateToken(userSession);
            return new LoginResponse(true, token, "Login Completed");

            throw new NotImplementedException();
        }
        private string GenerateToken(UserSession user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            //Claims are a general-purpose approach to describing any data that is known about 
            // a user and allow custom data to be added to the Identity user store.
            // Claims are used to represent any data known about a user.

            // The ClaimTypes class provides a series of constant string values that are used to
            // specify the claim type, which denotes the type of information the claim represents.
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.ID!.ToString()),
                new Claim(ClaimTypes.Name,user.Name!),
                new Claim(ClaimTypes.Email,user.Email!),
                new Claim(ClaimTypes.Role,user.Role!)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: userClaims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credential
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
