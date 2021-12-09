using IdentityExample.Models;
using IdentityExample.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel userLogin)
        {
            if (!ModelState.IsValid)
            {
                return View(userLogin);
            }

            AppUser user = await _userManager.FindByEmailAsync(userLogin.Email);

            if (user != null)
            {
                await _signInManager.SignOutAsync(); //Sistemde eski benim yazdığım bir cookie var ise o silinmesi için öncelikle bir çıkış yapıyorum.

                Microsoft.AspNetCore.Identity.SignInResult result =  await _signInManager.PasswordSignInAsync(user, userLogin.Password, false, false ); //isPersistent : Stattup'da verdiğim cookie tutma süremin geçerli olup olmamasını belirtiyorum.
                                                                                                                                                        // true set edersem eğer geçerli, false set edersem eğer browser kapanana kadar kullanıcı hatırlanır ve sonra tekrardan login olması gerekir. 
                                                                                                                                                        // Beni Hatırla butonuna eğer basılmış ise true set edilir. 
                                                                                                                                                        // lockoutOnFailure : Kullanıcı başarısız girişlerde kullanıcıyı kitleyip kitlememe özelliği ile ilgi parametredir. (Belirttiğimiz süre boyunca kullanıcının girişini kitleyebilir/ engelleyebiliriz.) 
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Member");
                }
            }
            else
            {
                //ModelState.AddModelError(nameof(LoginViewModel.Email), "Geçersiz email adresi veya şifresi"); //Sadece summary'de gösterilmesini değil de hatanın Email altında gözükmesini istiyorum. 
                ModelState.AddModelError("", "Geçersiz email adresi veya şifresi"); 
            }

            return View(userLogin);
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(UserViewModel userViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(userViewModel);
            }

            AppUser user = new()
            {
                UserName = userViewModel.UserName,
                Email = userViewModel.Email,
                PhoneNumber = userViewModel.PhoneNumber
            };

            IdentityResult result =  await _userManager.CreateAsync(user, userViewModel.Password); // Kayıt formunu dolduran kullanıcıyu kayıt etmek için CreateAsync metodunu kullanırız. 
                                                                                                   // İkinci parametre olarak kullanıcının şifresini veririz. Şifre alanı Identity aracılığı ile şifrelenerek veritabanına yazılır.

            if (result.Succeeded) 
            {
                return RedirectToAction("Login");

            }
            else
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("",item.Description); //Sadece bizim eklediğimiz hatalar var ise herhangi bir key belirtmediğimiz için View kısmında summary'de gözükür. 
                }
            }

            return View(userViewModel);
        }
    }
}
