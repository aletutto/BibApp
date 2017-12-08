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

namespace BibApp.Controllers
{
    public class BuecherController : Controller
    {
        private readonly BibContext context;
        private readonly UserManager<Benutzer> userManager;

        public BuecherController(BibContext context, UserManager<Benutzer> userManager)
        {
            this.context = context;
            this.userManager = userManager;
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
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Bezeichnung,Autoren,Verfügbarkeit,IstVorgemerkt,Verlag,Regal,Reihe,EntliehenVom,EntliehenBis")] Buch buch)
        {
            if (ModelState.IsValid)
            {
                context.Add(buch);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Bezeichnung,Autoren,Verfügbarkeit,IstVorgemerkt,Verlag,Regal,Reihe,EntliehenVom,EntliehenBis")] Buch buch)
        {
            if (id != buch.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
            var korb = Warenkorb.GetKorb(user, context);
            korb.AddToKorb(exemplar);

            return RedirectToAction(nameof(Index));
        }

    }
}
