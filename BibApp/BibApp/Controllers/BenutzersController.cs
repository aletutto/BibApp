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
    public class BenutzersController : Controller
    {
        private readonly BibContext bibContext;
        private readonly UserManager<Benutzer> userManager;
        private readonly SignInManager<Benutzer> signInManager;
        private readonly IToastNotification toastNotification;

        public BenutzersController(
            UserManager<Benutzer> userManager,
            SignInManager<Benutzer> signInManager,
            IToastNotification toastNotification,
            BibContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.toastNotification = toastNotification;
            this.bibContext = context;
        }

        // GET: Benutzers/Index
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            // Suchfeld Sortierungsdaten
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["RoleSortParm"] = sortOrder == "Role" ? "role_desc" : "Role";
            ViewData["EmailSortParm"] = sortOrder == "Email" ? "email_desc" : "Email";
            ViewData["CurrentFilter"] = searchString;

            var benutzer = from s in bibContext.Benutzers
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

        // GET: Benutzers/Details
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(String id)
        {
            // Suche das Benutzerkonto in der Datenbank mit der mitgegebenen ID.
            var usr = await bibContext.Benutzers
                .SingleOrDefaultAsync(m => m.Id == id);
            if (usr == null)
            {
                return NotFound();
            }

            return View(usr);
        }

        // GET: Benutzers/Edit
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(String id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Suche das Benutzerkonto in der Datenbank mit der mitgegebenen ID.
            var usr = await bibContext.Benutzers.SingleOrDefaultAsync(m => m.Id == id);
            if (usr == null)
            {
                return NotFound();
            }

            ViewData["role"] = usr.Role;

            return View(usr);
        }

        // POST: Benutzers/Edit
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

                    // Fall: Benutzername darf nicht leer sein.
                    if (model.UserName == null)
                    {
                        toastNotification.AddToastMessage("", "Der Benutzernamen darf nicht leer sein.", ToastEnums.ToastType.Error, new ToastOption()
                        {
                            PositionClass = ToastPositions.TopCenter
                        });

                        return RedirectToAction(nameof(Edit));
                    }
                    else
                    {
                        // Fall: Der neue Benutzername darf nicht identisch mit dem alten sein.
                        if (model.UserName != newName)
                        {
                            var userExist = bibContext.Benutzers.SingleOrDefault(b => b.UserName == model.UserName);

                            // Fall: Der neue Benutzername existiert bereits in der Datenbank.
                            if (userExist != null)
                            {
                                toastNotification.AddToastMessage("", "Der Benutzername \"" + model.UserName + "\" ist bereits vergeben.", ToastEnums.ToastType.Error, new ToastOption()
                                {
                                    PositionClass = ToastPositions.TopCenter
                                });

                                return RedirectToAction(nameof(Edit));
                            }

                            // Alle bisherigern Referenzen auf den neuen Namen mitändern
                            var warenkorb = bibContext.Warenkoerbe.Where(e => e.Benutzer.Equals(user.UserName));
                            foreach (var item in warenkorb)
                            {
                                item.Benutzer = model.UserName;
                                bibContext.Warenkoerbe.Update(item);
                            }

                            var adminKorb = bibContext.AdminWarenkoerbe.Where(e => e.Benutzer.Equals(user.UserName));
                            foreach (var item in adminKorb)
                            {
                                item.Benutzer = model.UserName;
                                bibContext.AdminWarenkoerbe.Update(item);
                            }

                            await bibContext.SaveChangesAsync();
                            var setNameResult = await userManager.SetUserNameAsync(user, model.UserName);
                            
                            if (!setNameResult.Succeeded)
                            {
                                throw new ApplicationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
                            }
                        }

                        if (user.Email != model.Email)
                        {
                            await userManager.SetEmailAsync(user, model.Email);
                        }

                        if (Role == "Admin")
                        {
                            await userManager.AddToRoleAsync(user, "Admin");
                            user.Role = "Admin";
                            bibContext.Update(user);
                            bibContext.SaveChanges();
                            toastNotification.AddToastMessage("", "Die Benutzerdaten von \"" + user.UserName + "\" wurden geändert.", ToastEnums.ToastType.Success, new ToastOption()
                            {
                                PositionClass = ToastPositions.TopCenter
                            });
                            return RedirectToAction(nameof(Index));
                        }
                        if (Role == "Member")
                        {
                            await userManager.AddToRoleAsync(user, "Member");
                            user.Role = "Member";
                            bibContext.Update(user);
                            bibContext.SaveChanges();
                            toastNotification.AddToastMessage("", "Der Benutzerdaten von \"" + user.UserName + "\" wurden geändert.", ToastEnums.ToastType.Success, new ToastOption()
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

        private bool BenutzerExists(String id)
        {
            return bibContext.Benutzers.Any(e => e.Id == id);
        }

        // GET: Benutzers/Delete
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(String id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Suche das Benutzerkonto in der Datenbank mit der mitgegebenen ID.
            var usr = await bibContext.Benutzers
                .SingleOrDefaultAsync(m => m.Id == id);
            if (usr == null)
            {
                return NotFound();
            }

            return View(usr);
        }

        // POST: Benutzers/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(String id)
        {
            // Suche das Benutzerkonto in der Datenbank mit der mitgegebenen ID.
            var usr = await bibContext.Benutzers.SingleOrDefaultAsync(m => m.Id == id);

            var adminKorbZurückgeben = bibContext.AdminWarenkoerbe.Where(e => e.Benutzer.Equals(usr.UserName) && e.IstVerliehen == true);
            var adminKorbAusleihen = bibContext.AdminWarenkoerbe.Where(e => e.Benutzer.Equals(usr.UserName) && e.IstVerliehen == false);

            // Fall: Benutzer hat noch Bücher ausgeliehen.
            if (adminKorbZurückgeben.Count() != 0)
            {

                toastNotification.AddToastMessage("", "Der Benutzer \"" + usr.UserName + "\" kann nicht gelöscht werden, da er noch Bücher ausgeliehen hat.", ToastEnums.ToastType.Error, new ToastOption()
                {
                    PositionClass = ToastPositions.TopCenter
                });
                return RedirectToAction(nameof(Index));
            }

            // Fall: Benutzer hat noch offene Leihaufträge.
            else if (adminKorbAusleihen.Count() != 0)
            {
                toastNotification.AddToastMessage("", "Der Benutzer \"" + usr.UserName + "\" kann nicht gelöscht werden, da er noch Leihaufträge versendet hat.", ToastEnums.ToastType.Error, new ToastOption()
                {
                    PositionClass = ToastPositions.TopCenter
                });
                return RedirectToAction(nameof(Index));
            }
            // Der Benutzer wird aus der Datenbank gelöscht.
            else
            {
                bibContext.Benutzers.Remove(usr);
                await bibContext.SaveChangesAsync();
                toastNotification.AddToastMessage("", "Der Benutzer von \"" + usr.UserName + "\" wurde gelöscht.", ToastEnums.ToastType.Success, new ToastOption()
                {
                    PositionClass = ToastPositions.TopCenter
                });
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Benutzers/Login
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Benutzers/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Prüft, ob die Logindaten richtig sind.
                var result = await signInManager.PasswordSignInAsync(model.Benutzername,
                    model.Passwort, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToLocal(returnUrl);
                }
                if (result.IsLockedOut)
                {
                    return View("Lockout");
                }
                else
                {
                    toastNotification.AddToastMessage("", "Benutzername oder Passwort falsch.", ToastEnums.ToastType.Error, new ToastOption()
                    {
                        PositionClass = ToastPositions.TopCenter
                    });
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View("Login");
        }

        // GET: Benutzers/ManageBenutzer
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ManageBenutzer()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            var model = new ManageBenutzerModel
            {
                Benutzername = user.UserName,
                Email = user.Email
            };
            return View(model);
        }

        // POST: Benutzers/ManageBenutzer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageBenutzer(ManageBenutzerModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.GetUserAsync(User);
            var newName = user.UserName;

            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            // Fall: Benutzernamefeld ist leer.
            if (model.Benutzername == null)
            {
                toastNotification.AddToastMessage("", "Der Benutzernamen darf nicht leer sein.", ToastEnums.ToastType.Error, new ToastOption()
                {
                    PositionClass = ToastPositions.TopCenter
                });

                return RedirectToAction(nameof(ManageBenutzer));
            }
            else
            {
                // Fall: Neuer Benutzername ist nicht identisch mit dem alten Benutzernamen 
                if (model.Benutzername != newName)
                {
                    var userExist = bibContext.Benutzers.SingleOrDefault(b => b.UserName == model.Benutzername);

                    // Prüft, ob der Benutzername schon bereits in der Datenbank vorhanden ist.
                    if (userExist != null)
                    {
                        toastNotification.AddToastMessage("", "Der Benutzername \"" + model.Benutzername + "\" ist bereits vergeben.", ToastEnums.ToastType.Error, new ToastOption()
                        {
                            PositionClass = ToastPositions.TopCenter
                        });

                        return RedirectToAction(nameof(ManageBenutzer));
                    }

                    // Alle bisherigern Referenzen auf den Namen mitändern
                    var warenkorb = bibContext.Warenkoerbe.Where(e => e.Benutzer.Equals(user.UserName));
                    foreach (var item in warenkorb)
                    {
                        item.Benutzer = model.Benutzername;
                        bibContext.Warenkoerbe.Update(item);
                    }

                    var adminKorb = bibContext.AdminWarenkoerbe.Where(e => e.Benutzer.Equals(user.UserName));
                    foreach (var item in adminKorb)
                    {
                        item.Benutzer = model.Benutzername;
                        bibContext.AdminWarenkoerbe.Update(item);
                    }

                    await bibContext.SaveChangesAsync();
                    var setNameResult = await userManager.SetUserNameAsync(user, model.Benutzername);

                    toastNotification.AddToastMessage("", "Der Benutzername wurde in \"" + model.Benutzername + "\" geändert.", ToastEnums.ToastType.Success, new ToastOption()
                    {
                        PositionClass = ToastPositions.TopCenter
                    });

                    if (!setNameResult.Succeeded)
                    {
                        throw new ApplicationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
                    }
                }
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: Benutzers/ChangePassword
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            var hasPassword = await userManager.HasPasswordAsync(user);
            var model = new ChangePasswordModel();
            return View(model);
        }

        // POST: Benutzers/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            // Passwort wird mit dem Usermanager geändert.
            var changePasswordResult = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                AddErrors(changePasswordResult);
                return View(model);
            }

            // Benutzer wird eingeloggt und auf die Hauptseite verwiesen.
            await signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        // GET: Benutzers/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Benutzers/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var userCheck = await bibContext.Benutzers.SingleOrDefaultAsync(m => m.UserName == model.Benutzername);

                // Prüft, ob der Benutzername schon vorhanden ist.
                if (userCheck == null)
                {
                    var user = new Benutzer { UserName = model.Benutzername, Email = model.Email };
                    // Erstellt einen neuen Benutzer mit dem Usermanager und fügt eine Rolle hinzu.
                    var result = await userManager.CreateAsync(user, model.Passwort);
                    await userManager.AddToRoleAsync(user, "Member");
                    user.Role = "Member";
                    bibContext.Update(user);
                    bibContext.SaveChanges();
                    // Prüft, ob beim Erstellen alles erfolgreich war.
                    if (result.Succeeded)
                    {

                        await signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                    AddErrors(result);
                }
                else
                {
                    toastNotification.AddToastMessage("Registrierung fehlgeschlagen", "Der Benutzername \"" + model.Benutzername + "\" ist bereits vergeben.", ToastEnums.ToastType.Error, new ToastOption()
                    {
                        PositionClass = ToastPositions.TopCenter
                    });
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // Logout
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

#region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

#endregion
    }
}
