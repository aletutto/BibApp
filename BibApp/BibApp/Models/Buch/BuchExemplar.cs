using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BibApp.Models.Buch
{
    public class BuchExemplar
    {
        public List<Exemplar> Exemplare { get; set; }
        public Exemplar Exemplar { get; set; }

        public List<Buch> Buecher { get; set; }
        public Buch Buch { get; set; }
    }
}
