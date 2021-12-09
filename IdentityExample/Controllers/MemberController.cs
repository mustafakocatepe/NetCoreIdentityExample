using Microsoft.AspNetCore.Mvc;

namespace IdentityExample.Controllers
{
    public class MemberController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
