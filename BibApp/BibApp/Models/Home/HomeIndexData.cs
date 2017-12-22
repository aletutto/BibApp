using BibApp.Models.Buch;
using BibApp.Models.Warenkorb;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BibApp.Models
{
    public class HomeIndexData
    {
        public List<Exemplar> Exemplare { get; set; }
        public Exemplar Exemplar { get; set; }

        public List<Buch.Buch> Buecher { get; set; }
        public Buch.Buch Buch { get; set; }

        public List<AdminKorb> AdminKoerbe { get; set; }
        public AdminKorb AdminKorb { get; set; }

        public Dictionary<AdminKorb, Exemplar>  Dictionary { get; set; }
}
}
