using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Henagone_WebGame.Controllers
{
    public class CGUController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}