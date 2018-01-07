using BibApp.Models.Buch;
using System.Collections.Generic;

namespace BibApp.Models.Warenkorb
{
    public class LeihauftragExemplar
    {
        public Leihauftrag Leihauftrag { get; set; }
        public Exemplar Exemplar { get; set; }

        public List<Leihauftrag> Ausleihen { get; set; }

        public Dictionary<Leihauftrag, Exemplar> Zurückgeben { get; set; }
    }
}
