using BibApp.Models.Buch;
using BibApp.Models.Warenkorb;
using System.Collections.Generic;

namespace BibApp.Models
{
    public class HomeIndexData
    {
        public Exemplar Exemplar { get; set; }
        public Buch.Buch Buch { get; set; }
        public Leihauftrag Leihauftrag { get; set; }

        // ADMIN
        public Dictionary<Leihauftrag, Exemplar> ExemplareAbgelaufen { get; set; }
        public Dictionary<Leihauftrag, Exemplar> ExemplareLaufenBaldAb { get; set; }

        // USER
        public Dictionary<Leihauftrag, Exemplar> ExemplareEntliehen { get; set; }
        public List<Leihauftrag> ExemplareLeihauftragVersendet { get; set; }
    }
}
