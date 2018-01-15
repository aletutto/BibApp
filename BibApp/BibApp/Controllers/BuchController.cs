using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BibApp.Models.Benutzer;
using BibApp.Models.Warenkorb;
using BibApp.Models.Buch;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using NToastNotify;

namespace BibApp.Controllers
{
    public class BuchController : Controller
    {
        private readonly BibContext bibContext;
        private readonly UserManager<Benutzer> userManager;
        private readonly IToastNotification toastNotification;

        public BuchController(
            BibContext bibContext,
            UserManager<Benutzer> userManager,
            IToastNotification toastNotification)
        {
            this.bibContext = bibContext;
            this.userManager = userManager;
            this.toastNotification = toastNotification;
        }

        // GET: Buch/Index
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            BuchExemplar model = new BuchExemplar();

            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["AutSortParm"] = sortOrder == "Autor" ? "aut_desc" : "Autor";
            ViewData["IsbnSortParm"] = sortOrder == "ISBN" ? "isbn_desc" : "ISBN";
            ViewData["ErschSortParm"] = sortOrder == "Erscheinung" ? "ersch_desc" : "Erscheinung";
            ViewData["VerlagSortParm"] = sortOrder == "Verlag" ? "verlag_desc" : "Verlag";
            ViewData["CurrentFilter"] = searchString;

            var bucher = from s in bibContext.Buch
                        select s;

            var exemplare = from s in bibContext.Exemplar
                            select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                bucher = bucher.Where(s =>
                s.Titel.Contains(searchString)
                || s.Autor.Contains(searchString)
                || s.Verlag.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "aut_desc":
                    bucher = bucher.OrderByDescending(s => s.Autor);
                    break;
                case "Autor":
                    bucher = bucher.OrderBy(s => s.Autor);
                    break;
                case "name_desc":
                    bucher = bucher.OrderByDescending(s => s.Titel);
                    break;
                case "isbn_desc":
                    bucher = bucher.OrderByDescending(s => s.ISBN);
                    break;
                case "ISBN":
                    bucher = bucher.OrderBy(s => s.ISBN);
                    break;
                case "ersch_desc":
                    bucher = bucher.OrderByDescending(s => s.Erscheinungsjahr);
                    break;
                case "Erscheinung":
                    bucher = bucher.OrderBy(s => s.Erscheinungsjahr);
                    break;
                case "verlag_desc":
                    bucher = bucher.OrderByDescending(s => s.Verlag);
                    break;
                case "Verlag":
                    bucher = bucher.OrderBy(s => s.Verlag);
                    break;
                default:
                    bucher = bucher.OrderBy(s => s.Titel);
                    break;
            }

            model.Exemplare = await exemplare.AsNoTracking().ToListAsync();
            model.Buecher = await bucher.AsNoTracking().ToListAsync();

            return View(model);
            
        }

        // GET: Buch/Details/x
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            var buch = await bibContext.Buch
                .SingleOrDefaultAsync(m => m.Id == id);

            return View(buch);
        }

        // GET: Buch/Create
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Buch/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("ISBN, Titel, Autor, Verlag, Erscheinungsjahr, Regal, Reihe, AnzahlExemplare")] Buch buch)
        {
            if (ModelState.IsValid)
            {
                var buchVorhanden = await bibContext.Buch.SingleOrDefaultAsync(m => m.ISBN == buch.ISBN);

                // Fall: Buch ist noch nicht in DB vorhanden
                if (buchVorhanden == null)
                {
                    bibContext.Add(buch);

                    // Erzeuge die Exemplare für das Buch
                    for (int i = 1; i <= buch.AnzahlExemplare; i++)
                    {
                        var exemplar = new Exemplar { ExemplarId = i, ISBN = buch.ISBN, Verfügbarkeit = true };
                        bibContext.Exemplar.Add(exemplar);
                    }

                    await bibContext.SaveChangesAsync();

                    toastNotification.AddToastMessage("Buch erstellt", "Das Buch \"" + buch.Titel + "\" wurde erstellt!", ToastEnums.ToastType.Success, new ToastOption()
                    {
                        PositionClass = ToastPositions.TopCenter
                    });

                    return RedirectToAction(nameof(Index));
                }
                else // Fall: Buch ist bereits in DB vorhanden
                {
                    toastNotification.AddToastMessage("Buch bereits vorhanden", "Das Buch mit der ISBN " + buch.ISBN + " existiert bereits in den Stammdaten!", ToastEnums.ToastType.Error, new ToastOption()
                    {
                        PositionClass = ToastPositions.TopCenter
                    });

                    return View(buch);
                }                
            }
            return View(buch);
        }

        // GET: Buch/Edit
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            var buch = await bibContext.Buch.SingleOrDefaultAsync(m => m.Id == id);
            return View(buch);
        }

        // POST: Buch/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Buch buch)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var buchVorher = await bibContext.Buch.AsNoTracking().SingleOrDefaultAsync(e => e.Id == id);
                    var anzahlExemplareVorher = buchVorher.AnzahlExemplare;

                    // Fall: Die Anzahl der Exemplare wurde erhöht
                    if (anzahlExemplareVorher < buch.AnzahlExemplare)
                    {
                        var Differenz = buch.AnzahlExemplare - anzahlExemplareVorher;

                        for (int i = 1; i <= Differenz; i++)
                        {
                            var exemplar = new Exemplar { ExemplarId = anzahlExemplareVorher + i, ISBN = buchVorher.ISBN, Verfügbarkeit = true };
                            bibContext.Exemplar.Add(exemplar);
                        }

                    }

                    // Fall: Die Anzahl der Exemplare wurde verringert
                    if (anzahlExemplareVorher > buch.AnzahlExemplare)
                    {
                        var differenz = anzahlExemplareVorher - buch.AnzahlExemplare;

                        for (int i = 0; i < differenz; i++)
                        {
                            var exemplar = await bibContext.Exemplar.SingleOrDefaultAsync(e => e.ISBN == buchVorher.ISBN && e.ExemplarId == anzahlExemplareVorher - i);
                            if (!exemplar.Verfügbarkeit) // Fall: Exemplar noch verliehen
                            {
                                toastNotification.AddToastMessage("Exemplar noch verliehen", "Das Exemplar ist noch verliehen und kann daher nicht gelöscht werden!", ToastEnums.ToastType.Error, new ToastOption()
                                {
                                    PositionClass = ToastPositions.TopCenter
                                });

                                return RedirectToAction(nameof(Edit));
                            }
                            else
                            {
                                bibContext.Exemplar.Remove(exemplar);
                            }
                        }
                    }

                    var warenkorbExemplare = bibContext.Warenkorb.Where(w => 
                    w.BuchTitel.Equals(buchVorher.Titel));

                    foreach(var exemplar in warenkorbExemplare)
                    {
                        exemplar.BuchTitel = buch.Titel;
                        bibContext.Update(exemplar);
                    }

                    var leihauftraege = bibContext.Leihauftrag.Where(w =>
                        w.BuchTitel.Equals(buchVorher.Titel));

                    foreach (var leihauftrag in leihauftraege)
                    {
                        leihauftrag.BuchTitel = buch.Titel;
                        bibContext.Update(leihauftrag);
                    }

                    bibContext.Update(buch);
                    await bibContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }

                toastNotification.AddToastMessage("Buch geändert", "Das Buch \"" + buch.Titel + "\" wurde geändert!", ToastEnums.ToastType.Success, new ToastOption()
                {
                    PositionClass = ToastPositions.TopCenter
                });

                return RedirectToAction(nameof(Index));
            }
            return View(buch);
        }

        // GET: Buch/Delete
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            var buch = await bibContext.Buch
                .SingleOrDefaultAsync(m => m.Id == id);

            return View(buch);
        }

        // POST: Buch/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var buch = await bibContext.Buch.SingleOrDefaultAsync(m => m.Id == id);
            var exemplare = bibContext.Exemplar.Where(e => e.ISBN == buch.ISBN);
            bool istEinExemplarVerliehen = false;

            foreach (var exemplar in exemplare)
            {
                if (!exemplar.Verfügbarkeit)
                {
                    istEinExemplarVerliehen = true;
                    break;
                }
            }

            if (!istEinExemplarVerliehen) // Fall: Buch wird gelöscht
            {
                bibContext.Buch.Remove(buch);
                await bibContext.SaveChangesAsync();

                toastNotification.AddToastMessage("Buch gelöscht", "Das Buch \"" + buch.Titel + "\" wurde gelöscht!", ToastEnums.ToastType.Success, new ToastOption()
                {
                    PositionClass = ToastPositions.TopCenter
                });

            }
            else // Fall: Buch kann nicht gelöscht werden
            {
                toastNotification.AddToastMessage("Buch kann nicht gelöscht werden", "Ein Exemplar des Buch \"" + buch.Titel + "\" ist noch verliehen, daher kann das Buch nicht gelöscht werden!", ToastEnums.ToastType.Error, new ToastOption()
                {
                    PositionClass = ToastPositions.TopCenter
                });

            }
            return RedirectToAction(nameof(Index));
        }

        // Legt ein Exemplar in den Warenkorb
        public async Task<IActionResult> InDenWarenkorb(int? id)
        {
            var exemplar = await bibContext.Exemplar.SingleOrDefaultAsync(e => e.Id == id);
            var user = await userManager.GetUserAsync(User);
            var buchBereitsInWarenkorb = bibContext.Warenkorb.SingleOrDefault(b => b.ISBN == exemplar.ISBN && b.Benutzer == user.UserName);
            var buch = await bibContext.Buch.SingleOrDefaultAsync(e => e.ISBN == exemplar.ISBN);

            if (buchBereitsInWarenkorb == null) // Fall: Das Buch befindet sich noch nicht im Warenkorb und wird ihm hinzugefügt
            {
                var warenkorbManager = WarenkorbManager.GetWarenkorbManager(user, bibContext);
                await warenkorbManager.InDenWarenkorb(exemplar);

                toastNotification.AddToastMessage("Buch in Warenkorb", "Das Buch \"" + buch.Titel + "\" wurde dem Warenkorb hinzugefügt!", ToastEnums.ToastType.Success, new ToastOption()
                {
                    PositionClass = ToastPositions.TopCenter
                });
            }
            else // Fall: Das Buch befindet sich bereits im Warenkorb
            {
                toastNotification.AddToastMessage("Bereits im Warenkorb", "Das Buch \"" + buch.Titel + "\" befindet sich bereits im Warenkorb!", ToastEnums.ToastType.Warning, new ToastOption()
                {
                    PositionClass = ToastPositions.TopCenter
                });
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
