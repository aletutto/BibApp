using System.ComponentModel.DataAnnotations;

namespace BibApp.Models.Warenkorb
{
    public class Leihauftrag
    {
        [Key]
        public int Id { get; set; }

        public string Benutzer { get; set; }

        public string ISBN { get; set; }

        [Display(Name = "Exemplar")]
        public int ExemplarId { get; set; }

        [Display(Name = "Buch")]
        public string BuchTitel { get; set; }

        public bool IstVerliehen { get; set; }
    }
}
