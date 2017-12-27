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

        public async Task<IActionResult> Index(string sortOrder, string sortOrder2)
        {
            HomeIndexData model = new HomeIndexData();

            // ADMIN: Abgelaufene Exemplare
            var exemplareAbgelaufen = context.Exemplare.Where(e =>
                e.EntliehenBis != null
                && (DateTime.Now.Date - e.EntliehenBis.Value).TotalDays > 0);
            ViewData["DatumSortParm"] = String.IsNullOrEmpty(sortOrder) ? "datum_desc" : "";

            switch (sortOrder)
            {
                case "datum_desc":
                    exemplareAbgelaufen = exemplareAbgelaufen.OrderByDescending(s => s.EntliehenBis);
                    break;
                default:
                    exemplareAbgelaufen = exemplareAbgelaufen.OrderBy(s => s.EntliehenBis);
                    break;
            }

            var exemplareAbgelaufenDic = new Dictionary<AdminKorb, Exemplar>();

            if (exemplareAbgelaufen != null)
            {
                foreach (var item in exemplareAbgelaufen)
                {
                    var adminKorb = context.AdminWarenkoerbe.SingleOrDefault(a => a.ISBN == item.ISBN && a.ExemplarId == item.ExemplarId);
                    exemplareAbgelaufenDic.Add(adminKorb, item);
                }
            }

            // ADMIN: Bald ablaufende Exemplare
            var exemplareLaufenBaldAb = context.Exemplare.Where(e =>
                e.EntliehenBis != null
                && (e.EntliehenBis.Value - DateTime.Now.Date).TotalDays < 7
                && (e.EntliehenBis.Value - DateTime.Now.Date).TotalDays > 0);

            ViewData["DatumSortParm2"] = String.IsNullOrEmpty(sortOrder) ? "datum2_desc" : "";

            switch (sortOrder)
            {
                case "datum2_desc":
                    exemplareLaufenBaldAb = exemplareLaufenBaldAb.OrderByDescending(s => s.EntliehenBis);
                    break;
                default:
                    exemplareLaufenBaldAb = exemplareLaufenBaldAb.OrderBy(s => s.EntliehenBis);
                    break;
            }

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

            ViewData["DatumSortParm3"] = String.IsNullOrEmpty(sortOrder) ? "datum3_desc" : "";

            Dictionary<AdminKorb, Exemplar> sorted = new Dictionary<AdminKorb, Exemplar>();

            switch (sortOrder)
            {
                case "datum3_desc":
                    foreach (var ex in exemplareEntliehenDic.OrderByDescending(s => s.Value.EntliehenBis.Value))
                    {
                        sorted.Add(ex.Key, ex.Value);
                    }
                    break;
                default:
                    foreach (var ex in exemplareEntliehenDic.OrderBy(s => s.Value.EntliehenBis.Value))
                    {
                        sorted.Add(ex.Key, ex.Value);
                    }
                    break;
            }

            // USER: Leihaufträge versendet Exemplare
            var exemplareLeihauftragVersendet = context.AdminWarenkoerbe.Where(a =>
                a.Benutzer.Equals(user.UserName)
                && a.IstVerliehen == false);

            model.ExemplareAbgelaufen = exemplareAbgelaufenDic;
            model.ExemplareLaufenBaldAb = exemplareLaufenBaldAbDic;
            model.ExemplareEntliehen = sorted;
            model.ExemplareLeihauftragVersendet = await exemplareLeihauftragVersendet.ToListAsync();
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
