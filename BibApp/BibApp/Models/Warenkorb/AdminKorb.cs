using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BibApp.Models.Warenkorb
{
    public class AdminKorb
    {
        [Key]
        public int Id { get; set; }
        public string Benutzer { get; set; }
        public string ISBN { get; set; }
        public int ExemplarId { get; set; }
        public string BuchTitel { get; set; }
    }
}
