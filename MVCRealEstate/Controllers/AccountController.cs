using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVCRealEstate.Models;
using MVCRealEstateData;
using NETCore.MailKit.Core;

namespace MVCRealEstate.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;
        private readonly IEmailService emailService;
        private readonly IWebHostEnvironment env;

        public AccountController(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            IEmailService emailService,
            IWebHostEnvironment env
            )
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.emailService = emailService;
            this.env = env;
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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var user = await userManager.GetUserAsync(User);
            await userManager.ChangePasswordAsync(user!, model.CurrentPassword, model.NewPassword);
            return RedirectToAction("Index", "Home", new { area = "" });
        }


        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var user = new User
            {
                UserName = model.UserName,
                Name = model.Name,
                Email = model.UserName
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Members");
                var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                var url = Url.Action(nameof(ConfirmEmail), "Account", new { id = user.Id, token }, Request.Scheme);
                var body = string.Format(
                    System.IO.File.ReadAllText(Path.Combine(env.WebRootPath, "templates", "emailconfirmation.html")),
                    model.Name,
                    url);
                emailService.Send(model.UserName, "MVCRE E-Posta Doğrulama Mesajı", body, isHtml: true);
                return View("RegisterSuccess");
            }
            return View();
        }

        public async Task<IActionResult> ConfirmEmail(Guid id, string token)
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            var result = await userManager.ConfirmEmailAsync(user, token);
            
            return View();
        }
    }
}
