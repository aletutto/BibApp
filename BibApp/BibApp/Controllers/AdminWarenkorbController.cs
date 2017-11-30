using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BibApp.Controllers
{
    public class AdminWarenkorbController : Controller
    {
        BibContext context;

        public AdminWarenkorbController(
            BibContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            return View(context.AdminWarenkoerbe.ToList());
        }

        // TODO: Funktion für das finale ausleihen implementieren
        public async Task<IActionResult> Ausleihen()
        {
            return RedirectToAction(nameof(Index));
        }
    }
}