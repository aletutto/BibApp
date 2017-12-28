using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BibApp.Models
{
    public class LoginViewModel
    {
        [Required]
        public string Benutzername { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Passwort { get; set; }
    }
}

