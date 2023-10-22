using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVCRealEstate.Models;
using MVCRealEstateData;

namespace MVCRealEstate.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;

        public AccountController(
            SignInManager<User> signInManager,
            UserManager<User> userManager
            )
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        public IActionResult Login()
        {
            return View(new LoginViewModel { RememberMe = true });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, true);
            if (result.Succeeded)
            {
                return Redirect(model.ReturnUrl ?? "/");
            }
            else
            {
                if (result.IsLockedOut)
                    ModelState.AddModelError("", "Çok fazla hatalı deneme lütfen bekleyiniz!");
                if (result.IsNotAllowed)
                    ModelState.AddModelError("", "E-Posta doğrulanmadığı için giriş işlemini yapılamıyor");

                ModelState.AddModelError("", "Geçersiz kullanıcı girişi");
                return View(model);
            }
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var user = await userManager.GetUserAsync(User);
            await userManager.ChangePasswordAsync(user!, model.CurrentPassword, model.NewPassword);
            return RedirectToAction("Index", "Home", new { area = "" });
        }


    }
}
