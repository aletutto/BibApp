using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace BibApp.Models.Buch
{
    public class Buch
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public String ISBN { get; set; }

        [Required]
        public String Titel { get; set; }

        [Required]
        public String Autor { get; set; }

        [Required]
        public String Verlag { get; set; }

        [Required]
        public int Erscheinungsjahr { get; set; }

        [Range(1, 24)]
        public int Regal { get; set; }

        [Range(1, 4)]
        public int Reihe { get; set; }

        [Required]
        [Display(Name = "Anzahl Exemplare")]
        public int AnzahlExemplare { get; set; }
    }
}
