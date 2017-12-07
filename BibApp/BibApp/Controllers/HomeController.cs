using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BibApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Dynamic;
using System.Collections.Generic;
using System;

namespace BibApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly BibContext _context;

        public HomeController(BibContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string sortOrder, string searchString, string searchString2)
        {
            HomeIndexData model = new HomeIndexData();

            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["IdSortParm"] = sortOrder == "Id" ? "id_desc" : "Id";
            ViewData["AutSortParm"] = sortOrder == "Autor" ? "aut_desc" : "Autor";
            ViewData["entlBisSortParm"] = sortOrder == "EntliehenBis" ? "entlBis_desc" : "EntliehenBis";
            ViewData["entlVomSortParm"] = sortOrder == "EntliehenVom" ? "entlVom_desc" : "EntliehenVom";
            ViewData["VorgSortParm"] = sortOrder == "Vorgemerkt" ? "vorg_desc" : "Vorgemerkt";
            ViewData["VerfSortParm"] = sortOrder == "Verfügbarkeit" ? "verfüg_desc" : "Verfügbarkeit";
            ViewData["VerlagSortParm"] = sortOrder == "Verlag" ? "verlag_desc" : "Verlag";
            ViewData["CurrentFilter"] = searchString;

            var books = from s in _context.Buecher
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
                case "id_desc":
                    books = books.OrderByDescending(s => s.Id);
                    break;
                case "Id":
                    books = books.OrderBy(s => s.Id);
                    break;
                case "aut_desc":
                    books = books.OrderByDescending(s => s.Autor);
                    break;
                case "Autor":
                    books = books.OrderBy(s => s.Autor);
                    break;
                case "name_desc":
                    books = books.OrderByDescending(s => s.Titel);
                    break;
                //case "entlBis_desc":
                //    books = books.OrderByDescending(s => s.EntliehenBis);
                //    break;
                //case "EntliehenBis":
                //    books = books.OrderBy(s => s.EntliehenBis);
                //    break;
                //case "entlVom_desc":
                //    books = books.OrderByDescending(s => s.EntliehenVom);
                //    break;
                //case "EntliehenVom":
                //    books = books.OrderBy(s => s.EntliehenVom);
                //    break;
                //case "vorg_desc":
                //    books = books.OrderByDescending(s => s.IstVorgemerkt);
                //    break;
                //case "Vorgemerkt":
                //    books = books.OrderBy(s => s.IstVorgemerkt);
                //    break;
                //case "verfüg_desc":
                //    books = books.OrderByDescending(s => s.Verfügbarkeit);
                //    break;
                //case "Verfügbarkeit":
                //    books = books.OrderBy(s => s.Verfügbarkeit);
                //    break;
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

            ViewData["NameSortParm2"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["RoleSortParm2"] = sortOrder == "Role" ? "role_desc" : "Role";
            ViewData["CurrentFilter2"] = searchString2;

            var benutzer = from s in _context.Benutzers
                           select s;

            if (!String.IsNullOrEmpty(searchString2))
            {
                benutzer = benutzer.Where(s => s.UserName.Contains(searchString2));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    benutzer = benutzer.OrderByDescending(s => s.UserName);
                    break;
                case "role_desc":
                    benutzer = benutzer.OrderByDescending(s => s.Role);
                    break;
                case "Role":
                    benutzer = benutzer.OrderBy(s => s.Role);
                    break;
                default:
                    benutzer = benutzer.OrderBy(s => s.UserName);
                    break;
            }

            model.Benutzers = await benutzer.AsNoTracking().ToListAsync();
            model.Buecher = await books.AsNoTracking().ToListAsync();
            return View(model);
        }

        public IActionResult UserIndex()
        {
            HomeIndexData model = new HomeIndexData();

            model.Benutzers = _context.Benutzers.ToList();
            model.Buecher = _context.Buecher.ToList();
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
