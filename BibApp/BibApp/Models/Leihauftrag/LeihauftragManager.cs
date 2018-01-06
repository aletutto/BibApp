using BibApp.Models.Buch;
using System;
using System.Threading.Tasks;

namespace BibApp.Models.Warenkorb
{
    public class LeihauftragManager
    {
        BibContext bibContext;

        public LeihauftragManager(BibContext bibContext)
        {
            this.bibContext = bibContext;
        }

        public static LeihauftragManager GetKorb(BibContext bibContext)
        {
            var cart = new LeihauftragManager(bibContext);
            return cart;
        }

        public async Task Ausleihen(Exemplar exemplar, Leihauftrag adminKorb)
        {
            exemplar.EntliehenBis = DateTime.Now.AddDays(30);
            exemplar.Verfügbarkeit = false;

            adminKorb.IstVerliehen = true;

            bibContext.Exemplar.Update(exemplar);
            bibContext.Leihauftrag.Update(adminKorb);
            await bibContext.SaveChangesAsync();
        }

        public async Task Loeschen(Leihauftrag adminKorb)
        {
            bibContext.Leihauftrag.Remove(adminKorb);
            await bibContext.SaveChangesAsync();
        }

        public async Task Zurueckgeben(Exemplar exemplar, Leihauftrag adminKorb)
        {
            exemplar.EntliehenBis = null;
            exemplar.Verfügbarkeit = true;

            bibContext.Exemplar.Update(exemplar);
            bibContext.Leihauftrag.Remove(adminKorb);
            await bibContext.SaveChangesAsync();
        }

        public async Task Verlaengern(Exemplar exemplar)
        {
            exemplar.EntliehenBis = exemplar.EntliehenBis.Value.AddDays(30);

            bibContext.Exemplar.Update(exemplar);
            await bibContext.SaveChangesAsync();
        }
    }
}
