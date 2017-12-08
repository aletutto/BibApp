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
            ViewData["AutSortParm"] = sortOrder == "Autor" ? "aut_desc" : "Autor";
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
                case "aut_desc":
                    books = books.OrderByDescending(s => s.Autor);
                    break;
                case "Autor":
                    books = books.OrderBy(s => s.Autor);
                    break;
                case "name_desc":
                    books = books.OrderByDescending(s => s.Titel);
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
