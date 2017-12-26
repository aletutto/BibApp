using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BibApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using System;
using BibApp.Models.Warenkorb;
using BibApp.Models.Buch;
using Microsoft.AspNetCore.Identity;
using BibApp.Models.Benutzer;

namespace BibApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly BibContext context;
        private readonly UserManager<Benutzer> userManager;

        public HomeController(BibContext context, UserManager<Benutzer> userManager)
        {
            this.context = context;
            this.userManager = userManager;
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

            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["BuchSortParm"] = sortOrder == "Buch" ? "buch_desc" : "Buch";
            ViewData["ISBNSortParm"] = sortOrder == "ISBN" ? "isbn_desc" : "ISBN";
            ViewData["CurrentFilter"] = searchString;


            // NEU
            var exemplareAbgelaufen = context.Exemplare.Where(e =>
                e.EntliehenBis != null
                && (DateTime.Now.Date - e.EntliehenBis.Value).TotalDays > 0);

            if (!String.IsNullOrEmpty(searchString))
            {
                exemplareAbgelaufen = context.Exemplare.Where(e =>
                    e.EntliehenBis != null &&
                    (DateTime.Now.Date - e.EntliehenBis.Value).TotalDays > 0
                    && e.EntliehenBis.ToString().Contains(searchString));

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



            // ADMIN: Abgelaufene Exemplare

            var exemplareAbgelaufenDic = new Dictionary<AdminKorb, Exemplar>();

            if (exemplareAbgelaufen != null)
            {
                foreach (var item in exemplareAbgelaufen)
                {
                    var adminKorb = context.AdminWarenkoerbe.SingleOrDefault(a => a.ISBN == item.ISBN && a.ExemplarId == item.ExemplarId );
                    exemplareAbgelaufenDic.Add(adminKorb, item);
                }
            }

            // ADMIN: Bald ablaufende Exemplare
            var exemplareLaufenBaldAb = context.Exemplare.Where(e =>
                e.EntliehenBis != null
                && (e.EntliehenBis.Value - DateTime.Now.Date).TotalDays < 7
                && (e.EntliehenBis.Value - DateTime.Now.Date).TotalDays > 0 );

            var exemplareLaufenBaldAbDic = new Dictionary<AdminKorb, Exemplar>();

            if (exemplareLaufenBaldAb != null)
            {
                foreach (var item in exemplareLaufenBaldAb)
                {
                    var adminKorb = context.AdminWarenkoerbe.SingleOrDefault(a => a.ISBN == item.ISBN && a.ExemplarId == item.ExemplarId);
                    exemplareLaufenBaldAbDic.Add(adminKorb, item);
                }
            }

            // USER: Entliehende Exemplare
            var user = await userManager.GetUserAsync(User);
            var exemplareEntliehen = context.AdminWarenkoerbe.Where(a =>
                a.Benutzer.Equals(user.UserName)
                && a.IstVerliehen == true);
            var exemplareEntliehenDic = new Dictionary<AdminKorb, Exemplar>();

            if (exemplareEntliehen != null)
            {
                foreach (var item in exemplareEntliehen)
                {
                    var exemplar = context.Exemplare.SingleOrDefault(a => a.ISBN == item.ISBN && a.ExemplarId == item.ExemplarId);
                    exemplareEntliehenDic.Add(item, exemplar);
                }
            }

            // USER: Leihaufträge versendet Exemplare
            var exemplareLeihauftragVersendet = context.AdminWarenkoerbe.Where(a =>
                a.Benutzer.Equals(user.UserName)
                && a.IstVerliehen == false);
            var exemplareLeihauftragVersendetDic = new Dictionary<AdminKorb, Exemplar>();

            if (exemplareLeihauftragVersendet != null)
            {
                foreach (var item in exemplareLeihauftragVersendet)
                {
                    var exemplar = context.Exemplare.SingleOrDefault(a => a.ISBN == item.ISBN && a.ExemplarId == item.ExemplarId);
                    exemplareLeihauftragVersendetDic.Add(item, exemplar);
                }
            }

            model.ExemplareAbgelaufen = exemplareAbgelaufenDic;
            model.ExemplareLaufenBaldAb = exemplareLaufenBaldAbDic;
            model.ExemplareEntliehen = exemplareEntliehenDic;
            model.ExemplareLeihauftragVersendet = exemplareLeihauftragVersendetDic;
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
