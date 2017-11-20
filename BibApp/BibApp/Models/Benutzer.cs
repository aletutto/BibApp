using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BibApp.Models
{
    public class Benutzer
    {
        public int Id { get; set; }

        public static bool isLoggedIn = false;

        public static bool isAdmin = false;

        public string Rolle { get; set; }

        [Required]
        public string Benutzername { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Passwort { get; set; }
    }
}
