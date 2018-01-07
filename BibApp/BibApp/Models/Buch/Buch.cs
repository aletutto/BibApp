using System;
using System.ComponentModel.DataAnnotations;

namespace BibApp.Models.Buch
{
    public class Buch
    {
        [Key]
        public int Id { get; set; }

        [Required( ErrorMessage = "Das ISBN-Feld ist erforderlich.")]
        public String ISBN { get; set; }

        [Required(ErrorMessage = "Das Titel-Feld ist erforderlich.")]
        public String Titel { get; set; }

        [Required(ErrorMessage = "Das Autor-Feld ist erforderlich.")]
        public String Autor { get; set; }

        [Required(ErrorMessage = "Das Verlag-Feld ist erforderlich.")]
        public String Verlag { get; set; }

        [Required(ErrorMessage = "Das Erscheinungsjahr-Feld ist erforderlich.")]
        public int Erscheinungsjahr { get; set; }

        [Required(ErrorMessage = "Das Regal-Feld ist erforderlich.")]
        [Range(1, 24, ErrorMessage = "Das Regal-Feld muss zwischen 1 und 24 liegen.")]
        public int Regal { get; set; }

        [Required(ErrorMessage = "Das Reihe-Feld ist erforderlich.")]
        [Range(1, 4, ErrorMessage = "Das Reihe-Feld muss zwischen 1 und 4 liegen.")]
        public int Reihe { get; set; }

        [Required(ErrorMessage = "Das Anzahl Exemplare-Feld ist erforderlich.")]
        [Display(Name = "Anzahl Exemplare")]
        [Range(0, 50, ErrorMessage = "Das Anzahl Exemplare-Feld muss zwischen 0 und 50 liegen.")]
        public int AnzahlExemplare { get; set; }
    }
}
