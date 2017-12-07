using BibApp.Models.Buch;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task Ausleihen(Exemplar exemplar)
        {
            exemplar.EntliehenVom = DateTime.Now;
            exemplar.EntliehenBis = DateTime.Now.AddDays(30);
            exemplar.Verfügbarkeit = false;

            bibContext.Exemplare.Update(exemplar);
            await bibContext.SaveChangesAsync();
        }
    }
}
