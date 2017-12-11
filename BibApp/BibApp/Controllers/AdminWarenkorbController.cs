using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BibApp.Models.Warenkorb;
using Microsoft.AspNetCore.Identity;
using BibApp.Models.Benutzer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

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
        [Authorize(Roles = "Admin")]
        public IActionResult Index(string searchString)
        {
            var adminWarenkoerbe = from s in context.AdminWarenkoerbe
                        select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                adminWarenkoerbe = adminWarenkoerbe.Where(s =>
                s.Benutzer.Contains(searchString)
                || s.BuchTitel.Contains(searchString)
                || s.ISBN.Contains(searchString));
            }
            ViewData["currentFilter"] = searchString;

            return View(adminWarenkoerbe.AsNoTracking().ToList()); 
        }

        // TODO: Funktion für das finale ausleihen implementieren
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Ausleihen(int? id)
        {
            var adminKorbExemplar = context.AdminWarenkoerbe.SingleOrDefault(
                c => c.Id == id);

            var exemplar = context.Exemplare.SingleOrDefault(
                c => c.ISBN == adminKorbExemplar.ISBN 
                && c.ExemplarId == adminKorbExemplar.ExemplarId);

            var user = await userManager.GetUserAsync(User);
            var adminWarenkorb = AdminWarenkorb.GetKorb(user, context);

            await adminWarenkorb.Ausleihen(exemplar, adminKorbExemplar);

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Loeschen(int? id)
        {
            var adminKorbExemplar = context.AdminWarenkoerbe.SingleOrDefault(
                c => c.Id == id);

            var user = await userManager.GetUserAsync(User);
            var adminWarenkorb = AdminWarenkorb.GetKorb(user, context);

            await adminWarenkorb.Loeschen(adminKorbExemplar);

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Zurueckgeben(int? id)
        {
            var adminKorbExemplar = context.AdminWarenkoerbe.SingleOrDefault(
                c => c.Id == id);

            var exemplar = context.Exemplare.SingleOrDefault(
                c => c.ISBN == adminKorbExemplar.ISBN
                && c.ExemplarId == adminKorbExemplar.ExemplarId);

            var user = await userManager.GetUserAsync(User);
            var adminWarenkorb = AdminWarenkorb.GetKorb(user, context);

            await adminWarenkorb.Zurueckgeben(exemplar, adminKorbExemplar);

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Verlaengern(int? id)
        {
            var adminKorbExemplar = context.AdminWarenkoerbe.SingleOrDefault(
                c => c.Id == id);

            var exemplar = context.Exemplare.SingleOrDefault(
                c => c.ISBN == adminKorbExemplar.ISBN
                && c.ExemplarId == adminKorbExemplar.ExemplarId);

            var user = await userManager.GetUserAsync(User);
            var adminWarenkorb = AdminWarenkorb.GetKorb(user, context);

            await adminWarenkorb.Verlaengern(exemplar);

            return RedirectToAction(nameof(Index));
        }
    }
}