using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BibApp.Models.Benutzer
{
    public class Benutzer : IdentityUser
    {
        [Display(Name = "Rolle")]
        public string Role { get; set; }

        public string Benutzername;
    }
}
