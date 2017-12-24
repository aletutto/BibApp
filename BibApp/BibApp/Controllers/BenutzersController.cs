using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using BibApp.Models.Benutzer;
using BibApp.Models;
using NToastNotify;

namespace BibApp.Controllers
{
    public class BenutzersController : Controller
    {
        private readonly BibContext _context;
        private readonly UserManager<Benutzer> _userManager;
        private readonly SignInManager<Benutzer> _signInManager;
        private readonly IToastNotification _toastNotification;

        public BenutzersController(
            UserManager<Benutzer> userManager,
            SignInManager<Benutzer> signInManager,
            IToastNotification toastNotification,
            BibContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _toastNotification = toastNotification;
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["RoleSortParm"] = sortOrder == "Role" ? "role_desc" : "Role";
            ViewData["CurrentFilter"] = searchString;

            var benutzer = from s in _context.Benutzers
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
                default:
                    benutzer = benutzer.OrderBy(s => s.UserName);
                    break;
            }
            return View(await benutzer.AsNoTracking().ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(String id)
        {
            var usr = await _context.Benutzers
                .SingleOrDefaultAsync(m => m.Id == id);
            if (usr == null)
            {
                return NotFound();
            }

            return View(usr);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(String id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usr = await _context.Benutzers.SingleOrDefaultAsync(m => m.Id == id);
            if (usr == null)
            {
                return NotFound();
            }

            ViewData["role"] = usr.Role;

            return View(usr);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(String id, [Bind("UserName")] Benutzer model, String Role)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(id);
                    var newName = user.UserName;
                    if (model.UserName != newName)
                    {
                        var setNameResult = await _userManager.SetUserNameAsync(user, model.UserName);
                        if (!setNameResult.Succeeded)
                        {
                            throw new ApplicationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
                        }
                    }

                    if (Role == "")
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    if (Role == "King")
                    {
                        await _userManager.AddToRoleAsync(user, "Admin");
                        user.Role = "Admin";
                        _context.Update(user);
                        _context.SaveChanges();
                        _toastNotification.AddToastMessage("", "Die Benutzerdaten von \"" + user.UserName + "\" wurde erfolgreich geändert!", ToastEnums.ToastType.Success, new ToastOption()
                        {
                            PositionClass = ToastPositions.TopCenter
                        });
                        return RedirectToAction(nameof(Index));
                    }
                    if (Role == "Pawn")
                    {
                        await _userManager.AddToRoleAsync(user, "Member");
                        user.Role = "Member";
                        _context.Update(user);
                        _context.SaveChanges();
                        _toastNotification.AddToastMessage("", "Der Benutzerdaten von \"" + user.UserName + "\" wurde erfolgreich geändert!", ToastEnums.ToastType.Success, new ToastOption()
                        {
                            PositionClass = ToastPositions.TopCenter
                        });
                        return RedirectToAction(nameof(Index));
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
            return _context.Benutzers.Any(e => e.Id == id);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(String id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usr = await _context.Benutzers
                .SingleOrDefaultAsync(m => m.Id == id);
            if (usr == null)
            {
                return NotFound();
            }

            return View(usr);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(String id)
        {
            var usr = await _context.Benutzers.SingleOrDefaultAsync(m => m.Id == id);
            _context.Benutzers.Remove(usr);
            await _context.SaveChangesAsync();
            _toastNotification.AddToastMessage("", "Der Benutzer von \"" + usr.UserName + "\" wurde erfolgreich gelöscht!", ToastEnums.ToastType.Success, new ToastOption()
            {
                PositionClass = ToastPositions.TopCenter
            });
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Benutzername,
                    model.Passwort, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _toastNotification.AddToastMessage("", "Login erfolgreich! Willkommen " + model.Benutzername + "!", ToastEnums.ToastType.Success, new ToastOption()
                    {
                        PositionClass = ToastPositions.TopCenter
                    });
                    return RedirectToLocal(returnUrl);
                }
                if (result.IsLockedOut)
                {
                    return View("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View("Login");
        }

        [HttpGet]
        public async Task<IActionResult> ManageBenutzer()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new ManageBenutzerModel
            {
                Benutzername = user.UserName,

            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageBenutzer(ManageBenutzerModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            var newName = user.UserName;
            if (model.Benutzername != newName)
            {
                var setNameResult = await _userManager.SetUserNameAsync(user, model.Benutzername);
                if (!setNameResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            var model = new ChangePasswordModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                AddErrors(changePasswordResult);
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var userCheck = await _context.Benutzers.SingleOrDefaultAsync(m => m.UserName == model.Benutzername);
                if (userCheck == null)
                {
                    var user = new Benutzer { UserName = model.Benutzername };
                    var result = await _userManager.CreateAsync(user, model.Password);
                    await _userManager.AddToRoleAsync(user, "Member");
                    user.Role = "Member";
                    _context.Update(user);
                    _context.SaveChanges();
                    if (result.Succeeded)
                    {

                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _toastNotification.AddToastMessage("", "Registrierung erfolgreich! Willkommen " + model.Benutzername + "!", ToastEnums.ToastType.Success, new ToastOption()
                        {
                            PositionClass = ToastPositions.TopCenter
                        });
                        return RedirectToLocal(returnUrl);
                    }
                    AddErrors(result);
                }
                else
                {
                    _toastNotification.AddToastMessage("", "Registrierung fehlgeschlagen!", ToastEnums.ToastType.Success, new ToastOption()
                    {
                        PositionClass = ToastPositions.TopCenter
                    });
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _toastNotification.AddToastMessage("", "Logout erfolgreich!", ToastEnums.ToastType.Success, new ToastOption()
            {
                PositionClass = ToastPositions.TopCenter
            });
            return View("Login");
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
