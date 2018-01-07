using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BibApp.Models.Warenkorb;
using BibApp.Models.Benutzer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using Microsoft.AspNetCore.Authorization;

namespace BibApp.Controllers
{
    public class WarenkorbController : Controller
    {
        private readonly BibContext bibContext;
        private readonly UserManager<Benutzer> userManager;
        private readonly IToastNotification toastNotification;

        public WarenkorbController(
            BibContext bibContext,
            UserManager<Benutzer> userManager,
            IToastNotification toastNotification)
            {
                this.bibContext = bibContext;
                this.userManager = userManager;
                this.toastNotification = toastNotification;
            }

        // GET: Warenkorb/Index
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            // Gibt alle Bücher im Warenkorb des eingeloggten Benutzer an den Index weiter
            var warenkorb = bibContext.Warenkorb.Where(w => w.Benutzer.Equals(userManager.GetUserAsync(User).Result.UserName));
            return View(await warenkorb.ToListAsync());
        }

        // Entfernt ein einzelnes Buch aus dem Warenkorb
        public async Task<IActionResult> EntferneVonWarenkorb(int? id)
        {
            var warenkorbExemplar = await bibContext.Warenkorb.SingleOrDefaultAsync(i => i.Id == id);
            var user = await userManager.GetUserAsync(User);
            var warenkorbManager = WarenkorbManager.GetWarenkorbManager(user, bibContext);
            await warenkorbManager.EntferneVonWarenkorb(warenkorbExemplar);

            toastNotification.AddToastMessage("Buch entfernt", "Das Buch \"" + warenkorbExemplar.BuchTitel + "\" wurde aus dem Warenkorb entfernt.", ToastEnums.ToastType.Success, new ToastOption()
            {
                PositionClass = ToastPositions.TopCenter
            });

            return RedirectToAction(nameof(Index));
        }

        // Entfernt alle Bücher aus dem Warenkorb
        public async Task<IActionResult> WarenkorbLeeren()
        {
            var user = await userManager.GetUserAsync(User);
            var warenkorbManager = WarenkorbManager.GetWarenkorbManager(user, bibContext);
            await warenkorbManager.WarenkorbLeeren();

            toastNotification.AddToastMessage("Warenkorb geleert", "Es wurden alle Bücher aus dem Warenkorb entfernt.", ToastEnums.ToastType.Success, new ToastOption()
            {
                PositionClass = ToastPositions.TopCenter
            });

            return RedirectToAction(nameof(Index));
        }

        // Sendet einen Leihauftrag an den Bibliothekar
        public async Task<IActionResult> LeihauftragSenden()
        {
            var user = await userManager.GetUserAsync(User);
            var warenkorbManager = WarenkorbManager.GetWarenkorbManager(user, bibContext);
            await warenkorbManager.LeihauftragSenden();

            toastNotification.AddToastMessage("Leihauftrag gesendet", "Der Leihauftrag wurde an den Bibliothekar gesendet.", ToastEnums.ToastType.Success, new ToastOption()
            {
                PositionClass = ToastPositions.TopCenter
            });

            return RedirectToAction(nameof(Index));
        }
    }
}