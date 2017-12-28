using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BibApp.Models
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Benutzername")]
        public string Benutzername { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Passwort")]
        public string Passwort { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Passwort bestätigen")]
        [Compare("Passwort", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPasswort { get; set; }
    }
}
