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
        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager)
        {
            _logger = logger;
            this._userManager = userManager;
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

        public IActionResult LogIn()
        {
            return View();
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
                    ModelState.AddModelError("",item.Description);
                }
            }

            return View(userViewModel);
        }
    }
}
