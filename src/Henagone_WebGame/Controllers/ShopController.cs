using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Henagone_WebGame.Models;
using Henagone_WebGame.Models.ShopViewModels;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Stripe;
using Microsoft.AspNetCore.Identity;
using Henagone_WebGame.Models.GameViewModels;
using System.Globalization;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.IO;
using System.Xml.Linq;
using ExtendedXmlSerialization;
using System.Net.Http.Headers;

namespace Henagone_WebGame.Controllers
{
    [Authorize]
    public class ShopController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public ShopController(UserManager<ApplicationUser> userManager) {
            _userManager = userManager;
        }

        // Get: Index
        public IActionResult Index()
        {
            //return View();
            return RedirectToAction(nameof(ShopController.Purchase), "Shop");
        }

        // GET: Purchase
        public IActionResult Purchase()
        {
            return View();
        }

        // GET: Purchase
        public IActionResult Finished()
        {
            return View();
        }

        // POST: HiPaySuccess
        private struct HIPAYRETURN
        {
            public string status { get; set; }
            public string status_description { get; set; }
            public string access_type { get; set; }
            public string transaction_id { get; set; }
            public string price { get; set; }
            public string paid { get; set; }
            public string validation_date { get; set; }
            public string product_name { get; set; }
            public string website { get; set; }
            public string customer_ip { get; set; }
            public string customer_country { get; set; }
            public string expected_number_of_codes { get; set; }
            public string[] codes { get; set; }
            public string merchant_transaction_id { get; set; }
            public string data { get; set; }
            public string affiliate { get; set; }
            public string partners { get; set; }
        }

        public IActionResult HiPaySuccess(string code)
        {
            try
            {
                HttpResponseMessage resultHttp;
                using (var http = new HttpClient())
                {
                    var formContent = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("site_id", "338486"),
                        new KeyValuePair<string, string>("product_id", "1486845"),
                        new KeyValuePair<string, string>("code[]", code)
                    });
                   resultHttp = http.PostAsync(@"https://payment.allopass.com/api/onetime/validate-codes.apu", formContent).Result;
                }

                // Read XML
                ExtendedXmlSerializer serializer = new ExtendedXmlSerializer();
                HIPAYRETURN dataAPI = serializer.Deserialize<HIPAYRETURN>(resultHttp.Content.ReadAsStringAsync().Result);

                if(dataAPI.status == "0" && dataAPI.status_description == "success")
                {
                    using (var context = new SiteDbContext())
                    {
                        double doublePrice = -1.0;
                        double.TryParse(dataAPI.price, out doublePrice);
                        if (doublePrice == -1.0)
                            doublePrice = 0.0;
                        

                        // Add Deal
                        Deals_statistics stats = new Deals_statistics()
                        {
                            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                            Code = code + "(" + dataAPI.transaction_id + ")",
                            Date = DateTime.Now,
                            Price = doublePrice,
                            Nb_Artifacts = 16,
                            PaymentType = "HiPay Mobile"
                        };
                        context.Deals_statistics.Add(stats);

                        //Add Artifact
                        Artifacts artifact = context.Artifacts
                                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                                        .Select(r => r).FirstOrDefault();

                        artifact.Number += (int)16;
                        context.Update(artifact);

                        context.SaveChanges();
                    }

                    TempData["success"] = "Your payment has been successfully completed! You have received your artefacts.";
                }  
                else
                {
                    TempData["error"] = "Code can already be used or invalid. If the problem contact the support.";
                }

                return RedirectToAction(nameof(ShopController.Finished), "Shop");
            }
            catch
            {
                TempData["error"] = "Error during payment, code can already be used. If the problem  contact the support.";
                return RedirectToAction(nameof(ShopController.Finished), "Shop");
            }
        }

        // POST: PaidWithPaypal
        private struct PAYPALLINKS
        {
            public string href { get; set; }
            public string rel { get; set; }
            public string method { get; set; }
        }

        private struct PAYPALRETURN
        {
            public string id { get; set; }
            public string create_time { get; set; }
            public string update_time { get; set; }
            public string state { get; set; }
            public string intent { get; set; }
            public PAYPALLINKS[] links { get; set; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PaidWithPaypal(PaypalForm model)
        {
            try
            {
                // Get Price
                ArtifactsPrices price;
                using (var context = new SiteDbContext())
                {
                    price = context.ArtifactsPrices
                        .Where(r => r.Id == model.DealsId)
                        .Select(r => r).FirstOrDefault();
                }
                if (price == null) throw new ArgumentNullException("Price error");
                var item_name = "Artifact - N°" + price.Id;

                HttpResponseMessage resultHttp;
                using (var http = new HttpClient())
                {
                    StringContent stringContent = new StringContent(
                         "{"
                         + "\"intent\": \"sale\","
                         + "\"payer\": {"
                            + "\"payment_method\": \"paypal\""
                         + "},"
                         + "\"transactions\": ["
                         + "{"
                            + "\"amount\": {"
                            + "\"total\": \"" + Convert.ToDouble(price.Price).ToString() + "\","
                            + "\"currency\": \"EUR\","
                            + "\"details\": {"
                                + "\"subtotal\": \"" + Convert.ToDouble(price.Price).ToString() + "\","
                                + "\"tax\": \"0.00\""
                            + "}"
                            + "},"
                            + "\"description\": \"Artifact Purchase\","
                            + "\"custom\": \"" + User.FindFirstValue(ClaimTypes.NameIdentifier) + "\","
                            + "\"payment_options\": {"
                                + "\"allowed_payment_method\": \"INSTANT_FUNDING_SOURCE\""
                            + "},"
                            + "],"
                            + "\"redirect_urls\": {"
                                + "\"return_url\": \"#\","
                                + "\"cancel_url\": \"#\""
                            + "}"
                         + "}"
                         +"}",
                        UnicodeEncoding.UTF8,
                        "application/json");


                    http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Convert.ToBase64String(Encoding.ASCII.GetBytes("EGT_CzVaelAyj3wb_InciUD5jyOXg2jgiaQyFvkShjx-zvMKYBqUrtTYodzuhQaMY2jwgMGYCL3K79IK")));
                    resultHttp = http.PostAsync(@"https://api.sandbox.paypal.com/v1/payments/payment", stringContent).Result;
                }

                PAYPALRETURN paypalReturn = JsonConvert.DeserializeObject<PAYPALRETURN>(resultHttp.Content.ReadAsStringAsync().Result);
                TempData["error"] = resultHttp.Content.ReadAsStringAsync().Result;
                return RedirectToAction(nameof(ShopController.Finished), "Shop");
            }
            catch
            {
                TempData["error"] = "Error during payment, check your card code. If the problem persists contact the support.";
                return RedirectToAction(nameof(ShopController.Finished), "Shop");
            }
        }

        // POST: PaidWithCard
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PaidWithCard(StripeChargeForm model)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                ApplicationUser user = await _userManager.FindByIdAsync(userId);

                // Get Price
                ArtifactsPrices price;
                using (var context = new SiteDbContext())
                {
                    price = context.ArtifactsPrices
                        .Where(r => r.Id == model.DealsId)
                        .Select(r => r).FirstOrDefault();
                }
                if(price == null) throw new ArgumentNullException("Price error");

                // Create or Get Customers
                string stripeId = user.StripeId;
                if (stripeId == null)
                {
                    var newCustomer = new StripeCustomerCreateOptions();
                    newCustomer.Email = user.Email;
                    newCustomer.Description = $"{model.Name} ({user.UserName})";
                    newCustomer.SourceToken = model.Token;

                    var customerService = new StripeCustomerService();
                    StripeCustomer stripeCustomer = customerService.Create(newCustomer);
                    stripeId = stripeCustomer.Id;

                    // Update User
                    user.StripeId = stripeId;
                    await _userManager.UpdateAsync(user);
                }

                // Charge
                var myCharge = new StripeChargeCreateOptions();

                myCharge.Amount = (int)(price.Price * 100);
                myCharge.Currency = "eur";
                myCharge.Description = "Le mythe des montagnes - Shop(Id: " + price.Id + ")";
                myCharge.CustomerId = stripeId;

                var chargeService = new StripeChargeService();
                StripeCharge stripeCharge = chargeService.Create(myCharge);

                // Verify
                if (stripeCharge.Status == "succeeded")
                {
                    using (var context = new SiteDbContext())
                    {
                        // Add Deal
                        Deals_statistics stats = new Deals_statistics()
                        {
                            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                            Date = DateTime.Now,
                            Price = (double)price.Price,
                            Nb_Artifacts = price.Artifacts,
                            PaymentType = "Stripe",
                        };
                        context.Deals_statistics.Add(stats);

                        //Add Artifact
                        Artifacts artifact = context.Artifacts
                                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                                        .Select(r => r).FirstOrDefault();

                        artifact.Number += (int)price.Artifacts;
                        context.Update(artifact);

                        context.SaveChanges();
                    }

                    // Set message
                    TempData["success"] = "Your payment has been successfully completed! You have received your artefacts.";
                }
                else
                    TempData["error"] = "Error during payment, check your card code. If the problem persists contact the support.";

                return RedirectToAction(nameof(ShopController.Finished), "Shop");
            }
            catch
            {
                TempData["error"] = "Error during payment, check your card code. If the problem persists contact the support.";
                return RedirectToAction(nameof(ShopController.Finished), "Shop");
            }
        }

        // POST: AjaxRedeemGift
        public IActionResult AjaxRedeemGift(string id)
        {
            try
            {
                string jsonString = "[";
                using (var context = new SiteDbContext())
                {
                    var now = DateTime.Now;

                    Gifts gift = context.Gifts
                    .Where(r => r.Code.Replace("-", "") == id && r.Active == true)
                    .Select(r => r).SingleOrDefault();

                    if (gift == null) throw new InvalidCastException("Your code is incorrect or has already been used."); 
                    if(!(now >= gift.Start && now <= gift.End)) throw new InvalidCastException("Your code has expired or is not active yet.");

                    if(!gift.SinglePerAccount)
                        gift.Active = false;

                    // SinglePerAccount
                    if (gift.SinglePerAccount)
                    {
                        var AccountAlreadyUseIt = context.Deals_statistics
                            .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && r.Code.Replace("-", "") == id)
                            .Count();
                        if (AccountAlreadyUseIt > 0) throw new InvalidCastException("The code already used by your account.");
                    }

                    Deals_statistics stats = new Deals_statistics()
                    {
                        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                        Date = DateTime.Now,
                        Code = gift.Code,
                        Price = 0,
                        Nb_Artifacts = (int)gift.Artifacts,
                        PaymentType = "Gift Code",
                    };
                    context.Deals_statistics.Add(stats);

                    if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr")
                        jsonString += "{ \"Success\" : \"true\",\"Title\" : \"Bravo\",\"Msg\" : \"Vous venez de valider votre code cadeau, les bonus ont été ajoutés à votre compte !\" }";
                    else
                        jsonString += "{ \"Success\" : \"true\",\"Title\" : \"Bravo\",\"Msg\" : \"You just validate your gift code, the bonuses have been added to your account !\" }";

                    // Artifacts
                    if (gift.Artifacts != null)
                    {
                        if (gift.Artifacts > 0)
                        {
                            Artifacts artifact = context.Artifacts
                                .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                                .Select(r => r).FirstOrDefault();

                                artifact.Number += (int)gift.Artifacts;
                                context.Update(artifact);

                            if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr")
                                jsonString += ",{\"Title\":\"Artefacts\", \"Text\":\"Vous obtenez " + gift.Artifacts + " artefats.\", \"Img\":\"/images/Game/artifact.svg\"}";
                            else
                                jsonString += ",{\"Title\":\"Artifacts\", \"Text\":\"You obtain " + gift.Artifacts + " artifacts.\", \"Img\":\"/images/Game/artifact.svg\"}";
                        }
                    }

                    // Xp
                    if (gift.Experiences != null)
                    {
                        if (gift.Experiences > 0)
                        {
                            Characters player = context.Characters
                                .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                                .Select(r => r).FirstOrDefault();

                            player.Xp += (int)gift.Experiences;
                            context.Update(player);

                            if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr")
                                jsonString += ",{\"Title\":\"Expérience\", \"Text\":\"Vous obtenez " + gift.Experiences + " d'expériences\", \"Img\":\"/images/Game/gold.svg\"}";
                            else
                                jsonString += ",{\"Title\":\"Experience\", \"Text\":\"You obtain " + gift.Experiences + " experiences.\", \"Img\":\"/images/Game/gold.svg\"}";
                        }
                    }

                    // Items
                    if (gift.Items != null)
                    {
                        IList<string> items = gift.Items.Split(',').ToList();
                        foreach(string item in items)
                        {
                            var itemExist = context.Items
                                .Where(r => r.Id == Int32.Parse(item))
                                .Select(r => r).FirstOrDefault();

                            if (itemExist != null)
                            {
                                Inventory inventory = new Inventory()
                                {
                                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                                    ItemId = Int32.Parse(item)
                                };

                                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr")
                                    jsonString += ",{\"Title\":\"" + itemExist.TitleFr + "\", \"Text\":\"Vous trouverez l'objet dans votre inventaire.\", \"Img\":\"/uploads/game/cards/items/" + itemExist.Id + ".fr.svg\"}";
                                else
                                    jsonString += ",{\"Title\":\"" + itemExist.TitleEn + "\", \"Text\":\"You will find the item in your inventory.\", \"Img\":\"/uploads/game/cards/items/" + itemExist.Id + ".en.svg\"}";

                                context.Add(inventory);
                            }
                        }
                    }

                    context.Update(gift);
                    context.SaveChanges();
                }

                jsonString += "]";
                return Json(JsonConvert.DeserializeObject(jsonString));
            }
            catch (InvalidCastException e)
            {
                string jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Error\",\"Msg\" : \"" + e.Message + "\" }]";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr")
                {
                    if (e.Message == "Your code has expired or is not active yet.")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Votre code a expiré ou n'est pas encore actif.\" }]";
                    else if(e.Message == "Your code is incorrect or has already been used.")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Votre code est incorrect ou a déjà été utilisé.\" }]";
                    else if(e.Message == "The code already used by your account.")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Le code à déjà utilisé par votre compte.\" }]";
                }

                return Json(JsonConvert.DeserializeObject(jsonString));
            }
        }
    }
}