using System;
using System.ComponentModel.DataAnnotations;

namespace BibApp.Models.Buch
{
    public class Exemplar
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Exemplar")]
        public int ExemplarId { get; set; }

        public string ISBN { get; set; }

        public bool Verfügbarkeit { get; set; }

        [Display(Name = "Entliehen bis")]
        [DataType(DataType.Date)]
        public DateTime? EntliehenBis { get; set; }
    }
}
