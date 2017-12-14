﻿using BibApp.Models.Buch;
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

        // TODO: Wenn das Buch schon dem Warenkorb hinzugefügt wurde, dann sollte man einen Hinweis erhalten das das Buch bereits im Warenkorb ist wenn man nochmal versucht es hinzuzufügen
        public bool CheckIfAlreadyAdded(Buch.Exemplar exemplar)
        {
            var cartItem = bibContext.Warenkoerbe.SingleOrDefault(
            c => c.Benutzer == BenutzerName
            && c.ISBN == exemplar.ISBN
            && c.ExemplarId == exemplar.ExemplarId);

            if (cartItem == null)
            {
                return false;
            }

            return true;
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

        public async Task RemoveFromKorb(Exemplar exemplar)
        {
            var cartItem = bibContext.Warenkoerbe.SingleOrDefault(
            c => c.Benutzer == BenutzerName
            && c.ISBN == exemplar.ISBN
            && c.ExemplarId == exemplar.ExemplarId);

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

        // TODO: Alle Bücher im Warenkorb zählen und im Layout anzeigen z.B. -> Warenkorb (2)
        public int CountAllItems(Benutzer.Benutzer benutzer)
        {
            return bibContext.Warenkoerbe.Count(
            c => c.Benutzer == BenutzerName);
        }

        public async Task LeihauftragSenden()
        {
            var cartItems = bibContext.Warenkoerbe.Where(
            c => c.Benutzer == BenutzerName);

            foreach (var cartItem in cartItems)
            {
                AdminKorb cart = new AdminKorb();
                cart.ISBN = cartItem.ISBN;
                cart.BuchTitel = cartItem.BuchTitel;
                cart.Benutzer = cartItem.Benutzer;
                cart.ExemplarId = cartItem.ExemplarId;
                cart.IstVerliehen = false;
                bibContext.AdminWarenkoerbe.Add(cart);
            }
            await bibContext.SaveChangesAsync();
            await RemoveAllFromKorb();
        }
    }
}
