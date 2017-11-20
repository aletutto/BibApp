using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BibApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Dynamic;
using System.Collections.Generic;

namespace BibApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly BibContext _context;

        public HomeController(BibContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            HomeIndexData model = new HomeIndexData();

            model.Benutzers = _context.Benutzers.ToList();
            model.Buecher = _context.Buecher.ToList();
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
