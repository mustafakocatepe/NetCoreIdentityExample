using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityExample.Controllers
{
    [Authorize] //Sadece üyelerin girebilmesi için controller içerisinde ki bütün actionları kısıtladık. 
    public class MemberController : Controller
    {     
        public IActionResult Index()
        {
            return View();
        }
    }
}
