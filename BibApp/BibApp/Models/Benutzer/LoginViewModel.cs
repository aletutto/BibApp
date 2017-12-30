using System.ComponentModel.DataAnnotations;

namespace BibApp.Models
{
    public class LoginViewModel
    {
        [Required]
        public string Benutzername { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Passwort { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}

