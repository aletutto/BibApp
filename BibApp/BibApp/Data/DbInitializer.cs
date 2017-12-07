using BibApp.Models.Benutzer;
using BibApp.Models.Buch;
using System;
using System.Linq;

namespace BibApp.Data
{
    public static class DbInitializer
    {
        public static void Initialize(BibContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.Buecher.Any())
            {
                return;   // DB has been seeded
            }

            // TODO: User hierüber laden
            var usr = new Benutzer[]
            {

            };
            foreach (Benutzer s in usr)
            {
                context.Benutzers.Add(s);
            }
            context.SaveChanges();

            var buecher = new Buch[]
            {
                new Buch{ISBN="1234-567-8910", Titel="Deutsch 1", Autor="Max H.", Erscheinungsjahr=2010, Regal=4, Reihe=1, Verlag="Beuth Verlag", AnzahlExemplare=1},
                new Buch{ISBN="1235-567-8910", Titel="ITIL V3", Autor="Max B.", Erscheinungsjahr=2009, Regal=2, Reihe=4, Verlag="Beuth Verlag", AnzahlExemplare=2},
                new Buch{ISBN="1236-567-8910", Titel="Java für Dummies", Autor="Max D.", Erscheinungsjahr=2000, Regal=1, Reihe=3, Verlag="Kühlen Verlag", AnzahlExemplare=1},
                new Buch{ISBN="1237-567-8910", Titel="SQL Datenbanken", Autor="Max A.", Erscheinungsjahr=2015, Regal=3, Reihe=2, Verlag="CARLSEN Verlag", AnzahlExemplare=3}
            };
            foreach (Buch buch in buecher)
            {
                context.Buecher.Add(buch);

                for(int i = 1; i <= buch.AnzahlExemplare; i++)
                {
                    var exemplar = new Exemplar { ExemplarId = i, ISBN = buch.ISBN, Verfügbarkeit = true, IstVorgemerkt = false };
                    context.Exemplare.Add(exemplar);
                }
               
            }
            context.SaveChanges();
        }
    }
}