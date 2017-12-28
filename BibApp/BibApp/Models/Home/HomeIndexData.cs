using BibApp.Models.Buch;
using BibApp.Models.Warenkorb;
using System.Collections.Generic;

namespace BibApp.Models
{
    public class HomeIndexData
    {
        // TODO: LÖSCHEN
        public List<Exemplar> Exemplare { get; set; }
        public Exemplar Exemplar { get; set; }

        public List<Buch.Buch> Buecher { get; set; }
        public Buch.Buch Buch { get; set; }

        public List<AdminKorb> AdminKoerbe { get; set; }
        public AdminKorb AdminKorb { get; set; }

        // ADMIN
        public Dictionary<AdminKorb, Exemplar> ExemplareAbgelaufen { get; set; }
        public Dictionary<AdminKorb, Exemplar> ExemplareLaufenBaldAb { get; set; }

        // USER
        public Dictionary<AdminKorb, Exemplar> ExemplareEntliehen { get; set; }
        public List<AdminKorb> ExemplareLeihauftragVersendet { get; set; }
    }
}
