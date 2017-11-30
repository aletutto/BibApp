using Microsoft.AspNetCore.Identity;

namespace BibApp.Models
{
    public class Benutzer : IdentityUser
    {
        public Buch Buch { get; set; }
    }
}
