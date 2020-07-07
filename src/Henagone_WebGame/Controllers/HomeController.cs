using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using Henagone_WebGame.Models;
using Henagone_WebGame.Models.AdminViewModels;

namespace Henagone_WebGame.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            Conf item = null;
            using (var context = new SiteDbContext())
            {
                item = context.Conf
                    .Where(r => r.Id == 1)
                    .Select(r => r).First();
            }

            if(item.Value == "2")
                return RedirectToAction(nameof(HomeController.Maintenance), "Home");
            else
                return View();
        }

        public IActionResult Maintenance()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }
    }
}
