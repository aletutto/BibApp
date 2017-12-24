using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BibApp.Models.Benutzer;
using BibApp.Models.Warenkorb;
using BibApp.Models.Buch;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using NToastNotify;

namespace BibApp.Controllers
{
    public class BuecherController : Controller
    {
        private readonly BibContext context;
        private readonly UserManager<Benutzer> userManager;
        private readonly IToastNotification toastNotification;

        public BuecherController(BibContext context, UserManager<Benutzer> userManager, IToastNotification toastNotification)
        {
            this.context = context;
            this.userManager = userManager;
            this.toastNotification = toastNotification;
        }

        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            BuchExemplar model = new BuchExemplar();

            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["AutSortParm"] = sortOrder == "Autor" ? "aut_desc" : "Autor";
            ViewData["IsbnSortParm"] = sortOrder == "ISBN" ? "isbn_desc" : "ISBN";
            ViewData["ErschSortParm"] = sortOrder == "Erscheinung" ? "ersch_desc" : "Erscheinung";
            ViewData["VerlagSortParm"] = sortOrder == "Verlag" ? "verlag_desc" : "Verlag";
            ViewData["CurrentFilter"] = searchString;

            var books = from s in context.Buecher
                           select s;

            var exemplare = from s in context.Exemplare
                           select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                books = books.Where(s => 
                s.Titel.Contains(searchString) 
                || s.Autor.Contains(searchString)
                || s.Verlag.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "aut_desc":
                    books = books.OrderByDescending(s => s.Autor);
                    break;
                case "Autor":
                    books = books.OrderBy(s => s.Autor);
                    break;
                case "name_desc":
                    books = books.OrderByDescending(s => s.Titel);
                    break;
                case "isbn_desc":
                    books = books.OrderByDescending(s => s.ISBN);
                    break;
                case "ISBN":
                    books = books.OrderBy(s => s.ISBN);
                    break;
                case "ersch_desc":
                    books = books.OrderByDescending(s => s.Erscheinungsjahr);
                    break;
                case "Erscheinung":
                    books = books.OrderBy(s => s.Erscheinungsjahr);
                    break;
                case "verlag_desc":
                    books = books.OrderByDescending(s => s.Verlag);
                    break;
                case "Verlag":
                    books = books.OrderBy(s => s.Verlag);
                    break;
                default:
                    books = books.OrderBy(s => s.Titel);
                    break;
            }

            model.Exemplare = await exemplare.AsNoTracking().ToListAsync();
            model.Buecher = await books.AsNoTracking().ToListAsync();

            return View(model);
        }

        // GET: Buecher/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var buch = await context.Buecher
                .SingleOrDefaultAsync(m => m.Id == id);
            if (buch == null)
            {
                return NotFound();
            }

            return View(buch);
        }

        // GET: Buecher/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Buecher/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("ISBN, Titel, Autor, Verlag, Erscheinungsjahr, Regal, Reihe, AnzahlExemplare")] Buch buch)
        {
            if (ModelState.IsValid)
            {
                var buchVorhanden = await context.Buecher.SingleOrDefaultAsync(m => m.ISBN == buch.ISBN);

                if (buchVorhanden == null)
                {
                    context.Add(buch);

                    for (int i = 1; i <= buch.AnzahlExemplare; i++)
                    {
                        var exemplar = new Exemplar { ExemplarId = i, ISBN = buch.ISBN, Verfügbarkeit = true, IstVorgemerkt = false };
                        context.Exemplare.Add(exemplar);
                    }

                    await context.SaveChangesAsync();

                    toastNotification.AddToastMessage("", "Das Buch \"" + buch.Titel + "\" wurde erstellt!", ToastEnums.ToastType.Success, new ToastOption()
                    {
                        PositionClass = ToastPositions.TopCenter
                    });

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    toastNotification.AddToastMessage("Fehler", "Das Buch mit der ISBN " + buch.ISBN + " existiert bereits in den Stammdaten!", ToastEnums.ToastType.Error, new ToastOption()
                    {
                        PositionClass = ToastPositions.TopCenter
                    });

                    return View(buch);
                }                
            }
            return View(buch);
        }

        // GET: Buecher/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var buch = await context.Buecher.SingleOrDefaultAsync(m => m.Id == id);
            if (buch == null)
            {
                return NotFound();
            }
            return View(buch);
        }

        // POST: Buecher/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Buch buch)
        {
            if (id != buch.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    var buchVorher = await context.Buecher.AsNoTracking().SingleOrDefaultAsync(e => e.Id == id);
                    var AnzahlExemplareVorher = buchVorher.AnzahlExemplare;

                    if (AnzahlExemplareVorher < buch.AnzahlExemplare)
                    {
                        var Differenz = buch.AnzahlExemplare - AnzahlExemplareVorher;

                        for (int i = 1; i <= Differenz; i++)
                        {
                            var exemplar = new Exemplar { ExemplarId = AnzahlExemplareVorher + i, ISBN = buchVorher.ISBN, Verfügbarkeit = true, IstVorgemerkt = false };
                            context.Exemplare.Add(exemplar);
                        }

                    }

                    if (AnzahlExemplareVorher > buch.AnzahlExemplare)
                    {
                        var Differenz = AnzahlExemplareVorher - buch.AnzahlExemplare;

                        for (int i = 0; i < Differenz; i++)
                        {
                            var exemplar = await context.Exemplare.SingleOrDefaultAsync(e => e.ISBN == buchVorher.ISBN && e.ExemplarId == AnzahlExemplareVorher - i);
                            if (!exemplar.Verfügbarkeit)
                            {
                                toastNotification.AddToastMessage("Fehler", "Das Exemplar ist noch verliehen und kann daher nicht gelöscht werden!", ToastEnums.ToastType.Error, new ToastOption()
                                {
                                    PositionClass = ToastPositions.TopCenter
                                });

                                return RedirectToAction(nameof(Edit));
                            }
                            else
                            {
                                context.Exemplare.Remove(exemplar);
                            }
                            
                        }

                    }

                    var exemplare = context.Exemplare.Where(e => e.ISBN == buchVorher.ISBN);

                    foreach (var exemplar in exemplare)
                    {
                        exemplar.ISBN = buch.ISBN; 
                    }

                    context.Update(buch);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BuchExists(buch.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                toastNotification.AddToastMessage("", "Das Buch \"" + buch.Titel + "\" wurde geändert!", ToastEnums.ToastType.Success, new ToastOption()
                {
                    PositionClass = ToastPositions.TopCenter
                });

                return RedirectToAction(nameof(Index));
            }
            return View(buch);
        }

        // GET: Buecher/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var buch = await context.Buecher
                .SingleOrDefaultAsync(m => m.Id == id);
            if (buch == null)
            {
                return NotFound();
            }

            return View(buch);
        }

        // POST: Buecher/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var buch = await context.Buecher.SingleOrDefaultAsync(m => m.Id == id);
            context.Buecher.Remove(buch);
            await context.SaveChangesAsync();

            toastNotification.AddToastMessage("", "Das Buch \"" + buch.Titel + "\" wurde gelöscht!", ToastEnums.ToastType.Success, new ToastOption()
            {
                PositionClass = ToastPositions.TopCenter
            });

            return RedirectToAction(nameof(Index));
        }

        private bool BuchExists(int id)
        {
            return context.Buecher.Any(e => e.Id == id);
        }

        // GET: Buecher/AddToCart
        public async Task<IActionResult> AddToCart(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var exemplar = await context.Exemplare.SingleOrDefaultAsync(e => e.Id == id);

            if (exemplar == null)
            {
                return NotFound();
            }

            var user = await userManager.GetUserAsync(User);
            var buchVorhanden = context.Warenkoerbe.SingleOrDefault(b => b.ISBN == exemplar.ISBN && b.Benutzer == user.UserName);
            var buch = await context.Buecher.SingleOrDefaultAsync(e => e.ISBN == exemplar.ISBN);

            if (buchVorhanden == null)
            {
                var korb = Warenkorb.GetKorb(user, context);
                await korb.AddToKorb(exemplar);

                toastNotification.AddToastMessage("", "Das Buch \"" + buch.Titel + "\" wurde dem Warenkorb hinzugefügt!", ToastEnums.ToastType.Success, new ToastOption()
                {
                    PositionClass = ToastPositions.TopCenter
                });
            }
            else
            {
                toastNotification.AddToastMessage("Warnung", "Das Buch \"" + buch.Titel + "\" befindet sich bereits im Warenkorb!", ToastEnums.ToastType.Warning, new ToastOption()
                {
                    PositionClass = ToastPositions.TopCenter
                });
            }

            return RedirectToAction(nameof(Index));
        }

        // TODO: Noch einbauen
        public async Task<IActionResult> RemoveFromCart(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var exemplar = await context.Exemplare.SingleOrDefaultAsync(e => e.Id == id);

            if (exemplar == null)
            {
                return NotFound();
            }

            var user = await userManager.GetUserAsync(User);
            var korb = Warenkorb.GetKorb(user, context);

            await korb.RemoveFromKorb(exemplar);

            var buch = await context.Buecher.SingleOrDefaultAsync(e => e.ISBN == exemplar.ISBN);

            toastNotification.AddToastMessage("", "Das Buch \"" + buch.Titel + "\" wurde vom Warenkorb entfernt!", ToastEnums.ToastType.Success, new ToastOption()
            {
                PositionClass = ToastPositions.TopCenter
            });

            return RedirectToAction(nameof(Index));
        }

    }
}
