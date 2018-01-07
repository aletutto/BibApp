using System.ComponentModel.DataAnnotations;

namespace BibApp.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Der Benutzername muss eingegeben werden.")]
        public string Benutzername { get; set; }

        [Required(ErrorMessage = "Das Passwort muss eingegeben werden.")]
        [DataType(DataType.Password)]
        public string Passwort { get; set; }

        public bool RememberMe { get; set; }
    }
}

