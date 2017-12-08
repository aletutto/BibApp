using BibApp.Models.Benutzer;
using BibApp.Models.Buch;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BibApp.Data
{
    public class DbInitializer
    {
        private readonly BibContext _context;
        private readonly UserManager<Benutzer> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(
        BibContext context,
        UserManager<Benutzer> userManager,
        RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task Initialize()
        {

            string[] roleNames = { "Admin", "Member" };
            IdentityResult roleResult;

            // Look for Roles.
            foreach (var roleName in roleNames)
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Look for any users.
            if (!_context.Benutzers.Any())
            {
                var poweruser = new Benutzer
                {
                    UserName = "dude",
                    Email = "dude2000@gmail.com",
                    Role = "Admin"
                };
                var testuser = new Benutzer
                {
                    UserName = "member",
                    Email = "member@gmail.com",
                    Role = "Member"
                };

                string userPWD = "dude";
                string testPWD = "member";
                var _user = await _userManager.FindByEmailAsync("dude2000@gmail.com");
                var _test = await _userManager.FindByEmailAsync("member@gmail.com");

                if (_user == null)
                {
                    var createPowerUser = await _userManager.CreateAsync(poweruser, userPWD);
                    var createTestUser = await _userManager.CreateAsync(testuser, testPWD);
                    if (createPowerUser.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(poweruser, "Admin");
                        await _userManager.AddToRoleAsync(testuser, "Member");
                    }
                }
            }
            

            var buecher = new Buch[]
            {
                new Buch{ISBN="1234-567-8910", Titel="Deutsch 1", Autor="Max H.", Erscheinungsjahr=2010, Regal=4, Reihe=1, Verlag="Beuth Verlag", AnzahlExemplare=1},
                new Buch{ISBN="1235-567-8910", Titel="ITIL V3", Autor="Max B.", Erscheinungsjahr=2009, Regal=2, Reihe=4, Verlag="Beuth Verlag", AnzahlExemplare=2},
                new Buch{ISBN="1236-567-8910", Titel="Java für Dummies", Autor="Max D.", Erscheinungsjahr=2000, Regal=1, Reihe=3, Verlag="Kühlen Verlag", AnzahlExemplare=1},
                new Buch{ISBN="1237-567-8910", Titel="SQL Datenbanken", Autor="Max A.", Erscheinungsjahr=2015, Regal=3, Reihe=2, Verlag="CARLSEN Verlag", AnzahlExemplare=3}
            };
            foreach (Buch buch in buecher)
            {
                _context.Buecher.Add(buch);

                for(int i = 1; i <= buch.AnzahlExemplare; i++)
                {
                    var exemplar = new Exemplar { ExemplarId = i, ISBN = buch.ISBN, Verfügbarkeit = true, IstVorgemerkt = false };
                    _context.Exemplare.Add(exemplar);
                }
               
            }
            _context.SaveChanges();
        }
    }
}