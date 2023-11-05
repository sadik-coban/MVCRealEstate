using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVCRealEstate.Models;
using MVCRealEstateData;
using NETCore.MailKit.Core;
using System.Security.Claims;

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


        public IActionResult AccessDenied()
        {
            return View();
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
                var claims = new[]
                {
                    new Claim(ClaimTypes.GivenName, user.Name),
                };


                await userManager.AddToRoleAsync(user, "Members");
                await userManager.AddClaimsAsync(user, claims);

                var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                var url = Url.Action(nameof(ConfirmEmail), "Account", new { id = user.Id, token }, Request.Scheme);
                var body = string.Format(
                    System.IO.File.ReadAllText(Path.Combine(env.WebRootPath, "templates", "emailconfirmation.html")),
                    model.Name,
                    url);
                emailService.Send(model.UserName, "MVCRE E-Posta Doğrulama Mesajı", body, isHtml: true);
                return View("RegisterSuccess");
            }
            else
            {
                result.Errors.ToList().ForEach(e => ModelState.AddModelError("", e.Description));
                return View(model);
            }
        }

        
        public async Task<IActionResult> ConfirmEmail(Guid id, string token)
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            var result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                await signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
            else
                return View("EmailConfirmFailed");
        }

        public async Task<IActionResult> VerifyEmail(string userName)
        {
            var user = await userManager.FindByNameAsync(userName);
            return Json(user is not null ? $"{userName} zaten kullanımdadır" : "true");
        }

        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var user = await userManager.FindByNameAsync(model.UserName);
            if (user is null)
            {
                ModelState.AddModelError("", "Kullanıcı bulunamadı");
                return View(model);
            }
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var url = Url.Action(nameof(CreateNewPassword), "Account", new { id = user.Id, token }, Request.Scheme);
            var body = string.Format(
                System.IO.File.ReadAllText(Path.Combine(env.WebRootPath, "templates", "resetpassword.html")),
                user.Name,
                url);
            emailService.Send(model.UserName, "MVCRE Parola Yenileme Mesajı", body, isHtml: true);

            return View("ResetPasswordSuccess");
        }

        public IActionResult CreateNewPassword(Guid id, string token)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewPassword(CreateNewPasswordViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id.ToString());
            await userManager.ResetPasswordAsync(user!, model.Token, model.NewPassword);
            return RedirectToAction(nameof(Login));
        }
    }
}
