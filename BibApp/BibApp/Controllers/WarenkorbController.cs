using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BibApp.Models.Warenkorb;
using BibApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BibApp.Controllers
{
    public class WarenkorbController : Controller
    {
        BibContext context;
        Benutzer benutzer;
        private readonly UserManager<Benutzer> userManager;

        public WarenkorbController(
            BibContext context,
            UserManager<Benutzer> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        //public async Task<IActionResult> Index()
        //{
        //    benutzer = await userManager.GetUserAsync(User);
        //    var cart = Warenkorb.GetKorb(this.HttpContext, benutzer, context);
            
        //    return View();
        //}

        public async Task<IActionResult> Index()
        {
            return View(context.Warenkoerbe.ToList());
        }


        // GET: Warenkorb/RemoveFromKorb
        public async Task<IActionResult> RemoveFromKorb(string id)
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
            korb.RemoveFromKorb(item);

            return RedirectToAction(nameof(Index));
        }

        // GET: Warenkorb/RemoveAllFromKorb
        public async Task<IActionResult> RemoveAllFromKorb()
        {
            var user = await userManager.GetUserAsync(User);
            var korb = Warenkorb.GetKorb(user, context);
            korb.RemoveAllFromKorb();

            return RedirectToAction(nameof(Index));
        }
    }
}