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
using Microsoft.AspNetCore.Authorization;

namespace BibApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly BibContext bibContext;
        private readonly UserManager<Benutzer> userManager;

        public HomeController(
            BibContext bibContext,
            UserManager<Benutzer> userManager)
        {
            this.bibContext = bibContext;
            this.userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index(string sortOrder, string sortOrder2)
        {
            HomeIndexData model = new HomeIndexData();

            if (User.IsInRole("Admin")) // Fall: Admin ist eingeloggt
            {
                // ADMIN: Abgelaufenes Leihfristende
                var exemplareAbgelaufen = bibContext.Exemplar.Where(e =>
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

                // Ordne den Leihaufträgen die entsprechenden Exemplare zu
                var exemplareAbgelaufenDic = new Dictionary<Leihauftrag, Exemplar>();

                if (exemplareAbgelaufen != null)
                {
                    foreach (var exemplar in exemplareAbgelaufen)
                    {
                        var leihauftrag = bibContext.Leihauftrag.SingleOrDefault(a => a.ISBN == exemplar.ISBN && a.ExemplarId == exemplar.ExemplarId);
                        exemplareAbgelaufenDic.Add(leihauftrag, exemplar);
                    }
                }

                // ADMIN: In Kürze ablaufendes Leihfristende
                var exemplareLaufenBaldAb = bibContext.Exemplar.Where(e =>
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

                var exemplareLaufenBaldAbDic = new Dictionary<Leihauftrag, Exemplar>();

                if (exemplareLaufenBaldAb != null)
                {
                    foreach (var exemplar in exemplareLaufenBaldAb)
                    {
                        var leihauftrag = bibContext.Leihauftrag.SingleOrDefault(a => a.ISBN == exemplar.ISBN && a.ExemplarId == exemplar.ExemplarId);
                        exemplareLaufenBaldAbDic.Add(leihauftrag, exemplar);
                    }
                }

                model.ExemplareAbgelaufen = exemplareAbgelaufenDic;
                model.ExemplareLaufenBaldAb = exemplareLaufenBaldAbDic;

            }
            else // Fall: Member ist eingeloggt
            {
                // MEMBER: Ausgeliehene Bücher
                var user = await userManager.GetUserAsync(User);
                var exemplareEntliehen = bibContext.Leihauftrag.Where(a =>
                    a.Benutzer.Equals(user.UserName)
                    && a.IstVerliehen == true);

                var exemplareEntliehenDic = new Dictionary<Leihauftrag, Exemplar>();

                if (exemplareEntliehen != null)
                {
                    foreach (var leihauftrag in exemplareEntliehen)
                    {
                        var exemplar = bibContext.Exemplar.SingleOrDefault(a => a.ISBN == leihauftrag.ISBN && a.ExemplarId == leihauftrag.ExemplarId);
                        exemplareEntliehenDic.Add(leihauftrag, exemplar);
                    }
                }

                ViewData["DatumSortParm3"] = String.IsNullOrEmpty(sortOrder) ? "datum3_desc" : "";

                Dictionary<Leihauftrag, Exemplar> sortedExemplareEntliehenDic = new Dictionary<Leihauftrag, Exemplar>();

                switch (sortOrder)
                {
                    case "datum3_desc":
                        foreach (var exemplar in exemplareEntliehenDic.OrderByDescending(s => s.Value.EntliehenBis.Value))
                        {
                            sortedExemplareEntliehenDic.Add(exemplar.Key, exemplar.Value);
                        }
                        break;
                    default:
                        foreach (var exemplar in exemplareEntliehenDic.OrderBy(s => s.Value.EntliehenBis.Value))
                        {
                            sortedExemplareEntliehenDic.Add(exemplar.Key, exemplar.Value);
                        }
                        break;
                }

                // MEMBER: Versendete Leihaufträge
                var exemplareLeihauftragVersendet = bibContext.Leihauftrag.Where(a =>
                    a.Benutzer.Equals(user.UserName)
                    && a.IstVerliehen == false);

                model.ExemplareEntliehen = sortedExemplareEntliehenDic;
                model.ExemplareLeihauftragVersendet = await exemplareLeihauftragVersendet.ToListAsync();
            }

            return View(model);
        }
    }
}
