using BibApp.Models.Buch;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BibApp.Models.Warenkorb
{
    public class Warenkorb
    {
        private readonly BibContext bibContext;
        string BenutzerName { get; set; }

        public Warenkorb(BibContext bibContext)
        {
            this.bibContext = bibContext;
        }

        public static Warenkorb GetWarenkorb(Benutzer.Benutzer benutzer, BibContext bibContext)
        {
            var warenkorb = new Warenkorb(bibContext)
            {
                BenutzerName = benutzer.UserName
            };
            return warenkorb;
        }

        // Fügt ein Exemplar dem Warenkorb des eingeloggten Benutzers hinzu
        public async Task InDenWarenkorb(Buch.Exemplar exemplar)
        {
            var warenkorbExemplar = bibContext.Warenkoerbe.SingleOrDefault(
                c => c.Benutzer == BenutzerName
                && c.ISBN == exemplar.ISBN
                && c.ExemplarId == exemplar.ExemplarId);

            if (warenkorbExemplar == null)
            {
                var buch = bibContext.Buecher.SingleOrDefault(
                    c => c.ISBN == exemplar.ISBN);

                warenkorbExemplar = new Korb()
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
        public async Task EntferneVonWarenkorb(Korb korb)
        {
            var warenkorbExemplar = bibContext.Warenkoerbe.SingleOrDefault(
            c => c.Benutzer == BenutzerName
            && c.ISBN == korb.ISBN
            && c.ExemplarId == korb.ExemplarId);

            bibContext.Warenkoerbe.Remove(warenkorbExemplar);
            await bibContext.SaveChangesAsync();
        }

        // Leert den Warenkorb und entfernt somit alle Exemplar aus dem Warenkorb des eingeloggten Benutzers
        public async Task WarenkorbLeeren()
        {
            var warenkorbExemplare = bibContext.Warenkoerbe.Where(
            c => c.Benutzer == BenutzerName);

            foreach (var warenkorbExemplar in warenkorbExemplare)
            {
                bibContext.Warenkoerbe.Remove(warenkorbExemplar);
            }
            await bibContext.SaveChangesAsync();
        }

        // Sendet einen Leihauftrag an den Bibliothekar
        public async Task LeihauftragSenden()
        {
            var warenkorbExemplare = bibContext.Warenkoerbe.Where(
            c => c.Benutzer == BenutzerName);

            foreach (var warenkorbExemplar in warenkorbExemplare)
            {
                AdminKorb leihauftrag = new AdminKorb
                {
                    ISBN = warenkorbExemplar.ISBN,
                    BuchTitel = warenkorbExemplar.BuchTitel,
                    Benutzer = warenkorbExemplar.Benutzer,
                    ExemplarId = warenkorbExemplar.ExemplarId,
                    IstVerliehen = false
                };
                bibContext.AdminWarenkoerbe.Add(leihauftrag);
            }
            await bibContext.SaveChangesAsync();
            await WarenkorbLeeren();
        }
    }
}
