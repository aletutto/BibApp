using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BibApp.Models
{
    public class Benutzer : IdentityUser
    {
        public Buch Buch { get; set; } 
        public string Role { get; set; }


    }
}
