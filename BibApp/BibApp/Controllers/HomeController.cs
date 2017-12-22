using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BibApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Dynamic;
using System.Collections.Generic;
using System;
using BibApp.Models.Warenkorb;
using BibApp.Models.Buch;

namespace BibApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly BibContext context;

        public HomeController(BibContext context)
        {
            this.context = context;
        }

        public async Task<IActionResult> Index(string sortOrder, string searchString, string searchString2)
        {
            HomeIndexData model = new HomeIndexData();

            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["BuchSortParm"] = sortOrder == "Buch" ? "buch_desc" : "Buch";
            ViewData["ISBNSortParm"] = sortOrder == "ISBN" ? "isbn_desc" : "ISBN";
            ViewData["CurrentFilter"] = searchString;

            var books = from s in context.AdminWarenkoerbe
                        select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                books = books.Where(s =>
                s.Benutzer.Contains(searchString)
                || s.BuchTitel.Contains(searchString)
                || s.ISBN.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "buch_desc":
                    books = books.OrderByDescending(s => s.BuchTitel);
                    break;
                case "Buch":
                    books = books.OrderBy(s => s.BuchTitel);
                    break;
                case "name_desc":
                    books = books.OrderByDescending(s => s.Benutzer);
                    break;
                case "isbn_desc":
                    books = books.OrderByDescending(s => s.ISBN);
                    break;
                case "ISBN":
                    books = books.OrderBy(s => s.ISBN);
                    break;
                default:
                    books = books.OrderBy(s => s.Benutzer);
                    break;
            }

            ViewData["NameSortParm2"] = String.IsNullOrEmpty(sortOrder) ? "name_desc2" : "";
            ViewData["BuchSortParm2"] = sortOrder == "Buch2" ? "buch_desc2" : "Buch2";
            ViewData["ISBNSortParm2"] = sortOrder == "ISBN2" ? "isbn_desc2" : "ISBN2";
            ViewData["CurrentFilter2"] = searchString2;

            var books2 = from s in context.Buecher
                        select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                books2 = books2.Where(s =>
                s.Titel.Contains(searchString)
                || s.Autor.Contains(searchString)
                || s.ISBN.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "buch_desc2":
                    books2 = books2.OrderByDescending(s => s.Titel);
                    break;
                case "Buch2":
                    books2 = books2.OrderBy(s => s.Titel);
                    break;
                case "name_desc2":
                    books2 = books2.OrderByDescending(s => s.Autor);
                    break;
                case "isbn_desc2":
                    books2 = books2.OrderByDescending(s => s.ISBN);
                    break;
                case "ISBN2":
                    books2 = books2.OrderBy(s => s.ISBN);
                    break;
                default:
                    books2 = books2.OrderBy(s => s.Autor);
                    break;
            }

            var exemplareFrist = context.Exemplare.Where(e =>
            e.EntliehenBis != null &&
            (DateTime.Now.Date - e.EntliehenBis.Value).TotalDays > 0);

            var dic = new Dictionary<AdminKorb, Exemplar>();

            if (exemplareFrist != null)
            {
                foreach (var item in exemplareFrist)
                {
                    var adminKorb = context.AdminWarenkoerbe.SingleOrDefault(a => a.ISBN == item.ISBN && a.ExemplarId == item.ExemplarId );
                    dic.Add(adminKorb, item);

                    Debug.WriteLine("HIER: " + (DateTime.Now.Date - item.EntliehenBis.Value).TotalDays);
                }
            }

            model.Dictionary = dic;
            model.Buecher = await books2.AsNoTracking().ToListAsync();
            model.AdminKoerbe = await books.AsNoTracking().ToListAsync();
            return View(model);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }     

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
