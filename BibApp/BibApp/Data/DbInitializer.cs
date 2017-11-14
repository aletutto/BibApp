using BibApp.Models;
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

            var usr = new Benutzer[]
            {
            new Benutzer{Benutzername="admin", Passwort="admin"},
            new Benutzer{Benutzername="peter", Passwort="peter"}
            };
            foreach (Benutzer s in usr)
            {
                context.Benutzers.Add(s);
            }
            context.SaveChanges();

            var buch = new Buch[]
            {
            new Buch{Bezeichnung="Deutsch 1", Autoren="Max H.", Regal=4, Reihe=1, Verfügbarkeit=true, Verlag="German Verlag"},
            new Buch{Bezeichnung="Deutsch 2", Autoren="Max B.", Regal=2, Reihe=4, Verfügbarkeit=false, Verlag="Germanee Verlag"},
            new Buch{Bezeichnung="Deutsch 3", Autoren="Max D.", Regal=1, Reihe=3, Verfügbarkeit=true, Verlag="dutch Verlag"},
            new Buch{Bezeichnung="Deutsch 4", Autoren="Max A.", Regal=3, Reihe=2, Verfügbarkeit=true, Verlag="english Verlag"},
            };
            foreach (Buch c in buch)
            {
                context.Buecher.Add(c);
            }
            context.SaveChanges();

           
        }
    }
}