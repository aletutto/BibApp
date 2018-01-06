using System;
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
using System.Collections.Generic;

namespace BibApp.Controllers
{
    public class LeihauftragController : Controller
    {
        private readonly BibContext context;
        private readonly UserManager<Benutzer> userManager;
        private readonly IToastNotification toastNotification;

        public LeihauftragController(
            BibContext context,
            UserManager<Benutzer> userManager,
            IToastNotification toastNotification)
        {
            this.context = context;
            this.userManager = userManager;
            this.toastNotification = toastNotification;
        }

        // GET: AdminWarenkorb/Index
        [Authorize(Roles = "Admin")]
        public IActionResult Index(string searchString, string searchString2)
        {
            LeihauftragExemplar model = new LeihauftragExemplar();

            var adminWarenkoerbeAusleihen = context.Leihauftrag.Where(e => e.IstVerliehen == false);
            var adminWarenkoerbeZurückgeben = context.Leihauftrag.Where(e => e.IstVerliehen == true);

            // Suchfeld Ausleihen
            if (!String.IsNullOrEmpty(searchString))
            {
                adminWarenkoerbeAusleihen = adminWarenkoerbeAusleihen.Where(s =>
                s.Benutzer.Contains(searchString)
                || s.BuchTitel.Contains(searchString)
                || s.ISBN.Contains(searchString));
            }
            ViewData["currentFilter"] = searchString;

            // Suchfeld Zurückgeben
            if (!String.IsNullOrEmpty(searchString2))
            {
                adminWarenkoerbeZurückgeben = adminWarenkoerbeZurückgeben.Where(s =>
                s.Benutzer.Contains(searchString2)
                || s.BuchTitel.Contains(searchString2)
                || s.ISBN.Contains(searchString2));
            }
            ViewData["currentFilter2"] = searchString2;

            var zurückgebenDic = new Dictionary<Leihauftrag, Exemplar>();

            foreach (var item in adminWarenkoerbeZurückgeben)
            {
                var exemplar = context.Exemplar.SingleOrDefault(e => e.ISBN == item.ISBN && e.ExemplarId == item.ExemplarId);
                zurückgebenDic.Add(item, exemplar);
            }
            
            model.Ausleihen = adminWarenkoerbeAusleihen.AsNoTracking().ToList();
            model.Zurückgeben = zurückgebenDic;

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Ausleihen(int? id)
        {
            // Sucht der ID nach zugehörigen Warenkorb heraus.
            var adminKorbExemplar = context.Leihauftrag.SingleOrDefault(
                c => c.Id == id);

            var exemplar = context.Exemplar.SingleOrDefault(
                c => c.ISBN == adminKorbExemplar.ISBN 
                && c.ExemplarId == adminKorbExemplar.ExemplarId);

            var buch = await context.Buch.SingleOrDefaultAsync(e => e.ISBN == exemplar.ISBN);

            if (buch == null)
            {
                toastNotification.AddToastMessage("", "Dieses Buch existiert nicht mehr in der Datenbank. Bitte löschen Sie den Leihauftrag.", ToastEnums.ToastType.Error, new ToastOption()
                {
                    PositionClass = ToastPositions.TopCenter
                });
                return RedirectToAction(nameof(Index));
            }

            if (!exemplar.Verfügbarkeit)
            {

                var exemplare = context.Exemplar.Where(e => e.ISBN == buch.ISBN);

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
                    var adminWarenkorb = LeihauftragManager.GetKorb(context);
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
                var adminWarenkorb = LeihauftragManager.GetKorb(context);

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
            var adminKorbExemplar = context.Leihauftrag.SingleOrDefault(
                c => c.Id == id);

            var adminWarenkorb = LeihauftragManager.GetKorb(context);

            await adminWarenkorb.Loeschen(adminKorbExemplar);

            toastNotification.AddToastMessage("Entfernt", "Das Buch \"" + adminKorbExemplar.BuchTitel + "\", welches von \"" + adminKorbExemplar.Benutzer + "\" ausgliehen werden wollte, wurde aus dem Warenkorb entfernt.", ToastEnums.ToastType.Success, new ToastOption()
            {
                PositionClass = ToastPositions.TopCenter
            });

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Zurueckgeben(int? id)
        {
            var adminKorbExemplar = context.Leihauftrag.SingleOrDefault(
                c => c.Id == id);

            var exemplar = context.Exemplar.SingleOrDefault(
                c => c.ISBN == adminKorbExemplar.ISBN
                && c.ExemplarId == adminKorbExemplar.ExemplarId);

            var adminWarenkorb = LeihauftragManager.GetKorb(context);

            await adminWarenkorb.Zurueckgeben(exemplar, adminKorbExemplar);

            toastNotification.AddToastMessage("Zurückgegeben", "Das Buch \"" + adminKorbExemplar.BuchTitel + "\" wurde vom Benutzer \"" + adminKorbExemplar.Benutzer + "\" zurückgegeben!", ToastEnums.ToastType.Success, new ToastOption()
            {
                PositionClass = ToastPositions.TopCenter
            });

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Verlaengern(int? id)
        {
            var adminKorbExemplar = context.Leihauftrag.SingleOrDefault(
                c => c.Id == id);

            var exemplar = context.Exemplar.SingleOrDefault(
                c => c.ISBN == adminKorbExemplar.ISBN
                && c.ExemplarId == adminKorbExemplar.ExemplarId);

            var adminWarenkorb = LeihauftragManager.GetKorb(context);

            await adminWarenkorb.Verlaengern(exemplar);

            toastNotification.AddToastMessage("Verlängert", "Das Buch \"" + adminKorbExemplar.BuchTitel + "\" des Benutzers \"" + adminKorbExemplar.Benutzer + "\" wurde verlängert!", ToastEnums.ToastType.Success, new ToastOption()
            {
                PositionClass = ToastPositions.TopCenter
            });

            return RedirectToAction(nameof(Index));
        }
    }
}