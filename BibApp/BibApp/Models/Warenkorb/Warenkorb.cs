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
        BibContext bibContext;
        string BenutzerName { get; set; }

        public Warenkorb() { }

        public Warenkorb(BibContext bibContext)
        {
            this.bibContext = bibContext;
        }
        public static Warenkorb GetKorb(Benutzer.Benutzer benutzer, BibContext bibContext)
        {
            var cart = new Warenkorb(bibContext);
            cart.BenutzerName = benutzer.UserName;
            return cart;
        }

        public async Task AddToKorb(Buch.Exemplar exemplar)
        {
            var cartItem = bibContext.Warenkoerbe.SingleOrDefault(
                c => c.Benutzer == BenutzerName
                && c.ISBN == exemplar.ISBN
                && c.ExemplarId == exemplar.ExemplarId);

            if (cartItem == null)
            {
                var buch = bibContext.Buecher.SingleOrDefault(
                    c => c.ISBN == exemplar.ISBN);

                cartItem = new Korb()
                {
                    Benutzer = BenutzerName,
                    ISBN = exemplar.ISBN,
                    ExemplarId = exemplar.ExemplarId,
                    BuchTitel = buch.Titel
                };
                bibContext.Add(cartItem);
                await bibContext.SaveChangesAsync();
            }
        }

        public async Task RemoveFromKorb(Korb korb)
        {
            var cartItem = bibContext.Warenkoerbe.SingleOrDefault(
            c => c.Benutzer == BenutzerName
            && c.ISBN == korb.ISBN
            && c.ExemplarId == korb.ExemplarId);

            bibContext.Warenkoerbe.Remove(cartItem);
            await bibContext.SaveChangesAsync();
        }

        public async Task RemoveAllFromKorb()
        {
            var cartItems = bibContext.Warenkoerbe.Where(
            c => c.Benutzer == BenutzerName);

            foreach (var cartItem in cartItems)
            {
                bibContext.Warenkoerbe.Remove(cartItem);
            }
            await bibContext.SaveChangesAsync();
        }

        public async Task LeihauftragSenden()
        {
            var cartItems = bibContext.Warenkoerbe.Where(
            c => c.Benutzer == BenutzerName);

            foreach (var cartItem in cartItems)
            {
                AdminKorb cart = new AdminKorb
                {
                    ISBN = cartItem.ISBN,
                    BuchTitel = cartItem.BuchTitel,
                    Benutzer = cartItem.Benutzer,
                    ExemplarId = cartItem.ExemplarId,
                    IstVerliehen = false
                };
                bibContext.AdminWarenkoerbe.Add(cart);
            }
            await bibContext.SaveChangesAsync();
            await RemoveAllFromKorb();
        }
    }
}
