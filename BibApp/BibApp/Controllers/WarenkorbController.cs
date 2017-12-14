using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BibApp.Models.Warenkorb;
using BibApp.Models.Benutzer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BibApp.Controllers
{
    public class WarenkorbController : Controller
    {
        BibContext context;
        private readonly UserManager<Benutzer> userManager;

        public WarenkorbController(
            BibContext context,
            UserManager<Benutzer> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            return View(context.Warenkoerbe.ToList());
        }


        // GET: Warenkorb/RemoveFromKorb
        public async Task<IActionResult> RemoveFromKorb(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await context.Warenkoerbe.SingleOrDefaultAsync(i => i.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            var user = await userManager.GetUserAsync(User);
            var korb = Warenkorb.GetKorb(user, context);
            await korb.RemoveFromKorb(item);

            return RedirectToAction(nameof(Index));
        }

        // GET: Warenkorb/RemoveAllFromKorb
        public async Task<IActionResult> RemoveAllFromKorb()
        {
            var user = await userManager.GetUserAsync(User);
            var korb = Warenkorb.GetKorb(user, context);
            await korb.RemoveAllFromKorb();

            return RedirectToAction(nameof(Index));
        }

        // GET: Warenkorb/Abschicken
        /* TODO: WICHTIG: Nur Bücher in den Warenkorb einfügen können, welche verfügbar, also noch nicht ausgeliehen worden sind.
         * Nur Bücher verschicken können, welche noch nicht im AdminWarenkorb vorhanden sind.
         * 
        */
        public async Task<IActionResult> LeihauftragSenden()
        {
            var user = await userManager.GetUserAsync(User);
            var korb = Warenkorb.GetKorb(user, context);
            await korb.LeihauftragSenden();

            return RedirectToAction(nameof(Index));
        }
    }
}