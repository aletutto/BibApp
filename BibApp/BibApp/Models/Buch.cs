using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace BibApp.Models
{
    public class Buch
    {
        public int Id { get; set; }

        public String Bezeichnung { get; set; }

        public String Autoren { get; set; }

        public bool Verfügbarkeit { get; set; }

        public String Verlag { get; set; }

        [Range(1, 24)]
        public int Regal { get; set; }

        [Range(1, 4)]
        public int Reihe { get; set; }
    }
}
