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

        public String ISBN { get; set; }

        public String Titel { get; set; }

        public String Autor { get; set; }

        public String Verlag { get; set; }

        public int Erscheinungsjahr { get; set; }

        [Range(1, 24)]
        public int Regal { get; set; }

        [Range(1, 4)]
        public int Reihe { get; set; }

        public int AnzahlExemplare { get; set; }
    }
}
