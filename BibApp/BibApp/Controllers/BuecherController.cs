using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BibApp.Models;
using BibApp.Models.Warenkorb;
using Microsoft.AspNetCore.Identity;

namespace BibApp.Controllers
{
    public class BuecherController : Controller
    {
        private readonly BibContext _context;
        private readonly UserManager<Benutzer> _userManager;

        public BuecherController(BibContext context, UserManager<Benutzer> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Buecher
        public async Task<IActionResult> Index()
        {
            return View(await _context.Buecher.ToListAsync());
        }

        // GET: Buecher/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var buch = await _context.Buecher
                .SingleOrDefaultAsync(m => m.Id == id);
            if (buch == null)
            {
                return NotFound();
            }

            return View(buch);
        }

        // GET: Buecher/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Buecher/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Bezeichnung,Autoren,Verfügbarkeit,IstVorgemerkt,Verlag,Regal,Reihe,EntliehenVom,EntliehenBis")] Buch buch)
        {
            if (ModelState.IsValid)
            {
                _context.Add(buch);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(buch);
        }

        // GET: Buecher/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var buch = await _context.Buecher.SingleOrDefaultAsync(m => m.Id == id);
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
                    _context.Update(buch);
                    await _context.SaveChangesAsync();
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
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var buch = await _context.Buecher
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var buch = await _context.Buecher.SingleOrDefaultAsync(m => m.Id == id);
            _context.Buecher.Remove(buch);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BuchExists(int id)
        {
            return _context.Buecher.Any(e => e.Id == id);
        }

        // GET: Buecher/AddToCart
        public async Task<IActionResult> AddToCart(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var buch = await _context.Buecher.SingleOrDefaultAsync(b => b.Id == id);

            if (buch == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var korb = Warenkorb.GetKorb(user, _context);
            korb.AddToKorb(buch);

            return RedirectToAction(nameof(Index));
        }

    }
}
