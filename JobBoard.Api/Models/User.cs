using Microsoft.AspNetCore.Identity;

namespace JobBoard.Api.Models
{
    public class User : IdentityUser
    {
        // Navigation property for EmployerProfile
        public EmployerProfile? EmployerProfile { get; set; }
    }
}
