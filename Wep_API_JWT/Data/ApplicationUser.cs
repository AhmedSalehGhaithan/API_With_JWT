using Microsoft.AspNetCore.Identity;
namespace Wep_API_JWT.Data
{
    public class ApplicationUser:IdentityUser
    {
        //adding custom property to the identityUser class that represent the user
        public string? Name { get; set; }
    }
}
