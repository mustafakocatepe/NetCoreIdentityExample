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
        public IActionResult Login(string returnUrl)
        {
            TempData["ReturnUrl"] = returnUrl;
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
                if (await _userManager.IsLockedOutAsync(user)) //Kullanıcı girişi kilitli mi kontrolü yapıyoruz.
                {
                    ModelState.AddModelError("", "Hesabınız bir süreliğine  kilitlenmiştir. Lütfen daha sonra tekrar deneyiniz.");

                    return View(userLogin);
                }

                await _signInManager.SignOutAsync(); //Sistemde eski benim yazdığım bir cookie var ise o silinmesi için öncelikle bir çıkış yapıyorum.

                Microsoft.AspNetCore.Identity.SignInResult result =  await _signInManager.PasswordSignInAsync(user, userLogin.Password, userLogin.RememberMe, false ); // isPersistent : Stattup'da verdiğim cookie tutma süremin geçerli olup olmamasını belirtiyorum.
                                                                                                                                                                       // true set edersem eğer geçerli, false set edersem eğer browser kapanana kadar kullanıcı hatırlanır ve sonra tekrardan login olması gerekir. 
                                                                                                                                                                       // Beni Hatırla butonuna eğer basılmış ise true set edilir. 
                                                                                                                                                                       // userLogin.RememberMe ile checkbox tıklanmış mı tıklanmamış mı ona göre isPersistent set edildi. 
                if (result.Succeeded)
                {
                    await _userManager.ResetAccessFailedCountAsync(user); //Eğer kullanıcı başarılı bir giriş yaptı ise hatalı giriş değeri sıfırlanıyor.

                    if (TempData["ReturnUrl"] != null)
                    {
                        return Redirect(TempData["ReturnUrl"].ToString());
                    }
                    return RedirectToAction("Index", "Member");
                }
                else
                {
                    await _userManager.AccessFailedAsync(user);  //Eğer kullanıcı başarılı bir giriş yapamadıysa başarısız giriş sayısı(AccessFailedCount) arttırılıyor.

                    int fail =await _userManager.GetAccessFailedCountAsync(user); // Kullanıcının kaç kere başarısız giriş yaptığı bilgisi alınıyor. 

                    ModelState.AddModelError("", $"{fail} kez başarısız giriş." );

                    if (fail == 3)
                    {
                        await _userManager.SetLockoutEndDateAsync(user, new DateTimeOffset(System.DateTime.Now.AddMinutes(20))); //Kullanıcı eğer 3 kere başarısız giriş yaptı ise kullanıcıyı kilitliyorum.

                        ModelState.AddModelError("", "Hesabınız 3 başarısız girişten dolayı 20 dakika süre kilitlenmiştir. Lütfen daha sonra tekrar deneyiniz.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Email adresiniz veya şifreniz yanlış.");
                    }
                }
            }
            else
            {
                //ModelState.AddModelError(nameof(LoginViewModel.Email), "Geçersiz email adresi veya şifresi"); //Sadece summary'de gösterilmesini değil de hatanın Email altında gözükmesini istiyorum. 
                ModelState.AddModelError("", "Bu email adesine kayıtlı kullanıcı bulunamamıştır."); 
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
