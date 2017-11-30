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

        public Warenkorb(BibContext context)
        {
            this.bibContext = context;
        }
        public static Warenkorb GetKorb(Benutzer benutzer, BibContext bibContext)
        {
            var cart = new Warenkorb(bibContext);
            cart.BenutzerName = benutzer.UserName;
            return cart;
        }

        public void AddToKorb(Buch buch)
        {
            var cartItem = bibContext.Warenkoerbe.SingleOrDefault(
            c => c.KorbId == BenutzerName
            && c.BuchId == buch.Id);

            if (cartItem == null)
            {
                cartItem = new Korb()
                {
                    KorbId = BenutzerName,
                    BuchId = buch.Id,
                    BuchTitel = buch.Bezeichnung
                };
                bibContext.Add(cartItem);
                bibContext.SaveChangesAsync();
            }
        }

        // TODO: Wenn das Buch schon dem Warenkorb hinzugefügt wurde, dann sollte man einen Hinweis erhalten das das Buch bereits im Warenkorb ist wenn man nochmal versucht es hinzuzufügen
        public bool checkIfAlreadyAdded(Buch buch)
        {
            var cartItem = bibContext.Warenkoerbe.SingleOrDefault(
            c => c.KorbId == BenutzerName
            && c.BuchId == buch.Id);

            if (cartItem == null)
            {
                return false;
            }

            return true;
        }

        public void RemoveFromKorb(Korb korb)
        {
            var cartItem = bibContext.Warenkoerbe.SingleOrDefault(
            c => c.KorbId == BenutzerName
            && c.BuchId == korb.BuchId);

            bibContext.Warenkoerbe.Remove(cartItem);
            bibContext.SaveChangesAsync();
        }

        public async Task RemoveAllFromKorb()
        {
            var cartItems = bibContext.Warenkoerbe.Where(
            c => c.KorbId == BenutzerName);

            foreach (var cartItem in cartItems)
            {
                bibContext.Warenkoerbe.Remove(cartItem);
            }
            await bibContext.SaveChangesAsync();
        }

        // TODO: Alle Bücher im Warenkorb zählen und im Layout anzeigen z.B. -> Warenkorb (2)
        public int CountAllItems(Benutzer benutzer)
        {
            return bibContext.Warenkoerbe.Count(
            c => c.KorbId == BenutzerName);
        }

        public async Task LeihauftragSenden()
        {
            var cartItems = bibContext.Warenkoerbe.Where(
            c => c.KorbId == BenutzerName);

            foreach (var cartItem in cartItems)
            {
                AdminKorb cart = new AdminKorb();
                cart.BuchId = cartItem.BuchId;
                cart.BuchTitel = cartItem.BuchTitel;
                cart.KorbId = cartItem.KorbId;
                bibContext.AdminWarenkoerbe.Add(cart);
            }
            await bibContext.SaveChangesAsync();
            await RemoveAllFromKorb();
        }
    }
}
