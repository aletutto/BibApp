using BibApp.Models.Buch;
using System.Collections.Generic;

namespace BibApp.Models.Warenkorb
{
    public class AdminWarenkorbModel
    {
        public AdminKorb AdminKorb { get; set; }
        public Exemplar Exemplar { get; set; }

        public List<AdminKorb> Ausleihen { get; set; }

        public Dictionary<AdminKorb, Exemplar> Zurückgeben { get; set; }
    }
}
