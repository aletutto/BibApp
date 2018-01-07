using BibApp.Models.Buch;
using System;
using System.Threading.Tasks;

namespace BibApp.Models.Warenkorb
{
    public class LeihauftragManager
    {
        private readonly BibContext bibContext;

        public LeihauftragManager(BibContext bibContext)
        {
            this.bibContext = bibContext;
        }

        public static LeihauftragManager GetLeihauftragManager(BibContext bibContext)
        {
            var cart = new LeihauftragManager(bibContext);
            return cart;
        }

        // Leiht einem Benutzer ein Exemplar aus
        public async Task Ausleihen(Exemplar exemplar, Leihauftrag leihauftrag)
        {
            exemplar.EntliehenBis = DateTime.Now.AddDays(30);
            exemplar.Verfügbarkeit = false;

            leihauftrag.IstVerliehen = true;

            bibContext.Exemplar.Update(exemplar);
            bibContext.Leihauftrag.Update(leihauftrag);
            await bibContext.SaveChangesAsync();
        }

        // Löscht einen Leihauftrag aus der Liste "Ausleihen"
        public async Task Loeschen(Leihauftrag leihauftrag)
        {
            bibContext.Leihauftrag.Remove(leihauftrag);
            await bibContext.SaveChangesAsync();
        }

        // Gibt ein Exemplar zurück an die Bibliothek
        public async Task Zurueckgeben(Exemplar exemplar, Leihauftrag leihauftrag)
        {
            exemplar.EntliehenBis = null;
            exemplar.Verfügbarkeit = true;

            bibContext.Exemplar.Update(exemplar);
            bibContext.Leihauftrag.Remove(leihauftrag);
            await bibContext.SaveChangesAsync();
        }

        // Verlängert die Leihfrist um 30 Tage
        public async Task Verlaengern(Exemplar exemplar)
        {
            exemplar.EntliehenBis = exemplar.EntliehenBis.Value.AddDays(30);

            bibContext.Exemplar.Update(exemplar);
            await bibContext.SaveChangesAsync();
        }
    }
}
