using System.ComponentModel.DataAnnotations;

namespace BibApp.Models
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Benutzername")]
        public string Benutzername { get; set; }

        [Required]
        [Display(Name = "E-Mail")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Das {0} muss mindestens {2} und maximal {1} Zeichen lang sein.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Passwort")]
        public string Passwort { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Passwort bestätigen")]
        [Compare("Passwort", ErrorMessage = "Das Passwort und die Passwort-Bestätigung stimmen nicht überein.")]
        public string ConfirmPasswort { get; set; }
    }
}
