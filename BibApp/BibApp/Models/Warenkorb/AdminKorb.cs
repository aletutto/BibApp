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
        public string Id { get; set; }
        public string KorbId { get; set; }
        public int BuchId { get; set; }
        public string BuchTitel { get; set; }
    }
}
