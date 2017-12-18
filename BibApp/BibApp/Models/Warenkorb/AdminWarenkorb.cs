using BibApp.Models.Buch;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BibApp.Models.Warenkorb
{
    public class AdminWarenkorb
    {
        BibContext bibContext;
        string BenutzerName { get; set; }

        public AdminWarenkorb(BibContext context)
        {
            this.bibContext = context;
        }

        public static AdminWarenkorb GetKorb(Benutzer.Benutzer benutzer, BibContext bibContext)
        {
            var cart = new AdminWarenkorb(bibContext);
            cart.BenutzerName = benutzer.UserName;
            return cart;
        }

        public async Task Ausleihen(Exemplar exemplar, AdminKorb adminKorb)
        {
            exemplar.EntliehenVom = DateTime.Now;
            exemplar.EntliehenBis = DateTime.Now.AddDays(30);
            exemplar.Verfügbarkeit = false;

            adminKorb.IstVerliehen = true;

            bibContext.Exemplare.Update(exemplar);
            bibContext.AdminWarenkoerbe.Update(adminKorb)
            await bibContext.SaveChangesAsync();
        }

        public async Task Loeschen(AdminKorb adminKorb)
        {
            bibContext.AdminWarenkoerbe.Remove(adminKorb);
            await bibContext.SaveChangesAsync();
        }

        public async Task Zurueckgeben(Exemplar exemplar, AdminKorb adminKorb)
        {
            exemplar.EntliehenVom = null;
            exemplar.EntliehenBis = null;
            exemplar.Verfügbarkeit = true;

            bibContext.Exemplare.Update(exemplar);
            bibContext.AdminWarenkoerbe.Remove(adminKorb);
            await bibContext.SaveChangesAsync();
        }

        public async Task Verlaengern(Exemplar exemplar)
        {
            exemplar.EntliehenBis = exemplar.EntliehenBis.Value.AddDays(30);

            bibContext.Exemplare.Update(exemplar);
            await bibContext.SaveChangesAsync();
        }
    }
}
