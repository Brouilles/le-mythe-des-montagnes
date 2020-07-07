using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Henagone_WebGame.Models.ShopViewModels;
using Henagone_WebGame.Models;
using Henagone_WebGame.Models.GameViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Security.Claims;
using Henagone_WebGame.Models.AdminViewModels;
using Microsoft.AspNetCore.Identity;

namespace Henagone_WebGame.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private IHostingEnvironment hostingEnv;
        public AdminController(UserManager<ApplicationUser> userManager, IHostingEnvironment environment)
        {
            _userManager = userManager;
            hostingEnv = environment;
        }

        // GET : GetModule
        public ActionResult GetModule(string partialName, string param1)
        {
            if (param1 == null)
                return PartialView("~/Views/Admin/" + partialName);
            else
                return PartialView("~/Views/Admin/" + partialName, param1);
        }

        /*************************************/
        /*              INDEX                */
        /*************************************/
        public IActionResult Index()
        {
            return View();
        }

        /*************************************/
        /*              GAME                 */
        /*************************************/
        // GET: Job
        public IActionResult Job(int? id)
        {
            if(id != null)  {
                using (var context = new SiteDbContext())
                {
                    Jobs job = context.Jobs
                    .Where(r => r.Id == id)
                    .Select(r => r).FirstOrDefault();

                    if (job != null)
                    {
                        ViewBag.Edit = "true";
                        return View(job);
                    }
                    else
                        return View();
                }
            }

            return View();
        }

        // POST: Job
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Job(Jobs model)
        {
            try
            {
                if (model.Id == null)
                {
                    using (var context = new SiteDbContext())
                    {
                        context.Add(model);
                        context.SaveChanges();
                    }
                    TempData["success"] = "Création avec succès du travail";
                }
                else
                {
                    using (var context = new SiteDbContext())
                    {
                        context.Update(model);
                        context.SaveChanges();
                    }
                    TempData["success"] = "Modification avec succès du travail";
                }

                return RedirectToAction(nameof(AdminController.Job), "Admin");
            }
            catch
            {
                ModelState.AddModelError("", "Erreur lors de la création du travail.");
                return View(model);
            }
        }

        // POST: AjaxDeleteJob
        public IActionResult AjaxDeleteJob(int id)
        {
            try
            {
                using (var context = new SiteDbContext())
                {
                    Jobs job = context.Jobs
                        .Where(r => r.Id == id)
                        .Select(r => r).FirstOrDefault();

                    var atWork = context.AtWork
                        .Where(r => r.JobId == id)
                        .Select(r => r).ToList();

                    foreach(var item in atWork)
                    {
                        context.Remove(item);
                    }

                    context.Remove(job);
                    context.SaveChanges();
                }

                Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                return Json(new { success = true });
            }
            catch
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return Json(new { success = true });
            }
        }

        // GET: Items
        public IActionResult Items(int? id)
        {
            if (id != null)
            {
                using (var context = new SiteDbContext())
                {
                    Items item = context.Items
                        .Where(r => r.Id == id)
                        .Select(r => r).FirstOrDefault();

                    if (item != null)
                    {
                        ViewBag.Edit = "true";
                        return View(item);
                    }
                }
            }

            Items model = new Items();
            model.EnergyAdd = 0;
            model.EnergyRequired = 0;

            model.StrengthAdd = 0;
            model.StrengthRequired = 0;

            model.DefenseAdd = 0;
            model.DefenseRequired = 0;

            model.Purchase = 0;
            model.PurchaseArtifact = 0;

            model.VitalityAdd = 0;
            model.InventorySpaceAdd = 0;
            model.Durability = 0;
            model.Scarcity = 0;
            model.Weight = 0;

            return View(model);
        }

        // POST: Items
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Items(Items model, IFormFile file, IFormFile file2)
        {
            try
            {
                if (model.Id == null)
                {
                    using (var context = new SiteDbContext())
                    {
                        context.Add(model);
                        context.SaveChanges();
                    }
                    TempData["success"] = "Création avec succès de l'objet";
                }
                else
                {
                    using (var context = new SiteDbContext())
                    {
                        context.Update(model);
                        context.SaveChanges();
                    }
                    TempData["success"] = "Modification avec succès de l'objet";
                }

                //Upload image
                if (file != null && file.Length > 0)
                {
                    var parsedContentDisposition = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
                    string FilePath = parsedContentDisposition.FileName.Trim('"');
                    string FileExtension = Path.GetExtension(FilePath);
                    var uploadDir = hostingEnv.WebRootPath + $@"\uploads\game\cards\items\";

                    if (model.Id != null)
                        System.IO.File.Delete(uploadDir + model.Id + ".fr.svg");

                    using (FileStream fs = System.IO.File.Create(uploadDir + model.Id + ".fr" + FileExtension))
                    {
                        file.CopyTo(fs);
                        fs.Flush();
                    }
                }

                if (file2 != null && file2.Length > 0)
                {
                    var parsedContentDisposition = ContentDispositionHeaderValue.Parse(file2.ContentDisposition);
                    string FilePath = parsedContentDisposition.FileName.Trim('"');
                    string FileExtension = Path.GetExtension(FilePath);
                    var uploadDir = hostingEnv.WebRootPath + $@"\uploads\game\cards\items\";

                    if (model.Id != null)
                        System.IO.File.Delete(uploadDir + model.Id + ".en.svg");

                    using (FileStream fs = System.IO.File.Create(uploadDir + model.Id + ".en" + FileExtension))
                    {
                        file2.CopyTo(fs);
                        fs.Flush();
                    }
                }

                return RedirectToAction(nameof(AdminController.Items), "Admin");
            }
            catch
            {
                ModelState.AddModelError("", "Erreur lors de la création de l'objet.");
                return View(model);
            }
        }

        // POST: AjaxDeleteItem
        public IActionResult AjaxDeleteItem(int id)
        {
            try
            {
                using (var context = new SiteDbContext())
                {
                    Items item = context.Items
                        .Where(r => r.Id == id)
                        .Select(r => r).FirstOrDefault();

                    var inventory = context.Inventory
                        .Where(r => r.ItemId == id)
                        .Select(r => r).ToList();

                    foreach(var itemFor in inventory)
                    {
                        context.Remove(itemFor);
                    }

                    var uploadDir = hostingEnv.WebRootPath + $@"\uploads\game\cards\items\";
                    System.IO.File.Delete(uploadDir + item.Id + ".fr.svg");
                    System.IO.File.Delete(uploadDir + item.Id + ".en.svg");

                    context.Remove(item);
                    context.SaveChanges();
                }

                Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                return Json(new { success = true });
            }
            catch
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return Json(new { success = true });
            }
        }

        /*************************************/
        /*           Companions              */
        /*************************************/
        // GET: Companions
        public IActionResult Companions(int? id)
        {
            if (id != null)
            {
                using (var context = new SiteDbContext())
                {
                    Companions item = context.Companions
                    .Where(r => r.Id == id)
                    .Select(r => r).FirstOrDefault();

                    if (item != null)
                    {
                        ViewBag.Edit = "true";
                        return View(item);
                    }
                }
            }

            Companions model = new Companions();
            model.Purchase = 0;
            model.Scarcity = 0;

            return View(model);
        }

        // POST: Companions
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Companions(Companions model, IFormFile file, IFormFile file2)
        {
            try
            {
                if (model.Id == null)
                {
                    using (var context = new SiteDbContext())
                    {
                        context.Add(model);
                        context.SaveChanges();
                    }
                    TempData["success"] = "Création avec succès de l'objet";
                }
                else
                {
                    using (var context = new SiteDbContext())
                    {
                        context.Update(model);
                        context.SaveChanges();
                    }
                    TempData["success"] = "Modification avec succès de l'objet";
                }

                //Upload image
                if (file != null && file.Length > 0)
                {
                    var parsedContentDisposition = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
                    string FilePath = parsedContentDisposition.FileName.Trim('"');
                    string FileExtension = Path.GetExtension(FilePath);
                    var uploadDir = hostingEnv.WebRootPath + $@"\uploads\game\cards\companions\";

                    if (model.Id != null)
                        System.IO.File.Delete(uploadDir + model.Id + ".fr.svg");

                    using (FileStream fs = System.IO.File.Create(uploadDir + model.Id + ".fr" + FileExtension))
                    {
                        file.CopyTo(fs);
                        fs.Flush();
                    }
                }

                //Upload image
                if (file2 != null && file2.Length > 0)
                {
                    var parsedContentDisposition = ContentDispositionHeaderValue.Parse(file2.ContentDisposition);
                    string FilePath = parsedContentDisposition.FileName.Trim('"');
                    string FileExtension = Path.GetExtension(FilePath);
                    var uploadDir = hostingEnv.WebRootPath + $@"\uploads\game\cards\companions\";

                    if (model.Id != null)
                        System.IO.File.Delete(uploadDir + model.Id + ".en.svg");

                    using (FileStream fs = System.IO.File.Create(uploadDir + model.Id + ".en" + FileExtension))
                    {
                        file2.CopyTo(fs);
                        fs.Flush();
                    }
                }

                return RedirectToAction(nameof(AdminController.Companions), "Admin");
            }
            catch
            {
                ModelState.AddModelError("", "Erreur lors de la création de l'objet.");
                return View(model);
            }
        }

        // POST: AjaxDeleteCompanions
        public IActionResult AjaxDeleteCompanions(int id)
        {
            try
            {
                using (var context = new SiteDbContext())
                {
                    Companions item = context.Companions
                    .Where(r => r.Id == id)
                    .Select(r => r).FirstOrDefault();

                    var companions = context.Teams
                        .Where(r => r.CompanionId == id)
                        .Select(r => r).ToList();
                    foreach(var itemFor in companions) { context.Remove(itemFor); }

                    var uploadDir = hostingEnv.WebRootPath + $@"\uploads\game\cards\companions\";
                    System.IO.File.Delete(uploadDir + item.Id + ".fr.svg");
                    System.IO.File.Delete(uploadDir + item.Id + ".en.svg");

                    context.Remove(item);
                    context.SaveChanges();
                }

                Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                return Json(new { success = true });
            }
            catch
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return Json(new { success = true });
            }
        }

        // GET: CompanionQuests
        public IActionResult CompanionQuests(int? id)
        {
            if (id != null)
            {
                using (var context = new SiteDbContext())
                {
                    CompanionQuests item = context.CompanionQuests
                    .Where(r => r.Id == id)
                    .Select(r => r).FirstOrDefault();

                    if (item != null)
                    {
                        ViewBag.Edit = "true";
                        return View(item);
                    }
                }
            }

            CompanionQuests model = new CompanionQuests();
            model.Success = 100;
            model.Time = 1;
            model.StrengthAdd = 0;
            model.EnergyAdd = 0;
            model.DefenseAdd = 0;
            model.Gold = 0;
            model.Xp = 0;

            return View(model);
        }

        // POST: CompanionQuests
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CompanionQuests(CompanionQuests model)
        {
            try
            {
                if (model.Id == null)
                {
                    using (var context = new SiteDbContext())
                    {
                        context.Add(model);
                        context.SaveChanges();
                    }
                    TempData["success"] = "Création avec succès de la quête";
                }
                else
                {
                    using (var context = new SiteDbContext())
                    {
                        context.Update(model);
                        context.SaveChanges();
                    }
                    TempData["success"] = "Modification avec succès de la quête";
                }

                return RedirectToAction(nameof(AdminController.CompanionQuests), "Admin");
            }
            catch
            {
                ModelState.AddModelError("", "Erreur lors de la création de l'objet.");
                return View(model);
            }
        }

        // POST: AjaxDeleteCompanionQuests
        public IActionResult AjaxDeleteCompanionQuests(int id)
        {
            try
            {
                using (var context = new SiteDbContext())
                {
                    CompanionQuests item = context.CompanionQuests
                        .Where(r => r.Id == id)
                        .Select(r => r).FirstOrDefault();

                    var companionsAtQuest = context.OnQuest
                        .Where(r => r.QuestId == id)
                        .Select(r => r).ToList();
                    foreach (var itemFor in companionsAtQuest) { context.Remove(itemFor); }

                    context.Remove(item);
                    context.SaveChanges();
                }

                Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                return Json(new { success = true });
            }
            catch
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return Json(new { success = true });
            }
        }

        /*************************************/
        /*            APOTHECARY             */
        /*************************************/
        // GET: Potions
        public IActionResult Potions(int? id)
        {
            if (id != null)
            {
                using (var context = new SiteDbContext())
                {
                    Potions item = context.Potions
                    .Where(r => r.Id == id)
                    .Select(r => r).FirstOrDefault();

                    if (item != null)
                    {
                        ViewBag.Edit = "true";
                        return View(item);
                    }
                }
            }

            Potions model = new Potions();
            model.VitalityAdd = 0;
            model.DefenseAdd = 0;
            model.EnergyAdd = 0;
            model.StrengthAdd = 0;
            model.Time = 1;

            return View(model);
        }

        // POST: Potions
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Potions(Potions model)
        {
            try
            {
                if (model.Id == null)
                {
                    using (var context = new SiteDbContext())
                    {
                        context.Add(model);
                        context.SaveChanges();
                    }
                    TempData["success"] = "Création avec succès de la potion";
                }
                else
                {
                    using (var context = new SiteDbContext())
                    {
                        context.Update(model);
                        context.SaveChanges();
                    }
                    TempData["success"] = "Modification avec succès de la potion";
                }

                return RedirectToAction(nameof(AdminController.Potions), "Admin");
            }
            catch
            {
                ModelState.AddModelError("", "Erreur lors de la création de la potion.");
                return View(model);
            }
        }

        // POST: AjaxDeletePotions
        public IActionResult AjaxDeletePotions(int id)
        {
            try
            {
                using (var context = new SiteDbContext())
                {
                    Potions item = context.Potions
                    .Where(r => r.Id == id)
                    .Select(r => r).FirstOrDefault();

                    var buff = context.Buff
                        .Where(r => r.PotionId == id)
                        .Select(r => r).ToList();
                    foreach (var itemFor in buff) { context.Remove(itemFor); }

                    context.Remove(item);
                    context.SaveChanges();
                }

                Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                return Json(new { success = true });
            }
            catch
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return Json(new { success = true });
            }
        }

        /*************************************/
        /*              SHOP                 */
        /*************************************/
        // GET: ShopPrices
        public IActionResult ShopPrices()
        {
            return View();
        }

        // POST: ShopPrices
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ShopPrices(ArtifactsPrices model)
        {
            try
            {
                if (model.Price != 0) {
                    using (var context = new SiteDbContext())
                    {
                        context.Add(model);
                        context.SaveChanges();
                    }
                }
                else {
                    throw new ArgumentNullException("value");
                }

                TempData["success"] = "Création avec succès du tarif";
                return RedirectToAction(nameof(AdminController.ShopPrices), "Admin");
            }
            catch
            {
                ModelState.AddModelError("", "Erreur lors de la création du tarif.");
                return View(model);
            }
        }

        // POST: AjaxDeletePrice
        public IActionResult AjaxDeletePrice(int id)
        {
            try
            {
                using (var context = new SiteDbContext())
                {
                    ArtifactsPrices price = context.ArtifactsPrices
                    .Where(r => r.Id == id)
                    .Select(r => r).FirstOrDefault();

                    context.Remove(price);
                    context.SaveChanges();
                }

                Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                return Json(new { success = true });
            }
            catch
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return Json(new { success = true });
            }
        }

        // GET: PostGift
        public IActionResult Gift()
        {
            return View();
        }

        // POST: Gift
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Gift(Gifts model)
        {
            try
            {
                model.Code = Guid.NewGuid().ToString();
                model.Active = true;
                using (var context = new SiteDbContext())
                {
                    context.Add(model);
                    context.SaveChanges();
                }

                TempData["success"] = "Votre code : " + model.Code;
                return RedirectToAction(nameof(AdminController.Gift), "Admin");
            }
            catch
            {
                ModelState.AddModelError("", "Erreur lors de la création du code cadeau.");
                return View(model);
            }
        }

        // POST: AjaxDeleteGift
        public IActionResult AjaxDeleteGift(int id)
        {
            try
            {
                using (var context = new SiteDbContext())
                {
                    Gifts gift = context.Gifts
                    .Where(r => r.Id == id)
                    .Select(r => r).FirstOrDefault();

                    context.Remove(gift);
                    context.SaveChanges();
                }

                Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                return Json(new { success = true });
            }
            catch
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return Json(new { success = true });
            }
        }

        // GET: ShopStatistics
        public IActionResult ShopStatistics()
        {
            using (var context = new SiteDbContext())
            {
                var deals = context.Deals_statistics
                    .OrderByDescending(r => r.Date)
                    .Select(r => r).ToList();

                return View(deals);
            }
        }

        /*************************************/
        /*           MONSTER TYPE            */
        /*************************************/
        // GET: Monsters
        public IActionResult MonstersType(int? id)
        {
            if (id != null)
            {
                using (var context = new SiteDbContext())
                {
                    MonstersType item = context.MonstersType
                    .Where(r => r.Id == id)
                    .Select(r => r).FirstOrDefault();

                    if (item != null)
                    {
                        ViewBag.Edit = "true";
                        return View(item);
                    }
                    else
                        return View();
                }
            }

            return View();
        }

        // POST: monsterstype
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MonstersType(MonstersType model, IFormFile file, IFormFile file2)
        {
            try
            {
                if (model.Id == null)
                {
                    using (var context = new SiteDbContext())
                    {
                        context.Add(model);
                        context.SaveChanges();
                    }
                    TempData["success"] = "Création avec succès du type de monstre";
                }
                else
                {
                    using (var context = new SiteDbContext())
                    {
                        context.Update(model);
                        context.SaveChanges();
                    }
                    TempData["success"] = "Modification avec succès du type de monstre";
                }

                //Upload image
                if (file != null && file.Length > 0)
                {
                    var parsedContentDisposition = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
                    string FilePath = parsedContentDisposition.FileName.Trim('"');
                    string FileExtension = Path.GetExtension(FilePath);
                    var uploadDir = hostingEnv.WebRootPath + $@"\uploads\game\cards\monsterstype\";

                    if (model.Id != null)
                        System.IO.File.Delete(uploadDir + model.Id + ".fr.svg");

                    using (FileStream fs = System.IO.File.Create(uploadDir + model.Id + ".fr" + FileExtension))
                    {
                        file.CopyTo(fs);
                        fs.Flush();
                    }
                }

                //Upload image
                if (file2 != null && file2.Length > 0)
                {
                    var parsedContentDisposition = ContentDispositionHeaderValue.Parse(file2.ContentDisposition);
                    string FilePath = parsedContentDisposition.FileName.Trim('"');
                    string FileExtension = Path.GetExtension(FilePath);
                    var uploadDir = hostingEnv.WebRootPath + $@"\uploads\game\cards\monsterstype\";

                    if (model.Id != null)
                        System.IO.File.Delete(uploadDir + model.Id + ".en.svg");

                    using (FileStream fs = System.IO.File.Create(uploadDir + model.Id + ".en" + FileExtension))
                    {
                        file2.CopyTo(fs);
                        fs.Flush();
                    }
                }

                return RedirectToAction(nameof(AdminController.MonstersType), "Admin");
            }
            catch
            {
                ModelState.AddModelError("", "Erreur lors de la création de l'objet.");
                return View(model);
            }
        }

        // POST: AjaxDeleteCompanions
        public IActionResult AjaxDeleteMonstersType(int id)
        {
            try
            {
                using (var context = new SiteDbContext())
                {
                    MonstersType item = context.MonstersType
                        .Where(r => r.Id == id)
                        .Select(r => r).FirstOrDefault();

                    var uploadDir = hostingEnv.WebRootPath + $@"\uploads\game\cards\monsterstype\";
                    System.IO.File.Delete(uploadDir + item.Id + ".fr.svg");
                    System.IO.File.Delete(uploadDir + item.Id + ".en.svg");

                    context.Remove(item);
                    context.SaveChanges();
                }

                Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                return Json(new { success = true });
            }
            catch
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return Json(new { success = true });
            }
        }


        /*************************************/
        /*             MONSTER               */
        /*************************************/
        // GET: Monsters
        public IActionResult Monsters(int? id)
        {
            if (id != null)
            {
                using (var context = new SiteDbContext())
                {
                    Monsters item = context.Monsters
                    .Where(r => r.Id == id)
                    .Select(r => r).FirstOrDefault();

                    if (item != null)
                    {
                        ViewBag.Edit = "true";
                        return View(item);
                    }
                }
            }

            Monsters model = new Monsters();
            model.Life = 100;
            model.Defense = 1;
            model.Strength = 1;
            model.Energy = 1;
            model.Scarcity = 0;

            return View(model);
        }

        // POST: Monsters
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Monsters(Monsters model, IFormFile file, IFormFile file2)
        {
            try
            {
                if (model.Id == null)
                {
                    using (var context = new SiteDbContext())
                    {
                        context.Add(model);
                        context.SaveChanges();
                    }
                    TempData["success"] = "Création avec succès du monstre";
                }
                else
                {
                    using (var context = new SiteDbContext())
                    {
                        context.Update(model);
                        context.SaveChanges();
                    }
                    TempData["success"] = "Modification avec succès du monstre";
                }

                //Upload image
                if (file != null && file.Length > 0)
                {
                    var parsedContentDisposition = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
                    string FilePath = parsedContentDisposition.FileName.Trim('"');
                    string FileExtension = Path.GetExtension(FilePath);
                    var uploadDir = hostingEnv.WebRootPath + $@"\uploads\game\cards\monsters\";

                    if (model.Id != null)
                        System.IO.File.Delete(uploadDir + model.Id + ".fr.svg");

                    using (FileStream fs = System.IO.File.Create(uploadDir + model.Id + ".fr" + FileExtension))
                    {
                        file.CopyTo(fs);
                        fs.Flush();
                    }
                }

                //Upload image
                if (file2 != null && file2.Length > 0)
                {
                    var parsedContentDisposition = ContentDispositionHeaderValue.Parse(file2.ContentDisposition);
                    string FilePath = parsedContentDisposition.FileName.Trim('"');
                    string FileExtension = Path.GetExtension(FilePath);
                    var uploadDir = hostingEnv.WebRootPath + $@"\uploads\game\cards\monsters\";

                    if (model.Id != null)
                        System.IO.File.Delete(uploadDir + model.Id + ".en.svg");

                    using (FileStream fs = System.IO.File.Create(uploadDir + model.Id + ".en" + FileExtension))
                    {
                        file2.CopyTo(fs);
                        fs.Flush();
                    }
                }

                return RedirectToAction(nameof(AdminController.Monsters), "Admin");
            }
            catch
            {
                ModelState.AddModelError("", "Erreur lors de la création de l'objet.");
                return View(model);
            }
        }

        // POST: AjaxDeleteCompanions
        public IActionResult AjaxDeleteMonsters(int id)
        {
            try
            {
                using (var context = new SiteDbContext())
                {
                    Monsters item = context.Monsters
                    .Where(r => r.Id == id)
                    .Select(r => r).FirstOrDefault();

                    var uploadDir = hostingEnv.WebRootPath + $@"\uploads\game\cards\monsters\";
                    System.IO.File.Delete(uploadDir + item.Id + ".fr.svg");
                    System.IO.File.Delete(uploadDir + item.Id + ".en.svg");

                    context.Remove(item);
                    context.SaveChanges();
                }

                Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                return Json(new { success = true });
            }
            catch
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return Json(new { success = true });
            }
        }

        /*************************************/
        /*              HERO                 */
        /*************************************/
        // GET: HeroQuests
        public IActionResult HeroQuests(int? id)
        {
            if (id != null)
            {
                using (var context = new SiteDbContext())
                {
                    HeroQuests item = context.HeroQuests
                    .Where(r => r.Id == id)
                    .Select(r => r).FirstOrDefault();

                    if (item != null)
                    {
                        ViewBag.Edit = "true";
                        return View(item);
                    }
                    else
                        return View();
                }
            }

            return View();
        }

        // POST: HeroQuests
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult HeroQuests(HeroQuests model)
        {
            try
            {
                if (model.MonsterId == 0)
                    model.MonsterId = null;

                if (model.Id == null)
                {
                    using (var context = new SiteDbContext())
                    {
                        context.Add(model);
                        context.SaveChanges();
                    }
                    TempData["success"] = "Création avec succès de la quête";
                }
                else
                {
                    using (var context = new SiteDbContext())
                    {
                        context.Update(model);
                        context.SaveChanges();
                    }
                    TempData["success"] = "Modification avec succès de l'objet";
                }

                return RedirectToAction(nameof(AdminController.HeroQuests), "Admin");
            }
            catch
            {
                ModelState.AddModelError("", "Erreur lors de la création de l'objet.");
                return View(model);
            }
        }

        /*************************************/
        /*        NarrativeQuestStep         */
        /*************************************/
        // GET: NarrativeQuestStep
        public IActionResult NarrativeQuestStep(int? id)
        {
            if (id != null)
            {
                using (var context = new SiteDbContext())
                {
                    NarrativeQuestStep item = context.NarrativeQuestStep
                    .Where(r => r.Id == id)
                    .Select(r => r).FirstOrDefault();

                    if (item != null)
                    {
                        ViewBag.Edit = "true";
                        return View(item);
                    }
                    else
                        return View();
                }
            }

            return View();
        }

        // POST: NarrativeQuestStep
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NarrativeQuestStep(NarrativeQuestStep model, IFormFile file)
        {
            try
            {
                if (model.Id == null)
                {
                    using (var context = new SiteDbContext())
                    {
                        context.Add(model);
                        context.SaveChanges();
                    }
                    TempData["success"] = "Création avec succès de l'étape de quête";
                }
                else
                {
                    using (var context = new SiteDbContext())
                    {
                        context.Update(model);
                        context.SaveChanges();
                    }
                    TempData["success"] = "Modification avec succès de l'étape de quête";
                }

                //Upload image
                if (file != null && file.Length > 0)
                {
                    var parsedContentDisposition = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
                    string FilePath = parsedContentDisposition.FileName.Trim('"');
                    string FileExtension = Path.GetExtension(FilePath);
                    var uploadDir = hostingEnv.WebRootPath + $@"\uploads\game\quests\landscape\";

                    if (model.Id != null)
                        System.IO.File.Delete(uploadDir + model.Id + ".svg");

                    using (FileStream fs = System.IO.File.Create(uploadDir + model.Id + FileExtension))
                    {
                        file.CopyTo(fs);
                        fs.Flush();
                    }
                }

                return RedirectToAction(nameof(AdminController.NarrativeQuestStep), "Admin");
            }
            catch
            {
                ModelState.AddModelError("", "Erreur lors de la création de l'objet.");
                return View(model);
            }
        }

        // POST: AjaxDeleteNarrativeQuestStep
        public IActionResult AjaxDeleteNarrativeQuestStep(int id)
        {
            try
            {
                using (var context = new SiteDbContext())
                {
                    NarrativeQuestStep item = context.NarrativeQuestStep
                    .Where(r => r.Id == id)
                    .Select(r => r).FirstOrDefault();

                    var uploadDir = hostingEnv.WebRootPath + $@"\uploads\game\quests\landscape\";
                    System.IO.File.Delete(uploadDir + item.Id + ".svg");

                    context.Remove(item);
                    context.SaveChanges();
                }

                Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                return Json(new { success = true });
            }
            catch
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return Json(new { success = true });
            }
        }

        /*************************************/
        /*          NarrativeQuest           */
        /*************************************/
        // GET: NarrativeQuest
        public IActionResult NarrativeQuest(int? id)
        {
            if (id != null)
            {
                using (var context = new SiteDbContext())
                {
                    NarrativeQuest item = context.NarrativeQuest
                    .Where(r => r.Id == id)
                    .Select(r => r).FirstOrDefault();

                    if (item != null)
                    {
                        ViewBag.Edit = "true";
                        return View(item);
                    }
                    else
                        return View();
                }
            }

            return View();
        }

        // POST: NarrativeQuest
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NarrativeQuest(NarrativeQuest model)
        {
            try
            {
                if (model.Id == null)
                {
                    using (var context = new SiteDbContext())
                    {
                        context.Add(model);
                        context.SaveChanges();
                    }
                    TempData["success"] = "Création avec succès de la quête";
                }
                else
                {
                    using (var context = new SiteDbContext())
                    {
                        context.Update(model);
                        context.SaveChanges();
                    }
                    TempData["success"] = "Modification avec succès de la quête";
                }

                return RedirectToAction(nameof(AdminController.NarrativeQuest), "Admin");
            }
            catch
            {
                ModelState.AddModelError("", "Erreur lors de la création de l'objet.");
                return View(model);
            }
        }

        // POST: AjaxDeleteNarrativeQuest
        public IActionResult AjaxDeleteNarrativeQuest(int id)
        {
            try
            {
                using (var context = new SiteDbContext())
                {
                    NarrativeQuest item = context.NarrativeQuest
                    .Where(r => r.Id == id)
                    .Select(r => r).FirstOrDefault();

                    context.Remove(item);
                    context.SaveChanges();
                }

                Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                return Json(new { success = true });
            }
            catch
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return Json(new { success = true });
            }
        }

        /*************************************/
        /*            STATISTICS             */
        /*************************************/
        public ActionResult Characters()
        {
            using (var context = new SiteDbContext())
            {
                var model = context.Characters
                    .Select(r => r).ToList();

                return View(model);
            }
        }

        public ActionResult Members()
        {
            using (var context = new SiteDbContext())
            {
                string totalArrayString = string.Empty;
                int total = 0;

                // Last 7 days
                var lastDays = _userManager.Users
                    .Where(r => r.Register >= DateTime.Now.AddDays(-7))
                    .GroupBy(r => r.Register.Day).Select(r => r.Count()).ToArray();

                foreach (var item in lastDays) {
                    total += item;
                }
                ViewData["TotalLastDays"] = total;

                // Current month
                var months = _userManager.Users
                    .Where(r => r.Register.Month >= DateTime.Now.Month - 1)
                    .GroupBy(r => r.Register.Month).Select(r => r.Count()).ToList();

                if (months.Count == 2)
                {
                    ViewData["totalRegisterThisMonth"] = months[1];

                    if (months[0] < months[1])
                        ViewData["totalRegisterThisMonthChevron"] = "indicator-positive mdi mdi-chevron-up";
                    else if (months[0] > months[1])
                        ViewData["totalRegisterThisMonthChevron"] = "indicator-negative mdi mdi-chevron-down";
                    else
                        ViewData["totalRegisterThisMonthChevron"] = "indicator - equal mdi mdi-chevron - right";
                }
                else
                {
                    ViewData["totalRegisterThisMonth"] = 0;
                    ViewData["totalRegisterThisMonthChevron"] = "indicator - equal mdi mdi-chevron - right";
                }

                if (months.Count == 1)
                    ViewData["totalRegisterLastMonth"] = months[0];
                else
                    ViewData["totalRegisterLastMonth"] = 0;

                // Total register
                var totalRegisterByMonth = _userManager.Users.GroupBy(r => r.Register.Month).Select(r => r.Count()).ToList();
                totalArrayString = string.Empty;
                total = 0;

                foreach (var item in totalRegisterByMonth) {
                    total += item;
                    totalArrayString += item + ",";
                }
                ViewData["Total"] = total;

                if (total == 0)
                    ViewData["TotalArray"] = "0";
                else
                    ViewData["TotalArray"] = totalArrayString.Remove(totalArrayString.Length - 1);

                // User List
                var userList = _userManager.Users.Select(r => r).ToList();
                return View(userList);
            }
        }

        //
        // POST: /Manage/RemoveDefinitelyLogin
        public async Task<IActionResult> RemoveDefinitelyLogin(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    AjaxDeleteCharacter(id);
                    return RedirectToAction(nameof(AdminController.Members), "Admin");
                }
            }

            return RedirectToAction(nameof(AdminController.Members), "Admin");
        }

        // POST: AjaxDeleteCharacter
        public IActionResult AjaxDeleteCharacter(string id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                using (var context = new SiteDbContext())
                {
                    Characters character = context.Characters
                        .Where(r => r.UserId == id)
                        .Select(r => r).FirstOrDefault();

                    var inventory = context.Inventory
                        .Where(r => r.UserId == id)
                        .Select(r => r).ToList();

                    foreach (var item in inventory)
                    {
                        context.Remove(item);
                    }

                    var atBattle = context.AtBattlePVE
                        .Where(r => r.UserId == id)
                        .Select(r => r).FirstOrDefault();

                    if (atBattle != null)
                    {
                        context.Remove(atBattle);
                    }

                    var atWork = context.AtWork
                        .Where(r => r.UserId == id)
                        .Select(r => r).FirstOrDefault();

                    if (atWork != null)
                    {
                        context.Remove(atWork);
                    }

                    var companions = context.Teams
                        .Where(r => r.UserId == id)
                        .Select(r => r).ToList();

                    foreach (var item in companions)
                    {
                        context.Remove(item);
                    }

                    var heroQuests = context.HeroesQuestsState
                        .Where(r => r.UserId == id)
                        .Select(r => r).ToList();

                    foreach (var item in heroQuests)
                    {
                        context.Remove(item);
                    }

                    Gold gold = context.Gold
                        .Where(r => r.UserId == userId)
                        .Select(r => r).FirstOrDefault();
                    gold.Number = 0;

                    if (gold != null)
                        context.Update(gold);

                    if (character != null)
                        context.Remove(character);

                    context.SaveChanges();
                }

                Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                return Json(new { success = true });
            }
            catch
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return Json(new { success = false });
            }
        }

        /*************************************/
        /*           MAINTENANCE             */
        /*************************************/
        public ActionResult Maintenance()
        {
            using (var context = new SiteDbContext())
            {
                Conf item = context.Conf
                    .Where(r => r.Id == 1)
                    .Select(r => r).First();

                return View(item);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Maintenance(Conf model)
        {
            using (var context = new SiteDbContext())
            {
                context.Update(model);
                context.SaveChanges();
            }

            return RedirectToAction(nameof(AdminController.Maintenance), "Admin");
        }

        /*************************************/
        /*              DEBUG                */
        /*************************************/
        /*** BATTLE DEBUG ***/
        public IActionResult DebugBattle()
        {
            return View();
        }

        public IActionResult AjaxCreateBattle(int playerVitality, int playerStrength, int playerDefense, int playerEnergy, int monsterId)
        {
            try
            {
                using (var context = new SiteDbContext())
                {
                    var atBattle = context.AtBattlePVE
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r).FirstOrDefault();
                    if(atBattle != null) { context.Remove(atBattle); context.SaveChanges(); }

                    var player = context.Characters
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r).FirstOrDefault();

                    var monster = context.Monsters
                        .Where(r => r.Id == monsterId)
                        .Select(r => r).First();

                    AtBattlePVE atBattlePVEAdd = new AtBattlePVE()
                    {
                        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                        UserLife = 100 + (playerVitality * 10),
                        UserStrength = playerStrength,
                        UserDefense = playerDefense,
                        UserEnergy = playerEnergy,
                        UserVitalityAdd = playerVitality,
                        UserStrengthAdd = 0,
                        UserDefenseAdd = 0,
                        UserEnergyAdd = 0,
                        UserFirstSkillId = player.FirstSkillId,
                        UserSecondSkillId = player.SecondSkillId,
                        UserThirdSkillId = player.ThirdSkillId,
                        UserCurrentEnergy = 90 + (playerEnergy * 10),
                        EntityId = monsterId,
                        EntityLife = monster.Life,
                        EntityCurrentEnergy = 90 + (monster.Energy * 10),
                        EntityCurrentPatternId = 0,
                        TourNumber = 0
                    };

                    context.Add(atBattlePVEAdd);
                    context.SaveChanges();
                }

                Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                return Json(new { success = true });
            }
            catch
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return Json(new { success = true });
            }
        }


        /*** IN GAME DEBUG ***/
        // POST : DebugResetStats
        public IActionResult DebugResetStats()
        {
            using (var context = new SiteDbContext())
            {
                // Characters
                Characters player = context.Characters
                    .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                    .Select(r => r).First();

                player.Life = 100;
                player.Level = 1;
                player.SkillsPoints = 0;
                player.Death = 0;
                player.Xp = 0;

                player.Defense = 1;
                player.Energy = 1;
                player.Strength = 1;

                player.ArmorInventoryId = null;
                player.BagInventoryId = null;
                player.BootsInventoryId = null;
                player.JewelryInventoryId = null;
                player.ShieldInventoryId = null;
                player.WeaponInventoryId = null;

                player.FirstSkillId = 1;
                player.SecondSkillId = 4;
                player.ThirdSkillId = 6;

                context.Update(player);
                context.SaveChanges();
            }
            return RedirectToAction(nameof(GameController.Index), "Game");
        }

        // POST : DebugResetInventory
        public IActionResult DebugResetInventory()
        {
            using (var context = new SiteDbContext())
            {
                // Inventory
                var items = context.Inventory
                    .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                    .Select(r => r).ToList();

                foreach(var item in items)
                {
                    context.Remove(item);
                }

                // Stuff
                Characters player = context.Characters
                    .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                    .Select(r => r).First();

                player.ArmorInventoryId = null;
                player.BagInventoryId = null;
                player.BootsInventoryId = null;
                player.JewelryInventoryId = null;
                player.ShieldInventoryId = null;
                player.WeaponInventoryId = null;

                context.Update(player);
                context.SaveChanges();
            }
            return RedirectToAction(nameof(GameController.Index), "Game");
        }

        // POST: DebugResetGold
        public IActionResult DebugResetGold()
        {
            using (var context = new SiteDbContext())
            {
                Gold item = context.Gold
                    .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                    .Select(r => r).First();

                item.Number = 0;

                context.Update(item);
                context.SaveChanges();
            }
            return RedirectToAction(nameof(GameController.Index), "Game");
        }

        // POST: DebugResetArtifacts
        public IActionResult DebugResetArtifacts()
        {
            using (var context = new SiteDbContext())
            {
                Artifacts item = context.Artifacts
                    .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                    .Select(r => r).First();

                item.Number = 10;

                context.Update(item);
                context.SaveChanges();
            }
            return RedirectToAction(nameof(GameController.Index), "Game");
        }

        // POST: DebugDeleteLastHeroQuest
        public IActionResult DebugDeleteLastHeroQuest()
        {
            using (var context = new SiteDbContext())
            {
                HeroesQuestsState item = context.HeroesQuestsState
                    .OrderByDescending(r => r.Id)
                    .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                    .Select(r => r).First();

                context.Remove(item);
                context.SaveChanges();
            }
            return RedirectToAction(nameof(GameController.Index), "Game");
        }

        // POST: DebugDeleteCurrentBattle
        public IActionResult DebugDeleteCurrentBattle()
        {
            using (var context = new SiteDbContext())
            {
                AtBattlePVE item = context.AtBattlePVE
                    .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                    .Select(r => r).First();

                context.Remove(item);
                context.SaveChanges();
            }
            return RedirectToAction(nameof(GameController.Index), "Game");
        }
    }
}