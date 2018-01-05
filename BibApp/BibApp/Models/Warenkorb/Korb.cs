using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BibApp.Models.Warenkorb
{
    public class Korb
    {
        [Key]
        public int Id { get; set; }

        public string Benutzer { get; set; }

        public string ISBN { get; set; }

        [Display(Name = "Exemplar")]
        public int ExemplarId { get; set; }

        [Display(Name = "Buch")]
        public string BuchTitel { get; set; }
    }
}
