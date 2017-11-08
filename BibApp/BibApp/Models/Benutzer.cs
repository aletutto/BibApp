using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BibApp.Models
{
    public class Benutzer
    {
        public int Id { get; set; }

        [Required]
        public string Benutzername { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Passwort { get; set; }
    }
}
