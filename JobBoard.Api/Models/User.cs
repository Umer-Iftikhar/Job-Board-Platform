using Microsoft.AspNetCore.Identity;

namespace JobBoard.Api.Models
{
    public class User :  IdentityUser
    {
        // Employer will have EmployerProfile linked via UserId.
    }
}
