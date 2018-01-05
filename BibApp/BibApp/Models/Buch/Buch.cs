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
        [Range(1, 24)]
        public int Regal { get; set; }

        [Required(ErrorMessage = "Das Reihe-Feld ist erforderlich.")]
        [Range(1, 4)]
        public int Reihe { get; set; }

        [Required(ErrorMessage = "Das Anzahl Exemplare-Feld ist erforderlich.")]
        [Display(Name = "Anzahl Exemplare")]
        [Range(0, 50)]
        public int AnzahlExemplare { get; set; }
    }
}
