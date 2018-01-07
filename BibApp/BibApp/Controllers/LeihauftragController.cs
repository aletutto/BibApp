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
        private readonly BibContext bibContext;
        private readonly UserManager<Benutzer> userManager;
        private readonly IToastNotification toastNotification;

        public LeihauftragController(
            BibContext bibContext,
            UserManager<Benutzer> userManager,
            IToastNotification toastNotification)
        {
            this.bibContext = bibContext;
            this.userManager = userManager;
            this.toastNotification = toastNotification;
        }

        // GET: Leihauftrag/Index
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Index(string searchString, string searchString2)
        {
            LeihauftragExemplar model = new LeihauftragExemplar();

            var leihauftragAusleihen = bibContext.Leihauftrag.Where(e => e.IstVerliehen == false);
            var leihauftragZurückgeben = bibContext.Leihauftrag.Where(e => e.IstVerliehen == true);

            // Suchfeld Ausleihen
            if (!String.IsNullOrEmpty(searchString))
            {
                leihauftragAusleihen = leihauftragAusleihen.Where(s =>
                s.Benutzer.Contains(searchString)
                || s.BuchTitel.Contains(searchString)
                || s.ISBN.Contains(searchString));
            }
            ViewData["currentFilter"] = searchString;

            // Suchfeld Zurückgeben
            if (!String.IsNullOrEmpty(searchString2))
            {
                leihauftragZurückgeben = leihauftragZurückgeben.Where(s =>
                s.Benutzer.Contains(searchString2)
                || s.BuchTitel.Contains(searchString2)
                || s.ISBN.Contains(searchString2));
            }
            ViewData["currentFilter2"] = searchString2;

            var leihauftragZurückgebenDic = new Dictionary<Leihauftrag, Exemplar>();

            foreach (var leihauftrag in leihauftragZurückgeben)
            {
                var exemplar = bibContext.Exemplar.SingleOrDefault(e => e.ISBN == leihauftrag.ISBN && e.ExemplarId == leihauftrag.ExemplarId);
                leihauftragZurückgebenDic.Add(leihauftrag, exemplar);
            }
            
            model.Ausleihen = leihauftragAusleihen.AsNoTracking().ToList();
            model.Zurückgeben = leihauftragZurückgebenDic;

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Ausleihen(int? id)
        {
            // Sucht der ID nach zugehörigen Warenkorb heraus.
            var leihauftrag = bibContext.Leihauftrag.SingleOrDefault(
                c => c.Id == id);

            var exemplar = bibContext.Exemplar.SingleOrDefault(
                c => c.ISBN == leihauftrag.ISBN 
                && c.ExemplarId == leihauftrag.ExemplarId);

            var buch = await bibContext.Buch.SingleOrDefaultAsync(e => e.ISBN == exemplar.ISBN);

            if (buch == null) // Fall: Buch wurde mitlerweile gelöscht
            {
                toastNotification.AddToastMessage("Buch gelöscht", "Dieses Buch existiert nicht mehr in der Datenbank. Bitte löschen Sie den Leihauftrag.", ToastEnums.ToastType.Error, new ToastOption()
                {
                    PositionClass = ToastPositions.TopCenter
                });
                return RedirectToAction(nameof(Index));
            }

            if (!exemplar.Verfügbarkeit)
            {
                var exemplare = bibContext.Exemplar.Where(e => e.ISBN == buch.ISBN);

                Exemplar gesuchtesExemplar = null;

                // Wenn ein das Exemplar eines Buches bereits verliehen ist, suche ein freies Exemplar des Buches
                foreach (var exemplarSuchen in exemplare)
                {
                    if (exemplarSuchen.Verfügbarkeit)
                    {
                        gesuchtesExemplar = exemplarSuchen;
                        break;
                    }
                }

                if (gesuchtesExemplar != null) // Fall: Exemplar bereits verliehen, jedoch wurde ein anderes Exemplar des Buches gefunden und verliehen
                {
                    var oldExemplarId = exemplar.ExemplarId;
                    var leihauftragManager = LeihauftragManager.GetLeihauftragManager(bibContext);
                    leihauftrag.ExemplarId = gesuchtesExemplar.ExemplarId;
                    
                    await leihauftragManager.Ausleihen(gesuchtesExemplar, leihauftrag);

                    toastNotification.AddToastMessage("Anderes Exemplar verliehen", "Das Exemplar " + oldExemplarId + " ist bereits verliehen! Es wurde nun das Exemplar " + gesuchtesExemplar.ExemplarId + " des Buches \"" + buch.Titel + "\" verliehen.", ToastEnums.ToastType.Warning, new ToastOption()
                    {
                        PositionClass = ToastPositions.TopCenter
                    });

                    return RedirectToAction(nameof(Index));
                }

                // Wenn das Programm bis hierhin kommt, sind bereits alle Exemplare des Buches verliehen
                toastNotification.AddToastMessage("Alle Exemplare verliehen", "Es sind bereits alle Exemplare des Buches \"" + buch.Titel + "\" verliehen.", ToastEnums.ToastType.Error, new ToastOption()
                {
                    PositionClass = ToastPositions.TopCenter
                });

                return RedirectToAction(nameof(Index));
            }
            else // Fall: Das Buch wurde verliehen
            {
                var leihauftragManager = LeihauftragManager.GetLeihauftragManager(bibContext);
                await leihauftragManager.Ausleihen(exemplar, leihauftrag);

                toastNotification.AddToastMessage("Buch verliehen", "Das Buch \"" + buch.Titel + "\" wurde verliehen.", ToastEnums.ToastType.Success, new ToastOption()
                {
                    PositionClass = ToastPositions.TopCenter
                });

                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Loeschen(int? id)
        {
            var leihauftrag = bibContext.Leihauftrag.SingleOrDefault(
                c => c.Id == id);

            var leihauftragManager = LeihauftragManager.GetLeihauftragManager(bibContext);
            await leihauftragManager.Loeschen(leihauftrag);

            toastNotification.AddToastMessage("Leihauftrag entfernt", "Das Buch \"" + leihauftrag.BuchTitel + "\", welches von \"" + leihauftrag.Benutzer + "\" ausgliehen werden wollte, wurde aus den Leihaufträgen entfernt.", ToastEnums.ToastType.Success, new ToastOption()
            {
                PositionClass = ToastPositions.TopCenter
            });

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Zurueckgeben(int? id)
        {
            var leihauftrag = bibContext.Leihauftrag.SingleOrDefault(
                c => c.Id == id);

            var exemplar = bibContext.Exemplar.SingleOrDefault(
                c => c.ISBN == leihauftrag.ISBN
                && c.ExemplarId == leihauftrag.ExemplarId);

            var leihauftragManager = LeihauftragManager.GetLeihauftragManager(bibContext);
            await leihauftragManager.Zurueckgeben(exemplar, leihauftrag);

            toastNotification.AddToastMessage("Buch zurückgegeben", "Das Buch \"" + leihauftrag.BuchTitel + "\" wurde vom Benutzer \"" + leihauftrag.Benutzer + "\" zurückgegeben.", ToastEnums.ToastType.Success, new ToastOption()
            {
                PositionClass = ToastPositions.TopCenter
            });

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Verlaengern(int? id)
        {
            var leihauftrag = bibContext.Leihauftrag.SingleOrDefault(
                c => c.Id == id);

            var exemplar = bibContext.Exemplar.SingleOrDefault(
                c => c.ISBN == leihauftrag.ISBN
                && c.ExemplarId == leihauftrag.ExemplarId);

            var leihauftragManager = LeihauftragManager.GetLeihauftragManager(bibContext);
            await leihauftragManager.Verlaengern(exemplar);

            toastNotification.AddToastMessage("Buch verlängert", "Das Buch \"" + leihauftrag.BuchTitel + "\" des Benutzers \"" + leihauftrag.Benutzer + "\" wurde verlängert.", ToastEnums.ToastType.Success, new ToastOption()
            {
                PositionClass = ToastPositions.TopCenter
            });

            return RedirectToAction(nameof(Index));
        }
    }
}