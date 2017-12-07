using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BibApp.Models
{
    public class HomeIndexData
    {
        public List<Benutzer.Benutzer> Benutzers { get; set; }
        public Benutzer.Benutzer Benutzer { get; set; }

        public string Rolle { get; set; }
        public IEnumerable<SelectListItem> Rollen { get; set; }

        public List<Buch.Buch> Buecher { get; set; }
        public Buch.Buch Buch { get; set; }
    }
}
