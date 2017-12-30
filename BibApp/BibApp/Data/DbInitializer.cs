using BibApp.Models.Benutzer;
using BibApp.Models.Buch;
using BibApp.Models.Warenkorb;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BibApp.Data
{
    public class DbInitializer
    {
        private readonly BibContext bibContext;
        private readonly UserManager<Benutzer> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public DbInitializer(
            BibContext bibContext,
            UserManager<Benutzer> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            this.bibContext = bibContext;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task Initialize()
        {

            string[] roleNames = { "Admin", "Member" };
            IdentityResult roleResult;

            // Erstelle alle Benutzer-Rollen
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Erstelle alle Benutzer
            if (!bibContext.Benutzers.Any())
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

                var user1 = new Benutzer
                {
                    UserName = "pedid001",
                    Email = "peter.diedrich@stud.hn.de",
                    Role = "Member"
                };

                var user2 = new Benutzer
                {
                    UserName = "hamue001",
                    Email = "hans.müller@stud.hn.de",
                    Role = "Member"
                };

                var user3 = new Benutzer
                {
                    UserName = "readl001",
                    Email = "rene.adler@stud.hn.de",
                    Role = "Member"
                };

                var user4 = new Benutzer
                {
                    UserName = "reaug001",
                    Email = "renato.augusto@stud.hn.de",
                    Role = "Member"
                };

                var user5 = new Benutzer
                {
                    UserName = "misch001",
                    Email = "michael.schumacher@stud.hn.de",
                    Role = "Member"
                };

                string powerUserPWD = "dude";
                string testPWD = "member";
                string userPWD = "member";

                var powerUserTest = await userManager.FindByEmailAsync("dude2000@gmail.com");
                var testUserTest = await userManager.FindByEmailAsync("member@gmail.com");
                var user1Test = await userManager.FindByEmailAsync("peter.diedrich@stud.hn.de");
                var user2Test = await userManager.FindByEmailAsync("hans.müller@stud.hn.de");
                var user3Test = await userManager.FindByEmailAsync("rene.adler@stud.hn.de");
                var user4Test = await userManager.FindByEmailAsync("renato.augusto@stud.hn.de");
                var user5Test = await userManager.FindByEmailAsync("michael.schumacher@stud.hn.de");

                if (powerUserTest == null)
                {
                    var createPowerUser = await userManager.CreateAsync(poweruser, powerUserPWD);

                    if (createPowerUser.Succeeded)
                    {
                        await userManager.AddToRoleAsync(poweruser, "Admin");
                    }
                }

                if (testUserTest == null)
                {
                    var createTestUser = await userManager.CreateAsync(testuser, testPWD);

                    if (createTestUser.Succeeded)
                    {
                        await userManager.AddToRoleAsync(testuser, "Member");
                    }
                }

                if (user1Test == null)
                {
                    var createUser1 = await userManager.CreateAsync(user1, userPWD);

                    if (createUser1.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user1, "Member");
                    }
                }

                if (user2Test == null)
                {
                    var createUser2 = await userManager.CreateAsync(user2, userPWD);

                    if (createUser2.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user2, "Member");
                    }
                }

                if (user3Test == null)
                {
                    var createUser3 = await userManager.CreateAsync(user3, userPWD);

                    if (createUser3.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user3, "Member");
                    }
                }

                if (user4Test == null)
                {
                    var createUser4 = await userManager.CreateAsync(user4, userPWD);

                    if (createUser4.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user4, "Member");
                    }
                }

                if (user5Test == null)
                {
                    var createUser5 = await userManager.CreateAsync(user5, userPWD);

                    if (createUser5.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user5, "Member");
                    }
                }
            }

            // Alle Bücher erstellen
            if (!bibContext.Buecher.Any())
            {
                var buecher = new Buch[]
                {
                    new Buch{ISBN="978-3470430287", Titel="Kompendium Wirtschaftsrecht", Autor="Brunhilde Steckler, Dimitra Tekidou-Kühlke", Erscheinungsjahr=2016, Regal=4, Reihe=1, Verlag="NWB", AnzahlExemplare=2},
                    new Buch{ISBN="978-3933070661", Titel="Andersens Märchen", Autor="Hans Christian Andersen", Erscheinungsjahr=2007, Regal=1, Reihe=2, Verlag="Edition Lempers ", AnzahlExemplare=3},
                    new Buch{ISBN="978-3866471788", Titel="Stolz und Vorurteil", Autor="Jane Austen", Erscheinungsjahr=2007, Regal=4, Reihe=1, Verlag="Anaconda", AnzahlExemplare=1},
                    new Buch{ISBN="978-3596900732", Titel="Vater Goriot", Autor="Honoré de Balzac", Erscheinungsjahr=2008, Regal=22, Reihe=3, Verlag="Fischer", AnzahlExemplare=4},
                    new Buch{ISBN="978-3866473720", Titel="Das Dekameron", Autor="Giovanni Boccaccio", Erscheinungsjahr=2009, Regal=6, Reihe=2, Verlag="Anaconda", AnzahlExemplare=5},
                    new Buch{ISBN="978-3518456729", Titel="Drei Romane. Molloy. Malone stirbt. Der Namenlose", Autor="Samuel Beckett", Erscheinungsjahr=2005, Regal=10, Reihe=4, Verlag="Suhrkamp", AnzahlExemplare=1},
                    new Buch{ISBN="978-3499221897", Titel="Der Fremde", Autor="Albert Camus", Erscheinungsjahr=1996, Regal=13, Reihe=2, Verlag="Rowohlt", AnzahlExemplare=2},

                    new Buch{ISBN="978-3518456651", Titel="Die Gedichte. Kommentierte Gesamtausgabe", Autor="Paul Celan", Erscheinungsjahr=2005, Regal=15, Reihe=3, Verlag="Suhrkamp", AnzahlExemplare=3},
                    new Buch{ISBN="978-3499236587", Titel="Reise ans Ende der Nacht", Autor="Louis-Ferdinand Céline", Erscheinungsjahr=2004, Regal=14, Reihe=2, Verlag="Rowohlt", AnzahlExemplare=2},
                    new Buch{ISBN="978-3491960831", Titel="Don Quijote", Autor="Miguel de Cervantes", Erscheinungsjahr=2004, Regal=16, Reihe=4, Verlag="Albatros", AnzahlExemplare=5},
                    new Buch{ISBN="978-3866472174", Titel="Die Canterbury-Erzählungen", Autor="Geoffrey Chaucer", Erscheinungsjahr=2008, Regal=17, Reihe=3, Verlag="Anaconda", AnzahlExemplare=3},
                    new Buch{ISBN="978-3423126489", Titel="Nostromo", Autor="Joseph Conrad", Erscheinungsjahr=1999, Regal=19, Reihe=1, Verlag="DTV", AnzahlExemplare=2},
                    new Buch{ISBN="978-3150007969", Titel="Die Göttliche Komödie", Autor="Dante Alighieri", Erscheinungsjahr=1986, Regal=20, Reihe=2, Verlag="Reclam", AnzahlExemplare=2},
                    new Buch{ISBN="978-3150015629", Titel="Grosse Erwartungen", Autor="Charles Dickens", Erscheinungsjahr=1993, Regal=4, Reihe=1, Verlag="Reclam", AnzahlExemplare=1},
                    new Buch{ISBN="978-3150093351", Titel="Jacques der Fatalist und sein Herr", Autor="Denis Diderot", Erscheinungsjahr=1972, Regal=4, Reihe=1, Verlag="Reclam", AnzahlExemplare=1},
                    new Buch{ISBN="978-3423002950", Titel="Berlin Alexanderplatz: Die Geschichte vom Franz Biberkopf", Autor="Alfred Döblin", Erscheinungsjahr=2002, Regal=22, Reihe=4, Verlag="DTV", AnzahlExemplare=4},
                    new Buch{ISBN="978-3596129973", Titel="Verbrechen und Strafe", Autor="Fjodor M. Dostojewskij", Erscheinungsjahr=1996, Regal=3, Reihe=3, Verlag="Fischer", AnzahlExemplare=2},

                    new Buch{ISBN="978-3492253420", Titel="Die Dämonen", Autor="Fjodor M. Dostojewski", Erscheinungsjahr=2008, Regal=2, Reihe=1, Verlag="Piper", AnzahlExemplare=2},
                    new Buch{ISBN="978-3596135103", Titel="Der Idiot", Autor="Fjodor M. Dostojewskij", Erscheinungsjahr=1998, Regal=10, Reihe=3, Verlag="Fischer", AnzahlExemplare=1},
                    new Buch{ISBN="978-3491961234", Titel="Die Brüder Karamasow", Autor="Fjodor M. Dostojewskij", Erscheinungsjahr=2004, Regal=5, Reihe=3, Verlag="Patmos", AnzahlExemplare=3},
                    new Buch{ISBN="978-3717580027", Titel="Middlemarch", Autor="George Eliot", Erscheinungsjahr=1962, Regal=6, Reihe=2, Verlag="Manesse", AnzahlExemplare=4},
                    new Buch{ISBN="978-3499138782", Titel="Der unsichtbare Mann", Autor="Ralph Ellison", Erscheinungsjahr=1998, Regal=7, Reihe=3, Verlag="Rowohlt", AnzahlExemplare=3},
                    new Buch{ISBN="978-3150079782", Titel="Medea: Griech. / Dt. ", Autor="Euripides", Erscheinungsjahr=1986, Regal=8, Reihe=4, Verlag="Reclam", AnzahlExemplare=2},
                    new Buch{ISBN="978-3257207217", Titel="Madame Bovary: Sitten der Provinz", Autor="Gustave Flaubert", Erscheinungsjahr=2005, Regal=10, Reihe=3, Verlag="Diogenes", AnzahlExemplare=1},
                    new Buch{ISBN="978-3518223567", Titel="Zigeunerromanzen: Primer romancero gitano", Autor="Federico García Lorca", Erscheinungsjahr=2002, Regal=14, Reihe=1, Verlag="Suhrkamp", AnzahlExemplare=2},
                    new Buch{ISBN="978-3596509812", Titel="Hundert Jahre Einsamkeit", Autor="Gabriel García Márquez", Erscheinungsjahr=2007, Regal=11, Reihe=2, Verlag="Fischer", AnzahlExemplare=3},
                    new Buch{ISBN="978-3596510214", Titel="Die Liebe in den Zeiten der Cholera", Autor="Gabriel García Márquez", Erscheinungsjahr=2007, Regal=15, Reihe=4, Verlag="Fischer", AnzahlExemplare=4},

                    new Buch{ISBN="978-3423124003", Titel="Faust: Eine Tragödie (Erster und zweiter Teil)", Autor="Johann Wolfgang von Goethe", Erscheinungsjahr=1997, Regal=15, Reihe=4, Verlag="Deutscher Taschenbuch Verlag", AnzahlExemplare=5},
                    new Buch{ISBN="978-3866471757", Titel="Die toten Seelen", Autor="Nikolaj Gogol", Erscheinungsjahr=2007, Regal=8, Reihe=2, Verlag="Anaconda", AnzahlExemplare=4},
                    new Buch{ISBN="978-3423118217", Titel="Die Blechtrommel", Autor="Günter Grass", Erscheinungsjahr=1993, Regal=14, Reihe=4, Verlag="DTV", AnzahlExemplare=3},
                    new Buch{ISBN="978-3425855625", Titel="Grande Sertão", Autor="João Guimarães Rosa", Erscheinungsjahr=1964, Regal=14, Reihe=2, Verlag="Kiepenheuer & Witsch", AnzahlExemplare=1},
                    new Buch{ISBN="978-3423252997", Titel="Hunger", Autor="Knut Hamsun", Erscheinungsjahr=2009, Regal=18, Reihe=3, Verlag="Deutscher Taschenbuch Verlag", AnzahlExemplare=2},
                    new Buch{ISBN="978-3499226014", Titel="Der alte Mann und das Meer", Autor="Ernest Hemingway", Erscheinungsjahr=2012, Regal=19, Reihe=2, Verlag="Rowohlt", AnzahlExemplare=1},
                    new Buch{ISBN="978-3423130004", Titel="Ilias · Odyssee", Autor="Homer", Erscheinungsjahr=2002, Regal=20, Reihe=4, Verlag="dtv Verlagsgesellschaft", AnzahlExemplare=1},
                    new Buch{ISBN="978-3596900473", Titel="Nora oder Ein Puppenheim", Autor="Henrik Ibsen", Erscheinungsjahr=2008, Regal=21, Reihe=4, Verlag="Fischer", AnzahlExemplare=1},
                    new Buch{ISBN="978-3938484777", Titel="Der Prozess", Autor="Franz Kafka", Erscheinungsjahr=2006, Regal=9, Reihe=3, Verlag="Anaconda", AnzahlExemplare=1},
                    new Buch{ISBN="978-3866471061", Titel="Das Schloß", Autor="Franz Kafka", Erscheinungsjahr=2007, Regal=7, Reihe=2, Verlag="Anaconda", AnzahlExemplare=1},

                    new Buch{ISBN="978-3250104650", Titel="Sakuntala - Ein Drama in sieben Akten", Autor="Kalidasa", Erscheinungsjahr=2004, Regal=4, Reihe=1, Verlag="Ammann", AnzahlExemplare=2},
                    new Buch{ISBN="978-3423112970", Titel="Ein Kirschbaum im Winter", Autor="Yasunari Kawabata", Erscheinungsjahr=1990, Regal=7, Reihe=3, Verlag="Deutscher Taschenbuch Verlag", AnzahlExemplare=2},
                    new Buch{ISBN="978-3866472976", Titel="Alexis Sorbas", Autor="Nikos Kazantzakis", Erscheinungsjahr=2008, Regal=2, Reihe=4, Verlag="Anaconda", AnzahlExemplare=2},
                    new Buch{ISBN="978-3499142123", Titel="Söhne und Liebhaber", Autor="D. H. Lawrence", Erscheinungsjahr=1967, Regal=3, Reihe=1, Verlag="Rowohlt", AnzahlExemplare=2},
                    new Buch{ISBN="978-3882438581", Titel="Sein eigener Herr", Autor="Halldór Laxness", Erscheinungsjahr=2002, Regal=6, Reihe=2, Verlag="Steidl Göttingen", AnzahlExemplare=4},
                    new Buch{ISBN="978-3596253968", Titel="Das goldene Notizbuch", Autor="Doris Lessing", Erscheinungsjahr=1989, Regal=12, Reihe=3, Verlag="Fischer", AnzahlExemplare=4},
                    new Buch{ISBN="978-3789129445", Titel="Pippi Langstrumpf", Autor="Astrid Lindgren", Erscheinungsjahr=1987, Regal=16, Reihe=2, Verlag="Oetinger Verlag", AnzahlExemplare=4},
                    new Buch{ISBN="978-3293004085", Titel="Das trunkene Land: Erzählungen", Autor="Lu Xun", Erscheinungsjahr=2009, Regal=18, Reihe=2, Verlag="Unionsverlag", AnzahlExemplare=4},
                    new Buch{ISBN="978-3293200500", Titel="Die Kinder unseres Viertels", Autor="Nagib Machfus, Naguib Mahfouz", Erscheinungsjahr=2006, Regal=19, Reihe=3, Verlag="Unionsverlag", AnzahlExemplare=2},
                    new Buch{ISBN="978-3596294312", Titel="Buddenbrooks. Verfall einer Familie", Autor="Thomas Mann", Erscheinungsjahr=2008, Regal=20, Reihe=2, Verlag="Fischer", AnzahlExemplare=3},

                    new Buch{ISBN=" 978-3596294336", Titel="Der Zauberberg", Autor="Thomas Mann", Erscheinungsjahr=1991, Regal=5, Reihe=1, Verlag="Fischer", AnzahlExemplare=2},
                    new Buch{ISBN="978-3800054794", Titel="Moby Dick", Autor="Herman Melville", Erscheinungsjahr=2009, Regal=6, Reihe=2, Verlag="Ueberreuter", AnzahlExemplare=1},
                    new Buch{ISBN="978-3938484401", Titel="Die Essais", Autor="Michel de Montaigne", Erscheinungsjahr=2005, Regal=7, Reihe=3, Verlag="Anaconda", AnzahlExemplare=1},
                    new Buch{ISBN="978-3492245647", Titel="La Storia", Autor="Elsa Morante", Erscheinungsjahr=2005, Regal=8, Reihe=4, Verlag="Piper", AnzahlExemplare=5},
                    new Buch{ISBN="978-3499244209", Titel="Menschenkind", Autor="Toni Morrison", Erscheinungsjahr=2007, Regal=9, Reihe=1, Verlag="Rowohlt", AnzahlExemplare=6},
                    new Buch{ISBN="978-3458333593", Titel="Die Geschichte vom Prinzen Genji", Autor="Murasaki", Erscheinungsjahr=1994, Regal=10, Reihe=2, Verlag="Insel", AnzahlExemplare=2},
                    new Buch{ISBN="978-3499134623", Titel="Der Mann ohne Eigenschaften I: Erstes und Zweites Buch", Autor="Robert Musil", Erscheinungsjahr=1994, Regal=11, Reihe=3, Verlag="Rowohlt", AnzahlExemplare=2},
                    new Buch{ISBN="978-3499225437", Titel="Lolita", Autor="Vladimir Nabokov", Erscheinungsjahr=1999, Regal=3, Reihe=1, Verlag="Rowohlt", AnzahlExemplare=3},
                    new Buch{ISBN="978-3938484074", Titel="Metamorphosen", Autor="Ovid", Erscheinungsjahr=2005, Regal=4, Reihe=1, Verlag="Anaconda", AnzahlExemplare=4},
                    new Buch{ISBN="978-3250250029", Titel="Das Buch der Unruhe", Autor="Fernando Pessoa", Erscheinungsjahr=2008, Regal=13, Reihe=1, Verlag="Ammann", AnzahlExemplare=2},

                    new Buch{ISBN="978-3458069164", Titel="Sämtliche Erzählungen in vier Bänden", Autor="Edgar Allan Poe", Erscheinungsjahr=2008, Regal=14, Reihe=3, Verlag="Insel", AnzahlExemplare=4},
                    new Buch{ISBN="978-3518397091", Titel="Auf der Suche nach der verlorenen Zeit", Autor="Marcel Proust", Erscheinungsjahr=2000, Regal=18, Reihe=3, Verlag="Suhrkamp", AnzahlExemplare=2},
                    new Buch{ISBN="978-3446230668", Titel="Pedro Páramo", Autor="Juan Rulfo", Erscheinungsjahr=2008, Regal=16, Reihe=4, Verlag="Carl Hanser", AnzahlExemplare=3},
                    new Buch{ISBN="978-3906005027", Titel="Das Masnavi: Erstes Buch", Autor="Moulana Jalaluddin Rumi", Erscheinungsjahr=2012, Regal=14, Reihe=3, Verlag="Edition Shershir", AnzahlExemplare=2},
                    new Buch{ISBN="978-3499238321", Titel="Mitternachtskinder", Autor="Salman Rushdie", Erscheinungsjahr=2005, Regal=3, Reihe=1, Verlag="Rowohlt", AnzahlExemplare=3},
                    new Buch{ISBN="978-0141187204", Titel="Season of Migration to the North", Autor="Tayeb Salih", Erscheinungsjahr=2003, Regal=16, Reihe=2, Verlag="Penguin Classics", AnzahlExemplare=2},
                    new Buch{ISBN="978-3499224676", Titel="Die Stadt der Blinden", Autor="José Saramago", Erscheinungsjahr=1999, Regal=13, Reihe=4, Verlag="Rowohlt", AnzahlExemplare=4},
                    new Buch{ISBN="978-3704321206", Titel="Krieg und Frieden", Autor="Leo N Tolstoi", Erscheinungsjahr=2005, Regal=20, Reihe=4, Verlag="Neuer Kaiser", AnzahlExemplare=3},
                    new Buch{ISBN="978-3491961968", Titel="Anna Karenina", Autor="Leo N. Tolstoi", Erscheinungsjahr=2007, Regal=3, Reihe=1, Verlag="Patmos", AnzahlExemplare=4},

                    new Buch{ISBN="978-3821807331", Titel="Leben und Ansichten von Tristram Shandy, Gentleman", Autor="Laurence Sterne", Erscheinungsjahr=2006, Regal=21, Reihe=1, Verlag="Anaconda", AnzahlExemplare=2},
                    new Buch{ISBN="978-3499134852", Titel="Zeno Cosini", Autor="Italo Svevo", Erscheinungsjahr=1990, Regal=5, Reihe=2, Verlag="Rowohlt", AnzahlExemplare=3},
                    new Buch{ISBN="978-3458347583", Titel="Gullivers Reisen", Autor="Jonathan Swift", Erscheinungsjahr=2004, Regal=12, Reihe=2, Verlag="Insel", AnzahlExemplare=3},
                    new Buch{ISBN="978-3458341277", Titel="Der Tod des Iwan Iljitsch", Autor="Lew Tolstoj", Erscheinungsjahr=2002, Regal=13, Reihe=3, Verlag="Insel", AnzahlExemplare=4},
                    new Buch{ISBN="978-3257217025", Titel="Meistererzählungen", Autor="Anton Cechov, Anton Tschechow", Erscheinungsjahr=2000, Regal=4, Reihe=1, Verlag="Diogenes", AnzahlExemplare=2},
                    new Buch{ISBN="978-3899055788", Titel="Tausend und eine Nacht", Autor="K.A.", Erscheinungsjahr=2007, Regal=4, Reihe=1, Verlag="ADAC", AnzahlExemplare=3},
                    new Buch{ISBN="978-3866471771", Titel="Die Abenteuer des Huckleberry Finn", Autor="Mark Twain, Samuel Clemens", Erscheinungsjahr=2007, Regal=4, Reihe=1, Verlag="Anaconda", AnzahlExemplare=2},
                    new Buch{ISBN="978-3938484081", Titel="Aeneis", Autor="Vergil", Erscheinungsjahr=2005, Regal=4, Reihe=1, Verlag="Anaconda", AnzahlExemplare=3},
                    new Buch{ISBN="978-3257213515", Titel="Grashalme", Autor="Walt Whitman", Erscheinungsjahr=1985, Regal=4, Reihe=1, Verlag="Diogenes", AnzahlExemplare=2},
                    new Buch{ISBN="978-3596140022", Titel="Mrs Dalloway", Autor="Virginia Woolf", Erscheinungsjahr=1997, Regal=4, Reihe=1, Verlag="Fischer", AnzahlExemplare=2},

                    new Buch{ISBN="978-3648065174", Titel="Agiles Projektmanagement: Scrum, Use Cases, Task Boards & Co.", Autor="Jörg Preußig", Erscheinungsjahr=2015, Regal=4, Reihe=1, Verlag="Haufe Lexware", AnzahlExemplare=2},
                    new Buch{ISBN="978-3864902611", Titel="Scrum - verstehen und erfolgreich einsetzen", Autor="Stefan Roock,Henning Wolf", Erscheinungsjahr=2015, Regal=4, Reihe=1, Verlag="dpunkt", AnzahlExemplare=1},
                    new Buch{ISBN="978-3648073278", Titel="Projektmanagement", Autor="Hans-D. Litke,Ilonka Kunow,Heinz Schulz-Wimmer", Erscheinungsjahr=2015, Regal=4, Reihe=1, Verlag="CreateSpace", AnzahlExemplare=4},
                    new Buch{ISBN="978-3636012913", Titel="Überleben im Projekt", Autor="Klaus D. Tumuscheit", Erscheinungsjahr=2007, Regal=15, Reihe=4, Verlag="Redline", AnzahlExemplare=5},
                    new Buch{ISBN="978-3658082833", Titel="Intensivtraining Projektmanagement", Autor="Walter Jakoby", Erscheinungsjahr=2015, Regal=12, Reihe=6, Verlag="van Ons", AnzahlExemplare=3},
                    new Buch{ISBN="978-3895784538", Titel="Tools für Projektmanagement, Workshops und Consulting", Autor="Nicolai Andler", Erscheinungsjahr=2015, Regal=17, Reihe=3, Verlag="van Ons", AnzahlExemplare=2},
                    new Buch{ISBN="978-3000543401", Titel="PROJEKT-INSZENATOR", Autor="Alexander Mereien", Erscheinungsjahr=2017, Regal=4, Reihe=1, Verlag="van Ons", AnzahlExemplare=1},
                    new Buch{ISBN="978-3636012050", Titel="Projektcontrolling leicht gemacht", Autor="Ulrich Ch Füting, Ingo Hahn", Erscheinungsjahr=2005, Regal=15, Reihe=2, Verlag="REDLINE", AnzahlExemplare=1},
                    new Buch{ISBN="978-3446447974", Titel="Projektmanagement mit Excel", Autor="Ignatz Schels, Uwe M. Seidel", Erscheinungsjahr=2016, Regal=8, Reihe=3, Verlag="Carl Hanser", AnzahlExemplare=4},
                    new Buch{ISBN="978-3446440746", Titel="Handbuch IT-Projektmanagement", Autor="Ernst Tiemeyer", Erscheinungsjahr=2014, Regal=9, Reihe=3, Verlag="Carl Hanser", AnzahlExemplare=2},

                    new Buch{ISBN="978-3110335286", Titel="Wirtschaftsinformatik", Autor="Hans Robert Hansen", Erscheinungsjahr=2015, Regal=11, Reihe=1, Verlag="De Gruyter Oldenbourg", AnzahlExemplare=3},
                    new Buch{ISBN="978-3849700881", Titel="Einführung in die Teamarbeit", Autor="Cornelia Edding,Karl Schattenhofer", Erscheinungsjahr=2015, Regal=14, Reihe=3, Verlag="Carl-Auer", AnzahlExemplare=2},
                    new Buch{ISBN="978-3658098025", Titel="Marketing: Grundlagen für Studium und Praxis", Autor="Manfred Bruhn", Erscheinungsjahr=2016, Regal=19, Reihe=3, Verlag="Springer Gabler", AnzahlExemplare=2},
                    new Buch{ISBN="978-3658136550", Titel="Marketingmanagement", Autor="Christian Homburg", Erscheinungsjahr=2016, Regal=10, Reihe=4, Verlag="Springer Gabler", AnzahlExemplare=4},
                    new Buch{ISBN="978-3658086886", Titel="Kundenzufriedenheit", Autor="Christian Homburg", Erscheinungsjahr=2015, Regal=18, Reihe=1, Verlag="Springer Gabler", AnzahlExemplare=2},
                    new Buch{ISBN="978-3642371158", Titel="Erfolgsfaktor Kundenzufriedenheit", Autor="Hansjörg Künzel", Erscheinungsjahr=2013, Regal=7, Reihe=2, Verlag="Springer Gabler", AnzahlExemplare=3},
                    new Buch{ISBN="978-1539744986", Titel="Online-Marketing-Konzeption - 2017", Autor="Erwin Lammenett", Erscheinungsjahr=2017, Regal=4, Reihe=1, Verlag="CreateSpace", AnzahlExemplare=4},
                    new Buch{ISBN="978-3658154936", Titel="Praxiswissen Online-Marketing", Autor="Erwin Lammenett", Erscheinungsjahr=2017, Regal=16, Reihe=2, Verlag="Springer Gabler", AnzahlExemplare=2},
                    new Buch{ISBN="978-3658163785", Titel="Grundkurs Wirtschaftsinformatik", Autor="Dietmar Abts, Wilhelm Mülder", Erscheinungsjahr=2017, Regal=6, Reihe=1, Verlag="Springer Vieweg", AnzahlExemplare=8},
                    new Buch{ISBN="978-3834800022", Titel="Masterkurs Wirtschaftsinformatik", Autor="Dietmar Abts, Wilhelm Mülder", Erscheinungsjahr=2009, Regal=4, Reihe=1, Verlag="Vieweg+Teubner", AnzahlExemplare=6},

                };

                // Erstelle die zugehörigen Exemplare
                foreach (Buch buch in buecher)
                {
                    bibContext.Buecher.Add(buch);

                    for (int i = 1; i <= buch.AnzahlExemplare; i++)
                    {
                        var exemplar = new Exemplar { ExemplarId = i, ISBN = buch.ISBN, Verfügbarkeit = true, IstVorgemerkt = false };
                        bibContext.Exemplare.Add(exemplar);
                    }
                }

                await bibContext.SaveChangesAsync();

                if (!bibContext.AdminWarenkoerbe.Any())
                {
                    // 1. Buch verleihen
                    var buch1 = bibContext.Buecher.SingleOrDefault(e => e.ISBN.Equals("978-3658163785"));
                    var exemplar1 = bibContext.Exemplare.SingleOrDefault(e => e.ISBN.Equals("978-3658163785") && e.ExemplarId == 1);
                    
                    var adminKorb1 = new AdminKorb {
                        Benutzer = "member",
                        ISBN = exemplar1.ISBN,
                        BuchTitel = buch1.Titel,
                        ExemplarId = exemplar1.ExemplarId,
                        IstVerliehen = true
                    };

                    bibContext.AdminWarenkoerbe.Add(adminKorb1);

                    exemplar1.Verfügbarkeit = false;
                    exemplar1.EntliehenVom = DateTime.Now.AddDays(-32);
                    exemplar1.EntliehenBis = DateTime.Now.AddDays(-2);

                    bibContext.Exemplare.Update(exemplar1);

                    // 2. Buch verleihen
                    var buch2 = bibContext.Buecher.SingleOrDefault(e => e.ISBN.Equals("978-3834800022"));
                    var exemplar2 = bibContext.Exemplare.SingleOrDefault(e => e.ISBN.Equals("978-3834800022") && e.ExemplarId == 1);

                    var adminKorb2 = new AdminKorb
                    {
                        Benutzer = "member",
                        ISBN = exemplar2.ISBN,
                        BuchTitel = buch2.Titel,
                        ExemplarId = exemplar2.ExemplarId,
                        IstVerliehen = true
                    };

                    bibContext.AdminWarenkoerbe.Add(adminKorb2);

                    exemplar2.Verfügbarkeit = false;
                    exemplar2.EntliehenVom = DateTime.Now.AddDays(-25);
                    exemplar2.EntliehenBis = DateTime.Now.AddDays(5);

                    bibContext.Exemplare.Update(exemplar2);

                    // 3. Buch verleihen
                    var buch3 = bibContext.Buecher.SingleOrDefault(e => e.ISBN.Equals("978-3938484081"));
                    var exemplar3 = bibContext.Exemplare.SingleOrDefault(e => e.ISBN.Equals("978-3938484081") && e.ExemplarId == 1);

                    var adminKorb3 = new AdminKorb
                    {
                        Benutzer = "misch001",
                        ISBN = exemplar3.ISBN,
                        BuchTitel = buch3.Titel,
                        ExemplarId = exemplar3.ExemplarId,
                        IstVerliehen = true
                    };

                    bibContext.AdminWarenkoerbe.Add(adminKorb3);

                    exemplar3.Verfügbarkeit = false;
                    exemplar3.EntliehenVom = DateTime.Now.AddDays(-15);
                    exemplar3.EntliehenBis = DateTime.Now.AddDays(15);

                    bibContext.Exemplare.Update(exemplar3);

                    // 4. Buch verleihen
                    var buch4 = bibContext.Buecher.SingleOrDefault(e => e.ISBN.Equals("978-3596135103"));
                    var exemplar4 = bibContext.Exemplare.SingleOrDefault(e => e.ISBN.Equals("978-3596135103") && e.ExemplarId == 1);

                    var adminKorb4 = new AdminKorb
                    {
                        Benutzer = "misch001",
                        ISBN = exemplar4.ISBN,
                        BuchTitel = buch4.Titel,
                        ExemplarId = exemplar4.ExemplarId,
                        IstVerliehen = true
                    };

                    bibContext.AdminWarenkoerbe.Add(adminKorb4);

                    exemplar4.Verfügbarkeit = false;
                    exemplar4.EntliehenVom = DateTime.Now.AddDays(-26);
                    exemplar4.EntliehenBis = DateTime.Now.AddDays(4);

                    bibContext.Exemplare.Update(exemplar4);

                    // 5. Buch verleihen
                    var buch5 = bibContext.Buecher.SingleOrDefault(e => e.ISBN.Equals("978-3866472976"));
                    var exemplar5 = bibContext.Exemplare.SingleOrDefault(e => e.ISBN.Equals("978-3866472976") && e.ExemplarId == 1);

                    var adminKorb5 = new AdminKorb
                    {
                        Benutzer = "readl001",
                        ISBN = exemplar5.ISBN,
                        BuchTitel = buch5.Titel,
                        ExemplarId = exemplar5.ExemplarId,
                        IstVerliehen = true
                    };

                    bibContext.AdminWarenkoerbe.Add(adminKorb5);

                    exemplar5.Verfügbarkeit = false;
                    exemplar5.EntliehenVom = DateTime.Now.AddDays(-34);
                    exemplar5.EntliehenBis = DateTime.Now.AddDays(-4);

                    bibContext.Exemplare.Update(exemplar5);
                }

                await bibContext.SaveChangesAsync();
            }
        }
    }
}