using System.ComponentModel.DataAnnotations;

namespace BibApp.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Der Benutzername muss eingegeben werden.")]
        [Display(Name = "Benutzername")]
        public string Benutzername { get; set; }

        [Required(ErrorMessage = "Die E-Mail Adresse muss eingegeben werden.")]
        [Display(Name = "E-Mail")]
        public string Email { get; set; }

        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[$@$!%*?&])[A-Za-z\\d$@$!%*?&]{8,}", ErrorMessage = "Passwort muss enthalten: Mindestens 8 Zeichen, 1 Großbuchstaben, 1 Kleinbuchstaben, 1 Ziffer und 1 Sonderzeichen")]
        [Required(ErrorMessage = "Ein Passwort muss vergeben werden.")]
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
