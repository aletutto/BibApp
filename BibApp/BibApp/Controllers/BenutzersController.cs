using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BibApp.Models;
using System.Data.Common;
using System.Collections.Generic;

namespace BibApp.Controllers
{
    public class BenutzersController : Controller
    {
        private readonly BibContext _context;

        public BenutzersController(BibContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult BenutzerCheck(Benutzer benutzer)
        {
              
            if (IsValid(benutzer.Benutzername, benutzer.Passwort))
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Login details are wrong.");
            }
            return View("Login");
        }

        private bool IsValid(string benutzername, string password)
        {
            bool IsValid = false;
            var user = _context.Benutzers.FirstOrDefault(u => u.Benutzername == benutzername);
            if (user != null)
            {
                var pw = _context.Benutzers.FirstOrDefault(u => u.Passwort == password);
                if (pw != null)
                {
                    IsValid = true;
                }
            }
            return IsValid;
        }

        //public async Task<ActionResult> BenutzerCheck(Benutzer user)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        List<Benutzer> groups = new List<Benutzer>();
        //        var conn = _context.Database.GetDbConnection();
        //        try
        //        {
        //            await conn.OpenAsync();
        //            using (var command = conn.CreateCommand())
        //            {
        //                string query = "SELECT Benutzername, Passwort "
        //                    + "FROM Benutzers "
        //                    + "WHERE Benutzername='" + user.Benutzername 
        //                    + "' AND Passwort='" + user.Passwort + "'";
        //                command.CommandText = query;
        //                DbDataReader reader = await command.ExecuteReaderAsync();

        //                if (reader.HasRows)
        //                {
        //                    while (await reader.ReadAsync())
        //                    {
        //                        var row = new Benutzer { Benutzername = reader.GetString(0), Passwort = reader.GetString(1) };
        //                        groups.Add(row);
        //                        System.Console.WriteLine("HIER:::  "+ row.Benutzername + "  PW:   " + row.Passwort);
        //                    }
        //                } else
        //                {
        //                    System.Console.WriteLine("FEHLER FEHLERFEHLERFEHLER FEHLER FEHLER");
        //                    return View("Login");
        //                }
        //                reader.Dispose();
        //            }
        //        }
        //        finally
        //        {
        //            conn.Close();
        //        }
        //        return RedirectToAction(nameof(Index));

        //    }
        //    return View("Login");
        //}

        // GET: Benutzers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Benutzers.ToListAsync());
        }

        // GET: Benutzers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var benutzer = await _context.Benutzers
                .SingleOrDefaultAsync(m => m.Id == id);
            if (benutzer == null)
            {
                return NotFound();
            }

            return View(benutzer);
        }

        // GET: Benutzers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Benutzers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Benutzername,Passwort")] Benutzer benutzer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(benutzer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(benutzer);
        }

        // GET: Benutzers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var benutzer = await _context.Benutzers.SingleOrDefaultAsync(m => m.Id == id);
            if (benutzer == null)
            {
                return NotFound();
            }
            return View(benutzer);
        }

        // POST: Benutzers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Benutzername,Passwort")] Benutzer benutzer)
        {
            if (id != benutzer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(benutzer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BenutzerExists(benutzer.Id))
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
            return View(benutzer);
        }

        // GET: Benutzers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var benutzer = await _context.Benutzers
                .SingleOrDefaultAsync(m => m.Id == id);
            if (benutzer == null)
            {
                return NotFound();
            }

            return View(benutzer);
        }

        // POST: Benutzers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var benutzer = await _context.Benutzers.SingleOrDefaultAsync(m => m.Id == id);
            _context.Benutzers.Remove(benutzer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BenutzerExists(int id)
        {
            return _context.Benutzers.Any(e => e.Id == id);
        }
    }
}
