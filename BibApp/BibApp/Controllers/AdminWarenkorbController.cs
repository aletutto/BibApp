using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BibApp.Models.Warenkorb;
using Microsoft.AspNetCore.Identity;
using BibApp.Models.Benutzer;

namespace BibApp.Controllers
{
    public class AdminWarenkorbController : Controller
    {
        BibContext context;
        private readonly UserManager<Benutzer> userManager;

        public AdminWarenkorbController(
            BibContext context,
            UserManager<Benutzer> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }
        public IActionResult Index()
        {
            return View(context.AdminWarenkoerbe.ToList());
        }

        // TODO: Funktion für das finale ausleihen implementieren
        public async Task<IActionResult> Ausleihen(int? id)
        {
            var adminKorbExemplar = context.AdminWarenkoerbe.SingleOrDefault(
                c => c.Id == id);

            var exemplar = context.Exemplare.SingleOrDefault(
                c => c.ISBN == adminKorbExemplar.ISBN 
                && c.ExemplarId == adminKorbExemplar.ExemplarId);

            var user = await userManager.GetUserAsync(User);
            var adminKorb = AdminWarenkorb.GetKorb(user, context);

            await adminKorb.Ausleihen(exemplar);

            return RedirectToAction(nameof(Index));
        }
    }
}