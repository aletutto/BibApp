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
using NToastNotify;
using BibApp.Models.Buch;

namespace BibApp.Controllers
{
    public class AdminWarenkorbController : Controller
    {
        private readonly BibContext context;
        private readonly UserManager<Benutzer> userManager;
        private readonly IToastNotification toastNotification;

        public AdminWarenkorbController(
            BibContext context,
            UserManager<Benutzer> userManager,
            IToastNotification toastNotification)
        {
            this.context = context;
            this.userManager = userManager;
            this.toastNotification = toastNotification;
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

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Ausleihen(int? id)
        {
            var adminKorbExemplar = context.AdminWarenkoerbe.SingleOrDefault(
                c => c.Id == id);

            var exemplar = context.Exemplare.SingleOrDefault(
                c => c.ISBN == adminKorbExemplar.ISBN 
                && c.ExemplarId == adminKorbExemplar.ExemplarId);

            var buch = await context.Buecher.SingleOrDefaultAsync(e => e.ISBN == exemplar.ISBN);

            if (!exemplar.Verfügbarkeit)
            {

                var exemplare = context.Exemplare.Where(e => e.ISBN == buch.ISBN);

                Exemplar gesuchtesExemplar = null;

                foreach (var exemplarSuchen in exemplare)
                {
                    if (exemplarSuchen.Verfügbarkeit)
                    {
                        gesuchtesExemplar = exemplarSuchen;
                        break;
                    }
                }

                if (gesuchtesExemplar != null)
                {
                    var oldExemplarId = exemplar.ExemplarId;

                    var user = await userManager.GetUserAsync(User);
                    var adminWarenkorb = AdminWarenkorb.GetKorb(user, context);

                    adminKorbExemplar.ExemplarId = gesuchtesExemplar.ExemplarId;

                    await adminWarenkorb.Ausleihen(gesuchtesExemplar, adminKorbExemplar);

                    toastNotification.AddToastMessage("Anderes Exemplar verliehen", "Das Exemplar " + oldExemplarId + " ist bereits verliehen! Es wurde nun das Exemplar " + gesuchtesExemplar.ExemplarId + " des Buches \"" + buch.Titel + "\" verliehen.", ToastEnums.ToastType.Warning, new ToastOption()
                    {
                        PositionClass = ToastPositions.TopCenter
                    });

                    return RedirectToAction(nameof(Index));
                }

                toastNotification.AddToastMessage("Fehler", "Es sind bereits alle Exemplare des Buches \"" + buch.Titel + "\" verliehen!", ToastEnums.ToastType.Error, new ToastOption()
                {
                    PositionClass = ToastPositions.TopCenter
                });

                return RedirectToAction(nameof(Index));
            }
            else
            {
                var user = await userManager.GetUserAsync(User);
                var adminWarenkorb = AdminWarenkorb.GetKorb(user, context);

                await adminWarenkorb.Ausleihen(exemplar, adminKorbExemplar);

                toastNotification.AddToastMessage("", "Das Buch \"" + buch.Titel + "\" wurde verliehen!", ToastEnums.ToastType.Success, new ToastOption()
                {
                    PositionClass = ToastPositions.TopCenter
                });

                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Loeschen(int? id)
        {
            var adminKorbExemplar = context.AdminWarenkoerbe.SingleOrDefault(
                c => c.Id == id);

            var user = await userManager.GetUserAsync(User);
            var adminWarenkorb = AdminWarenkorb.GetKorb(user, context);

            await adminWarenkorb.Loeschen(adminKorbExemplar);

            toastNotification.AddToastMessage("Entfernt", "Das Buch \"" + adminKorbExemplar.BuchTitel + "\", welches von \"" + user.UserName + "\" ausgliehen werden wollte, wurde aus dem Warenkorb entfernt.", ToastEnums.ToastType.Success, new ToastOption()
            {
                PositionClass = ToastPositions.TopCenter
            });

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

            toastNotification.AddToastMessage("Zurückgegeben", "Das Buch\"" + adminKorbExemplar.BuchTitel + "\" wurde vom Benutzer \"" + user.UserName + "\" zurückgegeben!", ToastEnums.ToastType.Success, new ToastOption()
            {
                PositionClass = ToastPositions.TopCenter
            });

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

            toastNotification.AddToastMessage("Verlängert", "Das Buch\"" + adminKorbExemplar.BuchTitel + "\" des Benutzers \"" + user.UserName + "\" wurde verlängert!", ToastEnums.ToastType.Success, new ToastOption()
            {
                PositionClass = ToastPositions.TopCenter
            });

            return RedirectToAction(nameof(Index));
        }
    }
}