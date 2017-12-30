using Microsoft.AspNetCore.Identity;

namespace BibApp.Models.Benutzer
{
    public class Benutzer : IdentityUser
    {
        public string Role { get; set; }
    }
}
