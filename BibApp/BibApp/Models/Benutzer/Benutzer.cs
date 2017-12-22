using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BibApp.Models.Benutzer
{
    public class Benutzer : IdentityUser
    {
        // TODO: LÖSCHEN
        public Buch.Buch Buch { get; set; } 
        public string Role { get; set; }
    }
}
