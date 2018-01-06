using System.Linq;
using System.Threading.Tasks;

namespace BibApp.Models.Warenkorb
{
    public class WarenkorbManager
    {
        private readonly BibContext bibContext;
        string BenutzerName { get; set; }

        public WarenkorbManager(BibContext bibContext)
        {
            this.bibContext = bibContext;
        }

        public static WarenkorbManager GetWarenkorb(Benutzer.Benutzer benutzer, BibContext bibContext)
        {
            var warenkorb = new WarenkorbManager(bibContext)
            {
                BenutzerName = benutzer.UserName
            };
            return warenkorb;
        }

        // Fügt ein Exemplar dem Warenkorb des eingeloggten Benutzers hinzu
        public async Task InDenWarenkorb(Buch.Exemplar exemplar)
        {
            var warenkorbExemplar = bibContext.Warenkorb.SingleOrDefault(
                c => c.Benutzer == BenutzerName
                && c.ISBN == exemplar.ISBN
                && c.ExemplarId == exemplar.ExemplarId);

            if (warenkorbExemplar == null)
            {
                var buch = bibContext.Buch.SingleOrDefault(
                    c => c.ISBN == exemplar.ISBN);

                warenkorbExemplar = new Warenkorb()
                {
                    Benutzer = BenutzerName,
                    ISBN = exemplar.ISBN,
                    ExemplarId = exemplar.ExemplarId,
                    BuchTitel = buch.Titel
                };
                bibContext.Add(warenkorbExemplar);
                await bibContext.SaveChangesAsync();
            }
        }

        // Entfernt ein Exemplar aus dem Warenkorb des eingeloggten Benutzers
        public async Task EntferneVonWarenkorb(Warenkorb korb)
        {
            var warenkorbExemplar = bibContext.Warenkorb.SingleOrDefault(
            c => c.Benutzer == BenutzerName
            && c.ISBN == korb.ISBN
            && c.ExemplarId == korb.ExemplarId);

            bibContext.Warenkorb.Remove(warenkorbExemplar);
            await bibContext.SaveChangesAsync();
        }

        // Leert den Warenkorb und entfernt somit alle Exemplar aus dem Warenkorb des eingeloggten Benutzers
        public async Task WarenkorbLeeren()
        {
            var warenkorbExemplare = bibContext.Warenkorb.Where(
            c => c.Benutzer == BenutzerName);

            foreach (var warenkorbExemplar in warenkorbExemplare)
            {
                bibContext.Warenkorb.Remove(warenkorbExemplar);
            }
            await bibContext.SaveChangesAsync();
        }

        // Sendet einen Leihauftrag an den Bibliothekar
        public async Task LeihauftragSenden()
        {
            var warenkorbExemplare = bibContext.Warenkorb.Where(
            c => c.Benutzer == BenutzerName);

            foreach (var warenkorbExemplar in warenkorbExemplare)
            {
                Leihauftrag leihauftrag = new Leihauftrag
                {
                    ISBN = warenkorbExemplar.ISBN,
                    BuchTitel = warenkorbExemplar.BuchTitel,
                    Benutzer = warenkorbExemplar.Benutzer,
                    ExemplarId = warenkorbExemplar.ExemplarId,
                    IstVerliehen = false
                };
                bibContext.Leihauftrag.Add(leihauftrag);
            }
            await bibContext.SaveChangesAsync();
            await WarenkorbLeeren();
        }
    }
}
