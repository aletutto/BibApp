using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Linq;
using BibApp.Models.Benutzer;
using BibApp.Models;
using NToastNotify;

namespace BibApp.Controllers
{
    public class BenutzerController : Controller
    {
        private readonly BibContext bibContext;
        private readonly UserManager<Benutzer> userManager;
        private readonly SignInManager<Benutzer> signInManager;
        private readonly IToastNotification toastNotification;

        public BenutzerController(
            UserManager<Benutzer> userManager,
            SignInManager<Benutzer> signInManager,
            IToastNotification toastNotification,
            BibContext bibContext)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.toastNotification = toastNotification;
            this.bibContext = bibContext;
        }

        // GET: Benutzer/Index
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            // Suchfeld und Sortierungsdaten
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["RoleSortParm"] = sortOrder == "Role" ? "role_desc" : "Role";
            ViewData["EmailSortParm"] = sortOrder == "Email" ? "email_desc" : "Email";
            ViewData["CurrentFilter"] = searchString;

            var benutzer = from s in bibContext.Benutzer
                           select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                benutzer = benutzer.Where(s => s.UserName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    benutzer = benutzer.OrderByDescending(s => s.UserName);
                    break;
                case "role_desc":
                    benutzer = benutzer.OrderByDescending(s => s.Role);
                    break;
                case "Role":
                    benutzer = benutzer.OrderBy(s => s.Role);
                    break;
                case "email_desc":
                    benutzer = benutzer.OrderByDescending(s => s.Email);
                    break;
                case "Email":
                    benutzer = benutzer.OrderBy(s => s.Email);
                    break;
                default:
                    benutzer = benutzer.OrderBy(s => s.UserName);
                    break;
            }
            return View(await benutzer.AsNoTracking().ToListAsync());
        }

        // GET: Benutzer/Details
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(String id)
        {
            // Suche den ausgewählten Benutzer in der Datenbank anhand der ID
            var user = await bibContext.Benutzer
                .SingleOrDefaultAsync(m => m.Id == id);

            return View(user);
        }

        // GET: Benutzer/Edit
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(String id)
        {
            // Suche den ausgewählten Benutzer in der Datenbank anhand der ID
            var user = await bibContext.Benutzer.SingleOrDefaultAsync(m => m.Id == id);
            ViewData["role"] = user.Role;

            return View(user);
        }

        // POST: Benutzer/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(String id, [Bind("UserName, Email")] Benutzer model, String Role)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await userManager.FindByIdAsync(id);
                    var newName = user.UserName;

                    if (model.UserName == null) // Fall: Benutzername ist leer.
                    {
                        toastNotification.AddToastMessage("Kein Benutzername eingegeben", "Es muss ein Benutzername vergeben werden.", ToastEnums.ToastType.Error, new ToastOption()
                        {
                            PositionClass = ToastPositions.TopCenter
                        });

                        return RedirectToAction(nameof(Edit));
                    }
                    else if (model.Email == null) // Fall: Email ist leer.
                    {
                        toastNotification.AddToastMessage("Kein Email eingegeben", "Es muss eine Email-Adresse vergeben werden.", ToastEnums.ToastType.Error, new ToastOption()
                        {
                            PositionClass = ToastPositions.TopCenter
                        });

                        return RedirectToAction(nameof(Edit));
                    }
                    else
                    {
                        // Fall: Der Benutzername wurde geändert.
                        if (model.UserName != newName)
                        {
                            var userExist = bibContext.Benutzer.SingleOrDefault(b => b.UserName == model.UserName);

                            // Fall: Der neue Benutzername existiert bereits in der Datenbank.
                            if (userExist != null)
                            {
                                toastNotification.AddToastMessage("Benutzername bereits vergeben", "Der Benutzername \"" + model.UserName + "\" ist bereits vergeben.", ToastEnums.ToastType.Error, new ToastOption()
                                {
                                    PositionClass = ToastPositions.TopCenter
                                });

                                return RedirectToAction(nameof(Edit));
                            }

                            // Alle bisherigern Referenzen auf den neuen Namen ändern
                            var warenkorb = bibContext.Warenkorb.Where(e => e.Benutzer.Equals(user.UserName));
                            foreach (var warenkorbExemplar in warenkorb)
                            {
                                warenkorbExemplar.Benutzer = model.UserName;
                                bibContext.Warenkorb.Update(warenkorbExemplar);
                            }

                            var leihauftraege = bibContext.Leihauftrag.Where(e => e.Benutzer.Equals(user.UserName));
                            foreach (var leihauftrag in leihauftraege)
                            {
                                leihauftrag.Benutzer = model.UserName;
                                bibContext.Leihauftrag.Update(leihauftrag);
                            }

                            await bibContext.SaveChangesAsync();
                            var setNameResult = await userManager.SetUserNameAsync(user, model.UserName);
                            
                        }

                        // Fall: Die Email-Adresse wurde geändert
                        if (user.Email != model.Email)
                        {
                            await userManager.SetEmailAsync(user, model.Email);
                        }

                        // Rolle des Benutzers wird für die Datenbank aktualisiert
                        if (Role == "Admin")
                        {
                            var role = await userManager.GetRolesAsync(user);
                            await userManager.RemoveFromRolesAsync(user, role);
                            await userManager.AddToRoleAsync(user, "Admin");
                            user.Role = "Admin";
                            bibContext.Update(user);
                            bibContext.SaveChanges();
                            toastNotification.AddToastMessage("Benutzerdaten geändert", "Die Benutzerdaten von \"" + user.UserName + "\" wurden geändert.", ToastEnums.ToastType.Success, new ToastOption()
                            {
                                PositionClass = ToastPositions.TopCenter
                            });
                            return RedirectToAction(nameof(Index));
                        }
                        if (Role == "Member")
                        {
                            var role = await userManager.GetRolesAsync(user);
                            await userManager.RemoveFromRolesAsync(user, role);
                            await userManager.AddToRoleAsync(user, "Member");
                            user.Role = "Member";
                            bibContext.Update(user);
                            bibContext.SaveChanges();
                            toastNotification.AddToastMessage("Benutzerdaten geändert", "Der Benutzerdaten von \"" + user.UserName + "\" wurden geändert.", ToastEnums.ToastType.Success, new ToastOption()
                            {
                                PositionClass = ToastPositions.TopCenter
                            });
                            return RedirectToAction(nameof(Index));
                        }
                    }

                    return RedirectToAction(nameof(Index));

                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Benutzer/Delete
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(String id)
        {
            // Suche den ausgewählten Benutzer in der Datenbank anhand der ID
            var user = await bibContext.Benutzer
                .SingleOrDefaultAsync(m => m.Id == id);

            return View(user);
        }

        // POST: Benutzer/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(String id)
        {
            var user = await bibContext.Benutzer.SingleOrDefaultAsync(m => m.Id == id);
            var zurueckgeben = bibContext.Leihauftrag.Where(e => e.Benutzer.Equals(user.UserName) && e.IstVerliehen == true);
            var ausleihen = bibContext.Leihauftrag.Where(e => e.Benutzer.Equals(user.UserName) && e.IstVerliehen == false);
                        
            if (zurueckgeben.Count() != 0) // Fall: Benutzer hat noch Bücher ausgeliehen.
            {
                toastNotification.AddToastMessage("Benutzer hat noch Bücher ausgeliehen", "Der Benutzer \"" + user.UserName + "\" kann nicht gelöscht werden, da er noch Bücher ausgeliehen hat.", ToastEnums.ToastType.Error, new ToastOption()
                {
                    PositionClass = ToastPositions.TopCenter
                });
                return RedirectToAction(nameof(Index));
            }
            else if (ausleihen.Count() != 0) // Fall: Benutzer hat noch offene Leihaufträge.
            {
                toastNotification.AddToastMessage("Benutzer hat noch Leihaufträge versendet", "Der Benutzer \"" + user.UserName + "\" kann nicht gelöscht werden, da er noch Leihaufträge versendet hat.", ToastEnums.ToastType.Error, new ToastOption()
                {
                    PositionClass = ToastPositions.TopCenter
                });
                return RedirectToAction(nameof(Index));
            }
            else // Der Benutzer wird aus der Datenbank gelöscht.
            {
                bibContext.Benutzer.Remove(user);
                await bibContext.SaveChangesAsync();

                toastNotification.AddToastMessage("Benutzer gelöscht", "Das Benutzerkonto von \"" + user.UserName + "\" wurde gelöscht.", ToastEnums.ToastType.Success, new ToastOption()
                {
                    PositionClass = ToastPositions.TopCenter
                });

                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Benutzer/Login
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            // Der existierende externe Cookie wird gecleart um einen sauberen Login Prozess zu gewährleisten
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            return View();
        }

        // POST: Benutzer/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Prüft, ob die Logindaten richtig sind.
                var result = await signInManager.PasswordSignInAsync(model.Benutzername,
                    model.Passwort, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    toastNotification.AddToastMessage("Login fehlgeschlagen", "Benutzername oder Passwort falsch.", ToastEnums.ToastType.Error, new ToastOption()
                    {
                        PositionClass = ToastPositions.TopCenter
                    });

                    return View(model);
                }
            }

            // Wenn es bis hier hin kommt, ist etwas schief gelaufen und die Login Seite wird wieder angezeigt
            return View("Login");
        }

        // GET: Benutzer/ManageBenutzer
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ManageBenutzer()
        {
            var user = await userManager.GetUserAsync(User);

            var model = new ManageBenutzerModel
            {
                Benutzername = user.UserName,
                Email = user.Email
            };
            return View(model);
        }

        // POST: Benutzer/ManageBenutzer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageBenutzer( [Bind("Benutzername, Email")] ManageBenutzerModel model)
        {
            if (!ModelState.IsValid)
            {
                var benutzer = await userManager.GetUserAsync(User);
                model.Benutzername = benutzer.UserName;
                model.Email = benutzer.Email;

                return View(model);
            }

            var user = await userManager.GetUserAsync(User);
            var oldName = user.UserName;

            if (model.Benutzername == null) // Fall: Benutzername ist leer.
            {
                toastNotification.AddToastMessage("Kein Benutzername eingegeben", "Es muss ein Benutzername vergeben werden.", ToastEnums.ToastType.Error, new ToastOption()
                {
                    PositionClass = ToastPositions.TopCenter
                });

                return RedirectToAction(nameof(ManageBenutzer));
            }
            else if (model.Email == null) // Fall: Email ist leer.
            {
                toastNotification.AddToastMessage("Kein Email eingegeben", "Es muss eine Email-Adresse vergeben werden.", ToastEnums.ToastType.Error, new ToastOption()
                {
                    PositionClass = ToastPositions.TopCenter
                });

                return RedirectToAction(nameof(ManageBenutzer));
            }
            else
            {
                string message = "";

                // Fall: Der Benutzername wurde geändert
                if (model.Benutzername != oldName)
                {
                    var userExist = bibContext.Benutzer.SingleOrDefault(b => b.UserName == model.Benutzername);

                    // Prüft, ob der Benutzername bereits in der Datenbank vorhanden ist.
                    if (userExist != null)
                    {
                        toastNotification.AddToastMessage("Benutzername vergeben", "Der Benutzername \"" + model.Benutzername + "\" ist bereits vergeben.", ToastEnums.ToastType.Error, new ToastOption()
                        {
                            PositionClass = ToastPositions.TopCenter
                        });

                        return RedirectToAction(nameof(ManageBenutzer));
                    }

                    // Alle bisherigern Referenzen auf den Namen ändern
                    var warenkorb = bibContext.Warenkorb.Where(e => e.Benutzer.Equals(user.UserName));
                    foreach (var item in warenkorb)
                    {
                        item.Benutzer = model.Benutzername;
                        bibContext.Warenkorb.Update(item);
                    }

                    var leihauftraege = bibContext.Leihauftrag.Where(e => e.Benutzer.Equals(user.UserName));
                    foreach (var leihauftrag in leihauftraege)
                    {
                        leihauftrag.Benutzer = model.Benutzername;
                        bibContext.Leihauftrag.Update(leihauftrag);
                    }

                    await bibContext.SaveChangesAsync();

                    await userManager.SetUserNameAsync(user, model.Benutzername);
                    message += "Der Benutzername wurde in \"" + model.Benutzername + "\" geändert.";

                }

                var oldEmail = user.Email;

                // Fall: Email wurde gändert
                if (model.Email != oldEmail)
                {
                    await userManager.SetEmailAsync(user, model.Email);
                    message += " Die Email wurde in \"" + model.Email + "\" geändert.";
                }

                if (model.Benutzername != oldName || model.Email != oldEmail)
                {
                    toastNotification.AddToastMessage("Benutzerdaten geändert", message, ToastEnums.ToastType.Success, new ToastOption()
                    {
                        PositionClass = ToastPositions.TopCenter
                    });
                }
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: Benutzer/ChangePassword
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await userManager.GetUserAsync(User);
            var hasPassword = await userManager.HasPasswordAsync(user);
            var model = new ChangePasswordModel();
            return View(model);
        }

        // POST: Benutzer/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.GetUserAsync(User);
            var result = await userManager.CheckPasswordAsync(user, model.OldPassword);

            if (!result)
            {
                toastNotification.AddToastMessage("Passwort falsch", "Das aktuelle Passwort wurde falsch eingegeben.", ToastEnums.ToastType.Error, new ToastOption()
                {
                    PositionClass = ToastPositions.TopCenter
                });
            }

            // Passwort wird geändert
            var changePasswordResult = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                return View(model);
            }

            // Benutzer wird eingeloggt und auf die Hauptseite weitergeleitet
            await signInManager.SignInAsync(user, isPersistent: false);

            toastNotification.AddToastMessage("Passwort geändert", "Das Passwort wurde geändert.", ToastEnums.ToastType.Success, new ToastOption()
            {
                PositionClass = ToastPositions.TopCenter
            });

            return RedirectToAction("Index", "Home");
        }
        
        // GET: Benutzer/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Benutzer/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userCheck = await bibContext.Benutzer.SingleOrDefaultAsync(m => m.UserName == model.Benutzername);

                // Fall: Benutzername ist noch nicht vergeben
                if (userCheck == null)
                {
                    // Erstellt einen neuen Benutzer mit dem Usermanager und fügt eine Rolle hinzu.
                    var user = new Benutzer { UserName = model.Benutzername, Email = model.Email };
                    var result = await userManager.CreateAsync(user, model.Passwort);

                    // Prüft, ob beim Erstellen alles erfolgreich war und loggt den Benutzer ein
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "Member");
                        user.Role = "Member";
                        bibContext.Update(user);
                        await bibContext.SaveChangesAsync();
                        await signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", "Home");
                    }
                }
                else  // Fall: Benutzername ist bereits vergeben
                {
                    toastNotification.AddToastMessage("Registrierung fehlgeschlagen", "Der Benutzername \"" + model.Benutzername + "\" ist bereits vergeben.", ToastEnums.ToastType.Error, new ToastOption()
                    {
                        PositionClass = ToastPositions.TopCenter
                    });
                }
            }

            // Wenn es bis hier hin kommt, ist etwas schief gelaufen und die Seite wird erneut geladen
            return View(model);
        }

        // Logout
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        // Zugriff verweigert, wenn en Benutzer nicht die Rechte hat um eine bestimmte Seite zu sehen
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
