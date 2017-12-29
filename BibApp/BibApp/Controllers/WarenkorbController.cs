using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BibApp.Models.Warenkorb;
using BibApp.Models.Benutzer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace BibApp.Controllers
{
    public class WarenkorbController : Controller
    {
        private readonly BibContext context;
        private readonly UserManager<Benutzer> userManager;
        private readonly IToastNotification toastNotification;

        public WarenkorbController(
            BibContext context,
            UserManager<Benutzer> userManager,
            IToastNotification toastNotification)
        {
            this.context = context;
            this.userManager = userManager;
            this.toastNotification = toastNotification;
        }

        public async Task<IActionResult> Index()
        {
            var warenkorb = context.Warenkoerbe.Where(w => w.Benutzer.Equals(userManager.GetUserAsync(User).Result.UserName));
            return View(await warenkorb.ToListAsync());
        }

        // GET: Warenkorb/RemoveFromKorb
        public async Task<IActionResult> RemoveFromKorb(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await context.Warenkoerbe.SingleOrDefaultAsync(i => i.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            var user = await userManager.GetUserAsync(User);
            var korb = Warenkorb.GetKorb(user, context);
            await korb.RemoveFromKorb(item);

            toastNotification.AddToastMessage("Entfernen", "Das Buch\"" + item.BuchTitel + "\" wurde aus dem Warenkorb entfernt!", ToastEnums.ToastType.Success, new ToastOption()
            {
                PositionClass = ToastPositions.TopCenter
            });

            return RedirectToAction(nameof(Index));
        }

        // GET: Warenkorb/RemoveAllFromKorb
        public async Task<IActionResult> RemoveAllFromKorb()
        {
            var user = await userManager.GetUserAsync(User);
            var korb = Warenkorb.GetKorb(user, context);
            await korb.RemoveAllFromKorb();

            toastNotification.AddToastMessage("Entfernen", "Es wurden alle Bücher aus dem Warenkorb entfernt!", ToastEnums.ToastType.Success, new ToastOption()
            {
                PositionClass = ToastPositions.TopCenter
            });

            return RedirectToAction(nameof(Index));
        }

        // GET: Warenkorb/Abschicken
        /* TODO: WICHTIG: Nur Bücher in den Warenkorb einfügen können, welche verfügbar, also noch nicht ausgeliehen worden sind.
         * Nur Bücher verschicken können, welche noch nicht im AdminWarenkorb vorhanden sind.
         * 
        */
        public async Task<IActionResult> LeihauftragSenden()
        {
            var user = await userManager.GetUserAsync(User);
            var korb = Warenkorb.GetKorb(user, context);
            await korb.LeihauftragSenden();

            toastNotification.AddToastMessage("Leihauftrag senden", "Der Leihauftrag wurde an den Bibliothekar gesendet!", ToastEnums.ToastType.Success, new ToastOption()
            {
                PositionClass = ToastPositions.TopCenter
            });

            return RedirectToAction(nameof(Index));
        }
    }
}