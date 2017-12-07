using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BibApp.Models.Buch
{
    public class Exemplar
    {
        [Key]
        public int Id { get; set; }
        public int ExemplarId { get; set; }
        public string ISBN { get; set; }

        public bool Verfügbarkeit { get; set; }

        [DataType(DataType.Date)]
        public DateTime EntliehenVom { get; set; }

        [DataType(DataType.Date)]
        public DateTime EntliehenBis { get; set; }

        public bool IstVorgemerkt { get; set; }
    }
}
