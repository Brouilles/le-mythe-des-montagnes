using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Henagone_WebGame.Models.GameViewModels;
using Henagone_WebGame.Models;
using System.Security.Claims;
using Henagone_WebGame.Models.ShopViewModels;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using Henagone_WebGame.Models.AdminViewModels;

public static class Util
{
    public static string ItemTitle(Items item)
    {
        if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr") //French
            return item.TitleFr;
        else //English
            return item.TitleEn;
    }

    public static string ItemDescription(Items item)
    {
        string sentence = "";

        if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr") //French
        {
            sentence += "<strong>Type :</strong> ";

            if (item.Type == 0)
                sentence += "Arme";
            else if (item.Type == 1)
                sentence += "Armure";
            else if (item.Type == 2)
                sentence += "Bouclier";
            else if (item.Type == 3)
                sentence += "Bottes";
            else if (item.Type == 4)
                sentence += "Bijoux";
            else if (item.Type == 5)
                sentence += "Sac";

            sentence += "<br/><strong>Poids :</strong> " + item.Weight + "<br/>";

            if (item.DefenseRequired > 0 || item.EnergyRequired > 0 || item.StrengthRequired > 0)
            {
                sentence += "<strong>Aptitude nécessaire</strong>";
                sentence += "<ul>";
                if (item.DefenseRequired > 0)
                    sentence += $"<li class=\"text-success\">+{item.DefenseRequired} <small>Défense</small></li>";

                if (item.EnergyRequired > 0)
                    sentence += $"<li class=\"text-success\">+{item.EnergyRequired} <small>Energie</small></li>";

                if (item.StrengthRequired > 0)
                    sentence += $"<li class=\"text-success\">+{item.StrengthRequired} <small>Force</small></li>";
                sentence += "</ul>";
            }

            sentence += "<strong>Bonus</strong>";
            sentence += "<ul>";

            if (item.VitalityAdd > 0)
                sentence += $"<li class=\"text-success\">+{item.VitalityAdd} <small>Augmente la vitalité</small></li>";
            else if (item.VitalityAdd < 0)
                sentence += $"<li class=\"text-warning\">{item.VitalityAdd} <small>Diminue la vitalité</small></li>";

            if (item.StrengthAdd > 0)
                sentence += $"<li class=\"text-success\">+{item.StrengthAdd} <small>Augmente la force</small></li>";
            else if (item.StrengthAdd < 0)
                sentence += $"<li class=\"text-warning\">{item.StrengthAdd} <small>Diminue la force</small></li>";

            if (item.DefenseAdd > 0)
                sentence += $"<li class=\"text-success\">+{item.DefenseAdd} <small>Augmente la défense</small></li>";
            else if (item.DefenseAdd < 0)
                sentence += $"<li class=\"text-warning\">{item.DefenseAdd} <small>Diminue la défense</small></li>";

            if (item.EnergyAdd > 0)
                sentence += $"<li class=\"text-success\">+{item.EnergyAdd} <small>Augmente l'énergie</small></li>";
            else if (item.EnergyAdd < 0)
                sentence += $"<li class=\"text-warning\">{item.EnergyAdd} <small>Diminue l'énergie</small></li>";

            sentence += "</ul>";
        }
        else //English
        {
            sentence += "<strong>Type :</strong> ";

            if (item.Type == 0)
                sentence += "Weapon";
            else if (item.Type == 1)
                sentence += "Armor";
            else if (item.Type == 2)
                sentence += "Shield";
            else if (item.Type == 3)
                sentence += "Boots";
            else if (item.Type == 4)
                sentence += "Jewelry";
            else if (item.Type == 5)
                sentence += "Bag";

            sentence += "<br/><strong>Weight :</strong> " + item.Weight + "<br/>";

            if (item.DefenseRequired > 0 || item.EnergyRequired > 0 || item.StrengthRequired > 0)
            {
                sentence += "<strong>Required ability</strong>";
                sentence += "<ul>";
                if (item.DefenseRequired > 0)
                    sentence += $"<li class=\"text-success\">+{item.DefenseRequired} <small>Defense</small></li>";

                if (item.EnergyRequired > 0)
                    sentence += $"<li class=\"text-success\">+{item.EnergyRequired} <small>Energy</small></li>";

                if (item.StrengthRequired > 0)
                    sentence += $"<li class=\"text-success\">+{item.StrengthRequired} <small>Strength</small></li>";
                sentence += "</ul>";
            }

            sentence += "<strong>Bonus</strong>";
            sentence += "<ul>";

            if (item.VitalityAdd > 0)
                sentence += $"<li class=\"text-success\">+{item.VitalityAdd} <small>Increases vitality</small></li>";
            else if (item.VitalityAdd < 0)
                sentence += $"<li class=\"text-warning\">{item.VitalityAdd} <small>Decreases vitality</small></li>";

            if (item.StrengthAdd > 0)
                sentence += $"<li class=\"text-success\">+{item.StrengthAdd} <small>Increases strength</small></li>";
            else if (item.StrengthAdd < 0)
                sentence += $"<li class=\"text-warning\">{item.StrengthAdd} <small>Decreases strength</small></li>";

            if (item.DefenseAdd > 0)
                sentence += $"<li class=\"text-success\">+{item.DefenseAdd} <small>Increases defense</small></li>";
            else if (item.DefenseAdd < 0)
                sentence += $"<li class=\"text-warning\">{item.DefenseAdd} <small>Decreases defense</small></li>";

            if (item.EnergyAdd > 0)
                sentence += $"<li class=\"text-success\">+{item.EnergyAdd} <small>Increases energy</small></li>";
            else if (item.EnergyAdd < 0)
                sentence += $"<li class=\"text-warning\">{item.EnergyAdd} <small>Decreases energy</small></li>";

            sentence += "</ul>";
        }
        return sentence;
    }

    public static string CompanionQuestBonus(CompanionQuests item)
    {
        string sentence = "<ul style=\"padding-left: 4px;\">";
        using (var context = new SiteDbContext()) {
            if (item.Potions != null) // Potions
            {
                if (item.Potions != String.Empty)
                {
                    IList<string> potions = item.Potions.Split(',').ToList();
                    foreach (string potion in potions)
                    {
                        var itemExist = context.Potions
                            .Where(r => r.Id == Int32.Parse(potion))
                            .Select(r => r).FirstOrDefault();

                        if (itemExist != null)
                            sentence += $"<li class=\"text-success\">{Util.LanguageSelection(itemExist.TitleEn, itemExist.TitleFr)}</li>";
                    }
                }
            }

            // Items
            if (item.Items != null)
            {
                IList<string> items = item.Items.Split(',').ToList();
                foreach (string thing in items)
                {
                    var itemExist = context.Items
                        .Where(r => r.Id == Int32.Parse(thing))
                        .Select(r => r).FirstOrDefault();

                    if (itemExist != null)
                        sentence += $"<li class=\"text-success\">{Util.LanguageSelection(itemExist.TitleEn, itemExist.TitleFr)}</li>";
                }
            }
        }

        if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr") //French
        {
            if (item.Gold > 0)
                sentence += $"<li class=\"text-success\">+{item.Gold} <small>Or</small></li>";

            if (item.Xp > 0)
                sentence += $"<li class=\"text-success\">+{item.Xp} <small>Xp</small></li>";

            if (item.StrengthAdd > 0)
                sentence += $"<li class=\"text-success\">+{item.StrengthAdd} <small>Augmente la force</small></li>";

            if (item.DefenseAdd > 0)
                sentence += $"<li class=\"text-success\">+{item.DefenseAdd} <small>Augmente la défense</small></li>";

            if (item.EnergyAdd > 0)
                sentence += $"<li class=\"text-success\">+{item.EnergyAdd} <small>Augmente l`énergie</small></li>";

            if (item.Care > 0)
                sentence += $"<li class=\"text-success\">+{item.Care}% de soin</li>";
        }
        else
        {
            if (item.Gold > 0)
                sentence += $"<li class=\"text-success\">+{item.Gold} <small>Gold</small></li>";

            if (item.Xp > 0)
                sentence += $"<li class=\"text-success\">+{item.Xp} <small>Xp</small></li>";

            if (item.StrengthAdd > 0)
                sentence += $"<li class=\"text-success\">+{item.StrengthAdd} <small>Increased strength</small></li>";

            if (item.DefenseAdd > 0)
                sentence += $"<li class=\"text-success\">+{item.DefenseAdd} <small>Increases defense</small></li>";

            if (item.EnergyAdd > 0)
                sentence += $"<li class=\"text-success\">+{item.EnergyAdd} <small>Increased energy</small></li>";

            if (item.Care > 0)
                sentence += $"<li class=\"text-success\">+{item.Care}% care</li>";
        }

        return sentence + "</ul>";
    }

    public static string PotionDescription(Potions item)
    {
        string sentence = "<ul>";

        if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr") //French
        {
            if (item.VitalityAdd > 0)
                sentence += $"<li class=\"text-success\">+{item.VitalityAdd} <small>Augmente la vitalité</small></li>";
            else if (item.VitalityAdd < 0)
                sentence += $"<li class=\"text-warning\">{item.VitalityAdd} <small>Diminue la vitalité</small></li>";

            if (item.StrengthAdd > 0)
                sentence += $"<li class=\"text-success\">+{item.StrengthAdd} <small>Augmente la force</small></li>";
            else if (item.StrengthAdd < 0)
                sentence += $"<li class=\"text-warning\">{item.StrengthAdd} <small>Diminue la force</small></li>";

            if (item.DefenseAdd > 0)
                sentence += $"<li class=\"text-success\">+{item.DefenseAdd} <small>Augmente la défense</small></li>";
            else if (item.DefenseAdd < 0)
                sentence += $"<li class=\"text-warning\">{item.DefenseAdd} <small>Diminue la défense</small></li>";

            if (item.EnergyAdd > 0)
                sentence += $"<li class=\"text-success\">+{item.EnergyAdd} <small>Augmente l'énergie</small></li>";
            else if (item.EnergyAdd < 0)
                sentence += $"<li class=\"text-warning\">{item.EnergyAdd} <small>Diminue l'énergie</small></li>";
        }
        else
        {
            if (item.VitalityAdd > 0)
                sentence += $"<li class=\"text-success\">+{item.VitalityAdd} <small>Increased vitality</small></li>";
            else if (item.VitalityAdd < 0)
                sentence += $"<li class=\"text-warning\">{item.VitalityAdd} <small>Decreases vitality</small></li>";

            if (item.StrengthAdd > 0)
                sentence += $"<li class=\"text-success\">+{item.StrengthAdd} <small>Increased strength</small></li>";
            else if (item.StrengthAdd < 0)
                sentence += $"<li class=\"text-warning\">{item.StrengthAdd} <small>Decreases strength</small></li>";

            if (item.DefenseAdd > 0)
                sentence += $"<li class=\"text-success\">+{item.DefenseAdd} <small>Increased defense</small></li>";
            else if (item.DefenseAdd < 0)
                sentence += $"<li class=\"text-warning\">{item.DefenseAdd} <small>Decreases defense</small></li>";

            if (item.EnergyAdd > 0)
                sentence += $"<li class=\"text-success\">+{item.EnergyAdd} <small>Increased energy</small></li>";
            else if (item.EnergyAdd < 0)
                sentence += $"<li class=\"text-warning\">{item.EnergyAdd} <small>Decreases energy</small></li>";
        }

        sentence += "</ul>";
        return sentence;
    }

    public static string LanguageSelection(string en, string fr)
    {
        if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr") // French
        {
            return fr;
        }
        else // English
        {
            return en;
        }
    }

    public static int? GetXpforUp(int level)
    {
        switch (level)
        {
            case 1:
                return 100;
            case 2:
                return 200;
            case 3:
                return 300;
            case 4:
                return 400;
            case 5:
                return 500;
            case 6:
                return 650;
            case 7:
                return 800;
            case 8:
                return 950;
            case 9:
                return 1100;
            case 10:
                return 1250;
            case 11:
                return 1450;
            case 12:
                return 1650;
            case 13:
                return 1850;
            case 14:
                return 2050;
            case 15:
                return 2250;
            case 16:
                return 2500;
            case 17:
                return 2750;
            case 18:
                return 3000;
            case 19:
                return 3250;
            case 20:
                return 3500;
            case 21:
                return 3800;
            case 22:
                return 4100;
            case 23:
                return 4400;
            case 24:
                return 4700;
            case 25:
                return 5000;
            default:
                return null;
        }
    }

    public static SkillViewModel GetSkillData(int id, ClaimsPrincipal user)
    {
        SkillViewModel skill = new SkillViewModel();
        Characters player;
        using (var context = new SiteDbContext())
        {
            player = context.Characters
                .Where(r => r.UserId == user.FindFirstValue(ClaimTypes.NameIdentifier))
                .Select(r => r).First();
        }

        int totalStrength = player.Strength + player.StrengthAdd;
        int totalDefense = player.Defense + player.DefenseAdd;
        int totalEnergy = (player.Energy + player.EnergyAdd);

        int maxDmg = totalStrength * 10;
        int maxPct = totalDefense * 10;

        if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr") //French
        {
            switch(id)
            {
                // Attack
                case 1:
                    skill.Title = "Coupe-genou";
                    skill.Effect = "Entre 0 et " + maxDmg / 2 + " de dégât";
                    skill.Cost = maxDmg / 2 / totalEnergy + " de coût d'énergie";
                    skill.Description = "<ul><li class=\"text-success\"> " + skill.Effect + "</li><li class=\"text-warning\">" + skill.Cost + "</li></ul>";
                    break;
                case 2:
                    skill.Title = "Coup d’estoc";
                    skill.Effect = "Entre 0 et " + maxDmg + " de dégât";
                    skill.Cost = maxDmg / totalEnergy + " de coût d'énergie";
                    skill.Description = "<ul><li class=\"text-success\"> " + skill.Effect + "</li><li class=\"text-warning\">" + skill.Cost + "</li></ul>";
                    break;
                case 3:
                    skill.Title = "Attaque puissante";
                    skill.Effect = "Entre 0 et " + maxDmg * 2 + " de dégât";
                    skill.Cost = maxDmg * 2 / totalEnergy + " de coût d'énergie";
                    skill.Description = "<ul><li class=\"text-success\"> " + skill.Effect + "</li><li class=\"text-warning\">" + skill.Cost + "</li></ul>";
                    break;
                // Defense
                case 4:
                    skill.Title = "Parade";
                    skill.Effect = "Entre 0 et " + maxPct / 2 + " de protection";
                    skill.Cost = maxPct / 2 / totalEnergy + " de coût d'énergie";
                    skill.Description = "<ul><li class=\"text-success\"> " + skill.Effect + "</li><li class=\"text-warning\">" + skill.Cost + "</li></ul>";
                    break;
                case 5:
                    skill.Title = "Blocage";
                    skill.Effect = "Entre 0 et " + maxPct + " de protection";
                    skill.Cost = maxPct / totalEnergy + " de coût d'énergie";
                    skill.Description = "<ul><li class=\"text-success\"> " + skill.Effect + "</li><li class=\"text-warning\">" + skill.Cost + "</li></ul>";
                    break;
                case 6:
                    skill.Title = "Couverture";
                    skill.Effect = "Entre 0 et " + maxPct * 2 + " de protection";
                    skill.Cost = maxPct * 2 / totalEnergy + " de coût d'énergie";
                    skill.Description = "<ul><li class=\"text-success\"> " + skill.Effect + "</li><li class=\"text-warning\">" + skill.Cost + "</li></ul>";
                    break;
                // Energy
                case 7:
                    skill.Title = "Repos";
                    skill.Effect = ((totalEnergy * 10) + 90) / 5 + " d'énergie et " + (100 + player.VitalityAdd) / 5 + " de vie";
                    skill.Cost = "0 de coût d'énergie";
                    skill.Description = "<ul><li class=\"text-success\"> " + skill.Effect + "</li><li class=\"text-warning\">" + skill.Cost + "</li></ul>";
                    break;
                case 8:
                    skill.Title = "Régénération";
                    skill.Effect = ((totalEnergy * 10) + 90) / 3 + " d'énergie";
                    skill.Cost = "0 de coût d'énergie";
                    skill.Description = "<ul><li class=\"text-success\"> " + skill.Effect + "</li><li class=\"text-warning\">" + skill.Cost + "</li></ul>";
                    break;
                case 9:
                    skill.Title = "Effort";
                    skill.Effect = ((totalEnergy * 10) + 90) / 2 + " d'énergie";
                    skill.Cost = player.Life / 4 + " de coût de points de vie";
                    skill.Description = "<ul><li class=\"text-success\"> " + skill.Effect + "</li><li class=\"text-danger\">" + skill.Cost + "</li></ul>";
                    break;
            }
        }
        else
        {
            switch (id)
            {
                // Attack
                case 1:
                    skill.Title = "Knee cutters";
                    skill.Effect = "Between 0 and " + maxDmg / 2 + " damage";
                    skill.Cost = maxDmg / 2 / totalEnergy + " energy cost";
                    skill.Description = "<ul><li class=\"text-success\"> " + skill.Effect + "</li><li class=\"text-warning\">" + skill.Cost + "</li></ul>";
                    break;
                case 2:
                    skill.Title = "Blow";
                    skill.Effect = "Between 0 and " + maxDmg + " damage";
                    skill.Cost = maxDmg / totalEnergy + " energy cost";
                    skill.Description = "<ul><li class=\"text-success\"> " + skill.Effect + "</li><li class=\"text-warning\">" + skill.Cost + "</li></ul>";
                    break;
                case 3:
                    skill.Title = "Powerful Attack";
                    skill.Effect = "Between 0 and " + maxDmg * 2 + " damage";
                    skill.Cost = maxDmg * 2 / totalEnergy + " energy cost";
                    skill.Description = "<ul><li class=\"text-success\"> " + skill.Effect + "</li><li class=\"text-warning\">" + skill.Cost + "</li></ul>";
                    break;
                // Defense
                case 4:
                    skill.Title = "Parade";
                    skill.Effect = "Between 0 and " + maxPct / 2 + " protection";
                    skill.Cost = maxPct / 2 / totalEnergy + " energy cost";
                    skill.Description = "<ul><li class=\"text-success\"> " + skill.Effect + "</li><li class=\"text-warning\">" + skill.Cost + "</li></ul>";
                    break;
                case 5:
                    skill.Title = "Blocage";
                    skill.Effect = "Between 0 and " + maxPct + " protection";
                    skill.Cost = maxPct / totalEnergy + " energy cost";
                    skill.Description = "<ul><li class=\"text-success\"> " + skill.Effect + "</li><li class=\"text-warning\">" + skill.Cost + "</li></ul>";
                    break;
                case 6:
                    skill.Title = "Cover";
                    skill.Effect = "Between 0 and " + maxPct * 2 + " protection";
                    skill.Cost = maxPct * 2 / totalEnergy + " energy cost";
                    skill.Description = "<ul><li class=\"text-success\"> " + skill.Effect + "</li><li class=\"text-warning\">" + skill.Cost + "</li></ul>";
                    break;
                // Energy
                case 7:
                    skill.Title = "Rest";
                    skill.Effect = ((totalEnergy * 10) + 90) / 5 + " of energy and " + (100 + player.VitalityAdd) / 5 + " life";
                    skill.Cost = "0 energy cost";
                    skill.Description = "<ul><li class=\"text-success\"> " + skill.Effect + "</li><li class=\"text-warning\">" + skill.Cost + "</li></ul>";
                    break;
                case 8:
                    skill.Title = "Regeneration";
                    skill.Effect = ((totalEnergy * 10) + 90) / 3 + " of energy";
                    skill.Cost = "0 energy cost";
                    skill.Description = "<ul><li class=\"text-success\"> " + skill.Effect + "</li><li class=\"text-warning\">" + skill.Cost + "</li></ul>";
                    break;
                case 9:
                    skill.Title = "Effort";
                    skill.Effect = ((totalEnergy * 10) + 90) / 2 + " of energy";
                    skill.Cost = player.Life / 4 + "  Cost of Hit Points";
                    skill.Description = "<ul><li class=\"text-success\"> " + skill.Effect + "</li><li class=\"text-danger\">" + skill.Cost + "</li></ul>";
                    break;
            }
        }

        return skill;
    }

    public static string GetSkillType(int id)
    {
        switch(id)
        {
            case 1:
                return "attack";
            case 2:
                return "attack";
            case 3:
                return "attack";
            case 4:
                return "defense";
            case 5:
                return "defense";
            case 6:
                return "defense";
            case 7:
                return "energy";
            case 8:
                return "energy";
            case 9:
                return "energy";
            default:
                return "";
        }
    }
}

namespace Henagone_WebGame.Controllers
{
    [Authorize]
    public class GameController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public GameController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        private void CompanionQuest(int confId, CompanionQuests[] CompanionQuests)
        {
            using (var context = new SiteDbContext())
            {
                Conf temp = context.Conf.Where(r => r.Id == confId).Select(r => r).First();
                temp.Value = "";
                var lastRandom = new int[] { -1, -1, -1 };
                int random = 0;

                for (int i = 0; i < 3; i++)
                {
                    switch (1)
                    {
                        case 1:
                            random = new Random(Guid.NewGuid().GetHashCode()).Next(0, CompanionQuests.Length);
                            goto default;
                        default:
                            foreach (var item in lastRandom)
                            {
                                if (item != -1)
                                {
                                    if (random == item)
                                        goto case 1;
                                }
                            }
                            break;
                    }

                    temp.Value += CompanionQuests[random].Id.ToString();

                    if (lastRandom[i] == -1)
                        lastRandom[i] = random;

                    if (i != 2)
                        temp.Value += ",";
                }

                context.Update(temp);
                context.SaveChanges();
            }
        }

        /*************************************/
        /*              INDEX                */
        /*************************************/
        // GET : Index
        public async Task<IActionResult> Index()
        {
            Characters model = new Characters();

            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            ViewBag.Email = user.Email;

            using (var context = new SiteDbContext())
            {
                model = context.Characters
                    .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                    .Select(r => r).FirstOrDefault();

                if (model == null)
                    return RedirectToAction(nameof(GameController.CreateCharacter), "Game");

                // Update Companion Quest
                Conf temp = context.Conf.Where(r => r.Id == 2).Select(r => r).First();
                DateTime tempTime = DateTime.ParseExact(temp.Value, "dd/MM/yyyy HH:mm:ss", null);
                if(DateTime.Compare(DateTime.Now.Date, tempTime.Date) > 0 || temp.Value == null)
                {
                    temp.Value = DateTime.Now.Date.ToString("dd/MM/yyyy HH:mm:ss");
                    context.Update(temp);

                    // Warrior
                    var CompanionQuests = context.CompanionQuests
                        .Where(r => r.Class == 0)
                        .Select(r => r).ToArray();
                    CompanionQuest(3, CompanionQuests);

                    // Sentinel
                    CompanionQuests = context.CompanionQuests
                        .Where(r => r.Class == 1)
                        .Select(r => r).ToArray();
                    CompanionQuest(4, CompanionQuests);

                    // Messenger
                    CompanionQuests = context.CompanionQuests
                        .Where(r => r.Class == 2)
                        .Select(r => r).ToArray();
                    CompanionQuest(5, CompanionQuests);

                    // Healer
                    CompanionQuests = context.CompanionQuests
                        .Where(r => r.Class == 4)
                        .Select(r => r).ToArray();
                    CompanionQuest(7, CompanionQuests);

                    // Thief
                    CompanionQuests = context.CompanionQuests
                        .Where(r => r.Class == 5)
                        .Select(r => r).ToArray();
                    CompanionQuest(8, CompanionQuests);

                    // WeaponMaster
                    CompanionQuests = context.CompanionQuests
                        .Where(r => r.Class == 6)
                        .Select(r => r).ToArray();
                    CompanionQuest(9, CompanionQuests);
                }

                // Tutorial
                if (model.Tutorial == false)
                {
                    ViewBag.Tutorial = true;
                    model.Tutorial = true;

                    context.Update(model);
                }
                context.SaveChanges();
            }

            this.CheckCharacterLevel();
            return View(model);
        }

        // Methods
        private bool CheckInventorySpace(int addWeight)
        {
            try
            {
                using (var context = new SiteDbContext())
                {
                    // Get Space avaible
                    int inventorySpace = 10; // Default
                    int? bag = context.Characters
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r.BagInventoryId).FirstOrDefault();

                    if(bag != null && bag != 0)
                    {
                        int? inventoryItemId = context.Inventory
                            .Where(r => r.Id == bag)
                            .Select(r => r.ItemId).FirstOrDefault();
                        if(inventoryItemId == null) throw new ArgumentNullException("Error to Find item");

                        int? itemSpace = context.Items
                            .Where(r => r.Id == inventoryItemId)
                            .Select(r => r.InventorySpaceAdd).FirstOrDefault();
                        if (itemSpace == null) throw new ArgumentNullException("Error to Find item");

                        inventorySpace += (int)itemSpace;
                    }

                    // Calcul actual Weight
                    var AllItemsId = context.Inventory
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r.ItemId);

                    int totalWeight = 0;
                    foreach (var item in AllItemsId)
                    {
                        var tempItems = context.Items.Where(r => r.Id == item).Select(r => r.Weight).FirstOrDefault();
                        totalWeight += tempItems;
                    }

                    // Verify
                    if (totalWeight + addWeight <= inventorySpace)
                        return true;
                    else
                        return false;
                }
            }
            catch
            {
                return false;
            }
        }

        private int GetSkillPoints(int level)
        {
            if (level >= 2 && level <= 9)
                return 3;
            else if (level >= 10 && level <= 17)
                return 2;
            else if (level >= 18 && level <= 25)
                return 1;
            else
                return 0;
        }

        private void CheckCharacterLevel()
        {
            using (var context = new SiteDbContext())
            {
                var player = context.Characters
                    .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                    .Select(r => r).FirstOrDefault();

                int? levelUp = Util.GetXpforUp(player.Level);
                if (levelUp != null)
                {
                    if (player.Xp >= levelUp) // Update Level
                    {
                        if (player.Level < 25)
                        {
                            player.Xp -= (int)levelUp;
                            player.Level += 1;
                            player.SkillsPoints += GetSkillPoints(player.Level);

                            context.Update(player);
                            context.SaveChanges();
                        }
                        else
                            player.Xp = (int)levelUp;
                    }
                }
            }
        }

        // GET : GetModule
        public ActionResult GetModule(string partialName, string param1)
        {
            if(param1 == null)
                return PartialView("~/Views/Game/Shared/" + partialName);
            else
                return PartialView("~/Views/Game/Shared/" + partialName, param1);
        }

        // GET : GetModuleIndex
        public ActionResult GetModuleIndex(string partialName, string param1)
        {
            if (param1 == null)
                return PartialView("~/Views/Game/Index/" + partialName);
            else
                return PartialView("~/Views/Game/Index/" + partialName, param1);
        }
        
        // GET : AjaxGetLife
        public ActionResult AjaxGetLife()
        {
            try
            {
                using (var context = new SiteDbContext())
                {
                    var player = context.Characters
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r).FirstOrDefault();

                    return Json(new { Number = (player.Life * 100 / (100 + player.VitalityAdd)) });
                }
            }
            catch
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return Json(new { success = false });
            }
        }

        // GET : AjaxGetGold
        public ActionResult AjaxGetGold()
        {
            try
            {
                Gold gold;
                using (var context = new SiteDbContext())
                {
                    gold = context.Gold
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r).FirstOrDefault();
                }

                if (gold == null) throw new ArgumentNullException("Not At Work");
                return Json(new { Number = gold.Number });
            }
            catch
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return Json(new { success = false });
            }
        }

        // GET : AjaxGetArtifacts
        public ActionResult AjaxGetArtifacts()
        {
            try
            {
                Artifacts artifacts;
                using (var context = new SiteDbContext())
                {
                    artifacts = context.Artifacts
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r).FirstOrDefault();
                }

                if (artifacts == null) throw new ArgumentNullException("Not At Work");
                return Json(new { Number = artifacts.Number });
            }
            catch
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return Json(new { success = false });
            }
        }

        // GET : AjaxGetInventoryWeight
        public ActionResult AjaxGetInventoryWeight()
        {
            using (var context = new SiteDbContext())
            {
                // Get Space avaible
                int inventorySpace = 10; // Default
                int? bag = context.Characters
                    .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                    .Select(r => r.BagInventoryId).FirstOrDefault();

                if (bag != null && bag != 0)
                {
                    int? inventoryItemId = context.Inventory
                        .Where(r => r.Id == bag)
                        .Select(r => r.ItemId).FirstOrDefault();

                    if (inventoryItemId != null) { 
                        int? itemSpace = context.Items
                            .Where(r => r.Id == inventoryItemId)
                            .Select(r => r.InventorySpaceAdd).FirstOrDefault();

                        if(itemSpace != null)
                            inventorySpace += (int)itemSpace;
                    }
                }

                // Calcul actual Weight
                var AllItemsId = context.Inventory
                    .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                    .Select(r => r.ItemId);

                int totalWeight = 0;
                foreach (var item in AllItemsId)
                {
                    var tempItems = context.Items.Where(r => r.Id == item).Select(r => r.Weight).FirstOrDefault();
                    totalWeight += tempItems;
                }

                return Json(JsonConvert.DeserializeObject("[{ \"Space\" : \"" + inventorySpace + "\", \"Weight\" : \"" + totalWeight + "\" }]"));
            }
        }

        // GET : AjaxGetTotalCapabilities
        public ActionResult AjaxGetTotalCapabilities()
        {
            Characters player;
            using (var context = new SiteDbContext())
            {
                player = context.Characters
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r).FirstOrDefault();
            }

            return Json(new
            {
                Defense = player.Defense + player.DefenseAdd,
                Energy = player.Energy + player.EnergyAdd,
                Vitality = player.VitalityAdd,
                Strength = player.Strength + player.StrengthAdd
            });
        }

        /*************************************/
        /*            CHARACTER              */
        /*************************************/
        // GET : AjaxGetCharacter
        public ActionResult AjaxGetCharacter()
        {
            try
            {
                this.CheckCharacterLevel();
                string jsonString = "[{";
                using (var context = new SiteDbContext())
                {
                    var player = context.Characters
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r).FirstOrDefault();

                    // Xp & general data
                    jsonString += "\"Level\" : \"" + player.Level + "\"";
                    int? LevelUp = Util.GetXpforUp(player.Level);
                    jsonString += ",\"Xp\" : \"" + (player.Xp * 100 / (int)LevelUp) + "\"";
                    jsonString += ",\"Points\" : \"" + player.SkillsPoints + "\"";

                    // Update Skills Points
                    var buffList = context.Buff
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r).ToList();

                    player.DefenseAdd = 0;
                    player.EnergyAdd = 0;
                    player.VitalityAdd = 0;
                    player.StrengthAdd = 0;

                    // Potions
                    foreach (var buff in buffList)
                    {
                        var potion = context.Potions
                            .Where(r => r.Id == buff.PotionId)
                            .Select(r => r).FirstOrDefault();

                        if (DateTime.Now >= buff.Start + TimeSpan.FromMinutes(potion.Time))
                            context.Remove(buff);
                        else
                        {
                            player.DefenseAdd += potion.DefenseAdd;
                            player.EnergyAdd += potion.EnergyAdd;
                            player.VitalityAdd += potion.VitalityAdd;
                            player.StrengthAdd += potion.StrengthAdd;
                        }
                    }

                    // Stuff
                    Items tempItems;
                    int? tempsItemId;
                    if (player.WeaponInventoryId != null || player.WeaponInventoryId > 0)
                    {
                        tempsItemId = context.Inventory
                            .Where(r => r.Id == player.WeaponInventoryId)
                            .Select(r => r.ItemId).FirstOrDefault();

                        tempItems = context.Items
                            .Where(r => r.Id == tempsItemId)
                            .Select(r => r).FirstOrDefault();

                        if (tempItems != null)
                        {
                            player.DefenseAdd += tempItems.DefenseAdd;
                            player.EnergyAdd += tempItems.EnergyAdd;
                            player.VitalityAdd += tempItems.VitalityAdd;
                            player.StrengthAdd += tempItems.StrengthAdd;
                        }
                        else
                        {
                            player.WeaponInventoryId = 0;
                        }
                    }

                    if (player.ArmorInventoryId != null || player.ArmorInventoryId > 0)
                    {
                        tempsItemId = context.Inventory
                            .Where(r => r.Id == player.ArmorInventoryId)
                            .Select(r => r.ItemId).FirstOrDefault();

                        tempItems = context.Items
                            .Where(r => r.Id == tempsItemId)
                            .Select(r => r).FirstOrDefault();

                        if (tempItems != null)
                        {
                            player.DefenseAdd += tempItems.DefenseAdd;
                            player.EnergyAdd += tempItems.EnergyAdd;
                            player.VitalityAdd += tempItems.VitalityAdd;
                            player.StrengthAdd += tempItems.StrengthAdd;
                        }
                        else
                        {
                            player.ArmorInventoryId = 0;
                        }
                    }

                    if (player.ShieldInventoryId != null || player.ShieldInventoryId > 0)
                    {
                        tempsItemId = context.Inventory
                            .Where(r => r.Id == player.ShieldInventoryId)
                            .Select(r => r.ItemId).FirstOrDefault();

                        tempItems = context.Items
                            .Where(r => r.Id == tempsItemId)
                            .Select(r => r).FirstOrDefault();

                        if (tempItems != null)
                        {
                            player.DefenseAdd += tempItems.DefenseAdd;
                            player.EnergyAdd += tempItems.EnergyAdd;
                            player.VitalityAdd += tempItems.VitalityAdd;
                            player.StrengthAdd += tempItems.StrengthAdd;
                        }
                        else
                        {
                            player.ShieldInventoryId = 0;
                        }
                    }

                    if (player.BootsInventoryId != null || player.BootsInventoryId > 0)
                    {
                        tempsItemId = context.Inventory
                            .Where(r => r.Id == player.BootsInventoryId)
                            .Select(r => r.ItemId).FirstOrDefault();

                        tempItems = context.Items
                            .Where(r => r.Id == tempsItemId)
                            .Select(r => r).FirstOrDefault();

                        if (tempItems != null)
                        {
                            player.DefenseAdd += tempItems.DefenseAdd;
                            player.EnergyAdd += tempItems.EnergyAdd;
                            player.VitalityAdd += tempItems.VitalityAdd;
                            player.StrengthAdd += tempItems.StrengthAdd;
                        }
                        else
                        {
                            player.BootsInventoryId = 0;
                        }
                    }

                    if (player.JewelryInventoryId != null || player.JewelryInventoryId > 0)
                    {
                        tempsItemId = context.Inventory
                            .Where(r => r.Id == player.JewelryInventoryId)
                            .Select(r => r.ItemId).FirstOrDefault();

                        tempItems = context.Items
                            .Where(r => r.Id == tempsItemId)
                            .Select(r => r).FirstOrDefault();

                        if (tempItems != null)
                        {
                            player.DefenseAdd += tempItems.DefenseAdd;
                            player.EnergyAdd += tempItems.EnergyAdd;
                            player.VitalityAdd += tempItems.VitalityAdd;
                            player.StrengthAdd += tempItems.StrengthAdd;
                        }
                        else
                        {
                            player.JewelryInventoryId = 0;
                        }
                    }

                    // Security
                    if (player.DefenseAdd < 0)
                        player.DefenseAdd = 0;
                    if (player.EnergyAdd < 0)
                        player.EnergyAdd = 0;
                    if (player.VitalityAdd < 0)
                        player.VitalityAdd = 0;
                    if (player.StrengthAdd < 0)
                        player.StrengthAdd = 0;

                    // Life
                    if (player.Life > 100 + player.VitalityAdd)
                        player.Life = 100 + player.VitalityAdd;

                    context.Update(player);
                    context.SaveChanges();

                    jsonString += ",\"DefenseAdd\" : \"" + player.DefenseAdd + "\",\"EnergyAdd\" : \"" + player.EnergyAdd + "\",\"StrengthAdd\" : \"" + player.StrengthAdd + "\"";
                    jsonString += ",\"Defense\" : \"" + player.Defense + "\",\"Energy\" : \"" + player.Energy + "\",\"Strength\" : \"" + player.Strength + "\"";
                }
                jsonString += "}]";

                return Json(JsonConvert.DeserializeObject(jsonString));
            }
            catch
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return Json(new { success = false });
            }
        }

        // POST : AjaxAddSkill
        public ActionResult AjaxAddSkill(int? id)
        {
            try
            {
                if (id != null)
                {
                    using (var context = new SiteDbContext())
                    {
                        var character = context.Characters
                            .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                            .Select(r => r).FirstOrDefault();

                        if (character != null)
                        {
                            switch(id)
                            {
                                case 0:
                                    character.Strength++;
                                    break;
                                case 1:
                                    character.Defense++;
                                    break;
                                case 2:
                                    character.Energy++;
                                    break;
                            }
                            character.SkillsPoints--;

                            context.Update(character);
                            context.SaveChanges();
                        }
                        else throw new ArgumentNullException("In Work");
                    }

                    Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                    return Json(new { success = true });
                }
                else throw new ArgumentNullException("Id");
            }
            catch
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return Json(new { success = false });
            }
        }

        // POST : AjaxSelectSkill
        public ActionResult AjaxSelectSkill(int? id)
        {
            try
            {
                if (id == null) throw new ArgumentNullException("Internal Error: Id");
                using (var context = new SiteDbContext())
                {
                    var character = context.Characters
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r).FirstOrDefault();

                    if (character == null) throw new InvalidCastException("Internal Error: Character does not exist");
                    if(!(id >= 1 && id <= 9)) throw new InvalidCastException("Skill doesn't exist.");

                    // Remove 
                    if (character.FirstSkillId == (int)id || character.SecondSkillId == (int)id || character.ThirdSkillId == (int)id)
                    {
                        if (character.FirstSkillId == (int)id)
                        {
                            character.FirstSkillId = 0;
                        }
                        else if (character.SecondSkillId == (int)id)
                        {
                            character.SecondSkillId = 0;
                        }
                        else if (character.ThirdSkillId == (int)id)
                        {
                            character.ThirdSkillId = 0;
                        }
                    }
                    else // Add
                    {
                        if (character.FirstSkillId == 0)
                        {
                            character.FirstSkillId = (int)id;
                        }
                        else if (character.SecondSkillId == 0)
                        {
                            character.SecondSkillId = (int)id;
                        }
                        else if (character.ThirdSkillId == 0)
                        {
                            character.ThirdSkillId = (int)id;
                        }
                        else throw new InvalidCastException("You already have all your skills. Delete in one to add the new one.");
                    }

                    context.Update(character);
                    context.SaveChanges();
                }

                return Json(JsonConvert.DeserializeObject("[{\"Success\" : \"true\"}]"));
            }
            catch (InvalidCastException e)
            {
                string jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Error\",\"Msg\" : \"" + e.Message + "\" }]";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr")
                {
                    if (e.Message == "Skill doesn't exist.")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"La compétence n'existe pas.\" }]";
                    else if (e.Message == "You already have all your skills. Delete in one to add the new one.")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Vous avez déjà toutes vos compétences. Supprimer en une pour ajouter la nouvelle.\" }]";
                }

                return Json(JsonConvert.DeserializeObject(jsonString));
            }
        }

        /*************************************/
        /*               JOB                 */
        /*************************************/
        // POST : AjaxStartWork
        public ActionResult AjaxStartWork(int? id)
        {
            try
            {
                if (id == null) throw new ArgumentNullException("Internal Error: Id");
                using (var context = new SiteDbContext())
                {
                    var atWork = context.AtWork
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r).FirstOrDefault();

                    if (atWork != null) throw new InvalidCastException("You are already at work.");
                    atWork = new AtWork();
                    atWork.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    atWork.JobId = (int)id;
                    atWork.Start = DateTime.Now;

                    context.Add(atWork);
                    context.SaveChanges();
                }

                string jsonString = "[{";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr") // French
                    jsonString += "\"Success\" : \"true\",\"Title\" : \"Bravo\",\"Msg\" : \"Votre personnage va travailler!\", \"ButtonTxt\" : \"Récupérer les récompenses\", \"FinishedMsg\" : \"Vous êtes de retour de votre travail !\"";
                else // English
                    jsonString += "\"Success\" : \"true\",\"Title\" : \"Bravo\",\"Msg\" : \"Your character goes to work !\", \"ButtonTxt\" : \"Recover rewards\", \"FinishedMsg\" : \"You are back from your work !\"";
                jsonString += "}]";

                return Json(JsonConvert.DeserializeObject(jsonString));
            }
            catch (InvalidCastException e)
            {
                string jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Error\",\"Msg\" : \"" + e.Message + "\" }]";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr")
                {
                    if (e.Message == "You are already at work.")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Vous êtes déjà au travail.\" }]";
                }

                return Json(JsonConvert.DeserializeObject(jsonString));
            } 
        }

        // POST : AjaxAccelerateWork
        public ActionResult AjaxAccelerateWork()
        {
            try
            {
                using (var context = new SiteDbContext())
                {
                    var atWork = context.AtWork
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r).FirstOrDefault();

                    if (atWork == null) throw new InvalidCastException("You are not at work.");
                    else
                    {
                        var artifacts = context.Artifacts
                            .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                            .Select(r => r).FirstOrDefault();

                        if(artifacts == null) throw new InvalidCastException("Internal error: Artifacts");

                        var job = context.Jobs
                            .Where(r => r.Id == atWork.JobId)
                            .Select(r => r).FirstOrDefault();

                        if (job == null) throw new InvalidCastException("Internal error: Jobs");

                        int price = (int)Math.Ceiling(((TimeSpan.FromMinutes(job.Time).TotalSeconds - (DateTime.Now - atWork.Start).TotalSeconds) / 900) * 3);
                        if (price < 1)
                            price = 1;

                        if (artifacts.Number >= price)
                        {
                            artifacts.Number -= price;
                            atWork.Start = DateTime.Now.AddDays(-10);

                            context.Update(atWork);
                            context.Update(artifacts);
                            context.SaveChanges();
                        } else throw new InvalidCastException("You do not have enough artefacts, you can buy them in the shop !");
                    }
                }

                string jsonString = "[{";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr") // French
                    jsonString += "\"Success\" : \"true\", \"ButtonTxt\" : \"Récupérer les récompenses\"";
                else // English
                    jsonString += "\"Success\" : \"true\", \"ButtonTxt\" : \"Recover rewards\"";
                jsonString += "}]";

                return Json(JsonConvert.DeserializeObject(jsonString));
            }
            catch (InvalidCastException e)
            {
                string jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Error\",\"Msg\" : \"" + e.Message + "\" }]";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr")
                {
                    if(e.Message == "You are not at work.")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Vous n'êtes pas au travail.\" }]";
                    else if (e.Message == "You do not have enough artefacts, you can buy them in the shop !")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Vous n'avez pas assez d'artefact, vous pouvez en acheter dans la boutique !\" }]";
                }

                return Json(JsonConvert.DeserializeObject(jsonString));
            }
        }

        // POST : AjaxStopWork
        public ActionResult AjaxStopWork()
        {
            try
            {
                using (var context = new SiteDbContext())
                {
                    var atWork = context.AtWork
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r).FirstOrDefault();

                    if (atWork == null) throw new InvalidCastException("You are not at work");
                    context.Remove(atWork);
                    context.SaveChanges();
                }

                string jsonString = "[{";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr") // French
                    jsonString += "\"Success\" : \"true\", \"Title\" : \"Bravo\",\"Msg\" : \"Votre personnage revient !\"";
                else // English
                    jsonString += "\"Success\" : \"true\", \"Title\" : \"Bravo\",\"Msg\" : \"Your character come back !\"";
                jsonString += "}]";

                return Json(JsonConvert.DeserializeObject(jsonString));
            }
            catch (InvalidCastException e)
            {
                string jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Error\",\"Msg\" : \"" + e.Message + "\" }]";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr")
                {
                    if (e.Message == "You are not at work.")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Vous n'êtes pas au travail.\" }]";
                }

                return Json(JsonConvert.DeserializeObject(jsonString));
            }
        }

        // POST : AjaxRecoverWork
        public ActionResult AjaxRecoverWork()
        {
            try
            {
                using (var context = new SiteDbContext())
                {
                    var atWork = context.AtWork
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r).FirstOrDefault();

                    if (atWork == null) throw new ArgumentNullException("You can not get the rewards, you're not at work !");
                    var job = context.Jobs
                        .Where(r => r.Id == atWork.JobId)
                        .Select(r => r).FirstOrDefault();

                        if (job != null)
                        {
                            if (DateTime.Now >= atWork.Start + TimeSpan.FromMinutes(job.Time))
                            {
                                var gold = context.Gold
                                    .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                                    .Select(r => r).FirstOrDefault();

                                gold.Number += job.Gold;
                                context.Update(gold);
                                context.Remove(atWork);
                            } else throw new ArgumentNullException("The work is not finished yet.");
                        }
                        else
                        {
                            context.Remove(atWork);
                            throw new ArgumentNullException("Internal Error: Job Error");
                        }
                        context.SaveChanges();
                    
                }
                string jsonString = "[{";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr") // French
                    jsonString += "\"Success\" : \"true\",\"Title\" : \"Bravo\",\"Msg\" : \"Bon travail !\"";
                else // English
                    jsonString += "\"Success\" : \"true\",\"Title\" : \"Bravo\",\"Msg\" : \"Good job !\"";
                jsonString += "}]";

                return Json(JsonConvert.DeserializeObject(jsonString));
            }
            catch (InvalidCastException e)
            {
                string jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Error\",\"Msg\" : \"" + e.Message + "\" }]";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr")
                {
                    if (e.Message == "You can not get the rewards, you're not at work !")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Vous ne pouvez pas récupérer les récompenses, vous n'êtes pas au travail !\" }]";
                    else if(e.Message == "The work is not finished yet.")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Le travail n'est pas encore fini.\" }]";
                }

                return Json(JsonConvert.DeserializeObject(jsonString));
            }
        }

        // GET : AjaxGetWork
        public ActionResult AjaxGetWork()
        {
            try
            {
                AtWork atWork;
                using (var context = new SiteDbContext())
                {
                    atWork = context.AtWork
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r).FirstOrDefault();
                }

                if(atWork == null) throw new InvalidCastException("Not at Work");

                string jsonString = "[{";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr") // French
                    jsonString += "\"Success\" : \"true\",\"AtWorkStart\" : \"" + atWork.Start.ToString("yyyy-MM-dd HH:mm:ss") + "\",\"JobId\" : \"" + atWork.JobId + "\",\"ButtonTxt\" : \"Récupérer les récompenses\"";
                else // English
                    jsonString += "\"Success\" : \"true\",\"AtWorkStart\" : \"" + atWork.Start.ToString("yyyy-MM-dd HH:mm:ss") + "\",\"JobId\" : \"" + atWork.JobId + "\",\"ButtonTxt\" : \"Recover rewards\"";
                jsonString += "}]";

                return Json(JsonConvert.DeserializeObject(jsonString));
            }
            catch (InvalidCastException e)
            {
                string jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Error\",\"Msg\" : \"" + e.Message + "\" }]";
                return Json(JsonConvert.DeserializeObject(jsonString));
            }
        }

        // GET : AjaxGetHeroQuest
        public ActionResult AjaxGetHeroQuest()
        {
            try
            {
                HeroesQuestsState heroesQuestsState;
                using (var context = new SiteDbContext())
                {
                    heroesQuestsState = context.HeroesQuestsState
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && r.State != 0)
                        .Select(r => r).FirstOrDefault();
                }

                if (heroesQuestsState == null) throw new InvalidCastException("Internal Error: Not at quest");

                string jsonString = "[{";
                jsonString += "\"Success\" : \"true\",\"QuestId\" : \"" + heroesQuestsState.QuestId + "\"";
                jsonString += "}]";

                return Json(JsonConvert.DeserializeObject(jsonString));
            }
            catch (InvalidCastException e)
            {
                string jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Error\",\"Msg\" : \"" + e.Message + "\" }]";
                return Json(JsonConvert.DeserializeObject(jsonString));
            }
        }

        // GET : CreateCharacter
        public IActionResult CreateCharacter()
        {
            Characters model;
            using (var context = new SiteDbContext())
            {
                model = context.Characters
                    .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                    .Select(r => r).FirstOrDefault();

                var artifacts = context.Artifacts
                    .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                    .Select(r => r.Number).FirstOrDefault();
                ViewBag.Artifacts = artifacts;
            }



            if (model != null)
                return RedirectToAction(nameof(GameController.Index), "Game");

            return View();
        }

        /*************************************/
        /*         CreateCharacter           */
        /*************************************/
        // POST : CreateCharacter
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateCharacter(CreateCharacterViewModel model)
        {
            Characters character = new Characters()
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                AvatarId = Int32.Parse(model.AvatarId),
                Level = 1,
                Life = 100,
                Xp = 0,
                Strength = 1,
                Energy = 1, 
                Defense = 1,
                Register = DateTime.Now,
                FirstSkillId = 2,
                SecondSkillId = 5,
                ThirdSkillId = 8
            };

            using (var context = new SiteDbContext())
            {
                context.Add(character);
                context.SaveChanges();
            }

            return RedirectToAction(nameof(GameController.Index), "Game");
        }

        /*************************************/
        /*              SHOP                 */
        /*************************************/
        // GET : AjaxPurchaseItem
        public ActionResult AjaxPurchaseItem(int id)
        {
            try
            {
                using (var context = new SiteDbContext())
                {
                    var item = context.Items
                        .Where(r => r.Id == id)
                        .Select(r => r).FirstOrDefault();

                    if (item == null) throw new InvalidCastException("System error: Item doesn't exist");

                    var gold = context.Gold
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r).FirstOrDefault();

                    if (gold == null) throw new InvalidCastException("System error: Gold doesn't exist");

                    if (gold.Number >= item.Purchase)
                    {
                        if(!CheckInventorySpace(item.Weight)) throw new InvalidCastException("Your inventory is full. Bought a bag or empty it.");
                        gold.Number -= item.Purchase;

                        Inventory inventory = new Inventory()
                        {
                            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                            ItemId = (int)item.Id
                        };

                        context.Update(gold);
                        context.Add(inventory);
                        context.SaveChanges();
                    }
                    else throw new InvalidCastException("You do not have enough gold.");
                }

                string jsonString = "[{";
                if(CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr") // French
                    jsonString += "\"Success\" : \"true\",\"Title\" : \"Bravo\",\"Msg\" : \"Vous avez acheté un article avec succès !\"";
                else // English
                    jsonString += "\"Success\" : \"true\",\"Title\" : \"Bravo\",\"Msg\" : \"You bought an item successfully !\"";
                jsonString += "}]";

                return Json(JsonConvert.DeserializeObject(jsonString));
            }
            catch(InvalidCastException e)
            {
                string jsonString = "[{ \"Success\" : \"false\", \"Title\" : \"Error\",\"Msg\" : \"" + e.Message + "\" }]";
                
                if(CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr")
                {
                    if(e.Message == "Your inventory is full. Bought a bag or empty it.")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Votre inventaire est plein. Acheté un sac ou vider le.\" }]";
                    else if(e.Message == "You do not have enough gold.")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Vous n'avez pas assez d'or.\" }]";
                }
                return Json(JsonConvert.DeserializeObject(jsonString));
            }
        }

        // GET : AjaxPurchaseItemArtifact
        public ActionResult AjaxPurchaseItemArtifact(int id)
        {
            try
            {
                using (var context = new SiteDbContext())
                {
                    var item = context.Items
                        .Where(r => r.Id == id)
                        .Select(r => r).FirstOrDefault();

                    if (item == null) throw new InvalidCastException("System error: Item doesn't exist");
                    if (item.PurchaseArtifact <= 0) throw new InvalidCastException("System error: Item doesn't exist");

                    var artifacts = context.Artifacts
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r).FirstOrDefault();

                    if (artifacts == null) throw new InvalidCastException("System error: Artifact doesn't exist");

                    if (artifacts.Number >= item.PurchaseArtifact)
                    {
                        if (!CheckInventorySpace(item.Weight)) throw new InvalidCastException("Your inventory is full. Bought a bag or empty it.");
                        artifacts.Number -= item.PurchaseArtifact;

                        Inventory inventory = new Inventory()
                        {
                            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                            ItemId = (int)item.Id
                        };

                        context.Update(artifacts);
                        context.Add(inventory);
                        context.SaveChanges();
                    }
                    else throw new InvalidCastException("You do not have enough artifacts.");
                }

                string jsonString = "[{";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr") // French
                    jsonString += "\"Success\" : \"true\",\"Title\" : \"Bravo\",\"Msg\" : \"Vous avez acheté un article avec succès !\"";
                else // English
                    jsonString += "\"Success\" : \"true\",\"Title\" : \"Bravo\",\"Msg\" : \"You bought an item successfully !\"";
                jsonString += "}]";

                return Json(JsonConvert.DeserializeObject(jsonString));
            }
            catch (InvalidCastException e)
            {
                string jsonString = "[{ \"Success\" : \"false\", \"Title\" : \"Error\",\"Msg\" : \"" + e.Message + "\" }]";

                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr")
                {
                    if (e.Message == "Your inventory is full. Bought a bag or empty it.")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Votre inventaire est plein. Acheté un sac ou vider le.\" }]";
                    else if (e.Message == "You do not have enough artifacts.")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Vous n'avez pas assez d'artefacts.\" }]";
                }
                return Json(JsonConvert.DeserializeObject(jsonString));
            }
        }

        // GET : AjaxConvertArtifactToGold
        public ActionResult AjaxConvertArtifactToGold()
        {
            try
            {
                using (var context = new SiteDbContext())
                {
                    var artifacts = context.Artifacts
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r).FirstOrDefault();
                    if (artifacts == null) throw new InvalidCastException("System error: Artifact doesn't exist");

                    var gold = context.Gold
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r).FirstOrDefault();
                    if (gold == null) throw new InvalidCastException("System error: Gold doesn't exist");

                    if(artifacts.Number < 22) throw new InvalidCastException("You do not have enough artefacts, you can buy them in the shop !");
                    gold.Number += 100;
                    artifacts.Number -= 22;

                    context.Update(gold);
                    context.Update(artifacts);
                    context.SaveChanges();
                }

                string jsonString = "[{";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr") // French
                    jsonString += "\"Success\" : \"true\",\"Title\" : \"Bravo\",\"Msg\" : \"Vous avez réussi à échanger des artefacts pour de l'or !\"";
                else // English
                    jsonString += "\"Success\" : \"true\",\"Title\" : \"Bravo\",\"Msg\" : \"You have successfully exchanged artefacts for gold !\"";
                jsonString += "}]";

                return Json(JsonConvert.DeserializeObject(jsonString));
            }
            catch (InvalidCastException e)
            {
                string jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Error\",\"Msg\" : \"" + e.Message + "\" }]";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr")
                {
                    if (e.Message == "You do not have enough artefacts, you can buy them in the shop !")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Vous n'avez pas assez d'artefact, vous pouvez en acheter dans la boutique !\" }]";
                }

                return Json(JsonConvert.DeserializeObject(jsonString));
            }
        }

        // GET : AjaxSaleItem
        public ActionResult AjaxSaleItem(int id)
        {
            try
            {
                using (var context = new SiteDbContext())
                {
                    var inventory = context.Inventory
                        .Where(r => r.ItemId == id && r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r).FirstOrDefault();
                    if (inventory == null) throw new InvalidCastException("System error: Inventory doesn't exist");

                    var item = context.Items
                        .Where(r => r.Id == inventory.ItemId)
                        .Select(r => r).FirstOrDefault();
                    if (item == null) throw new InvalidCastException("System error: Item doesn't exist");

                    var gold = context.Gold
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r).FirstOrDefault();
                    if (gold == null) throw new InvalidCastException("System error: Gold doesn't exist");

                    gold.Number += item.Sale;

                    context.Update(gold);
                    context.Remove(inventory);
                    context.SaveChanges();
                }

                string jsonString = "[{";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr") // French
                    jsonString += "\"Success\" : \"true\",\"Title\" : \"Bravo\",\"Msg\" : \"Vous avez vendu votre article !\"";
                else // English
                    jsonString += "\"Success\" : \"true\",\"Title\" : \"Bravo\",\"Msg\" : \"You have sold your item !\"";
                jsonString += "}]";

                return Json(JsonConvert.DeserializeObject(jsonString));
            }
            catch (InvalidCastException e)
            {
                string jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Error\",\"Msg\" : \"" + e.Message + "\" }]";
                return Json(JsonConvert.DeserializeObject(jsonString));
            }
        }

        /*************************************/
        /*            APOTHECARY             */
        /*************************************/
        private int GetCarePrice(int level)
        {
            if (level > 0 && level <= 5)
                return 20;
            else if (level > 5 && level <= 10)
                return 30;
            else if (level > 10 && level <= 15)
                return 50;
            else if (level > 15 && level <= 20)
                return 75;
            else
                return 90;
        }

        // GET : AjaxGetCarePrice
        public ActionResult AjaxGetCarePrice()
        {
            int level;
            using (var context = new SiteDbContext())
            {
                level = context.Characters
                    .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                    .Select(r => r.Level).FirstOrDefault();
            }

            return Json(new { Price = GetCarePrice(level) });
        }

        // GET : AjaxGetCarePrice
        public ActionResult AjaxResetStats()
        {
            try
            {
                using (var context = new SiteDbContext())
                {
                    var player = context.Characters
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r).FirstOrDefault();
                    if (player == null) throw new InvalidCastException("Internal Error : Characters doesn't exist");

                    var artifacts = context.Artifacts
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r).FirstOrDefault();

                    if (artifacts.Number < 20) throw new InvalidCastException("You do not have enough artefacts, you can buy them in the shop !");
                    artifacts.Number -= 20;

                    player.SkillsPoints = 0;
                    for (int i = 0; i <= player.Level; i++) {
                        player.SkillsPoints += GetSkillPoints(i);
                    }

                    player.Defense = 1;
                    player.Energy = 0;
                    player.Strength = 1;

                    player.ArmorInventoryId = null;
                    player.BagInventoryId = null;
                    player.BootsInventoryId = null;
                    player.JewelryInventoryId = null;
                    player.ShieldInventoryId = null;
                    player.WeaponInventoryId = null;

                    context.Update(artifacts);
                    context.Update(player);
                    context.SaveChanges();
                }

                string jsonString = "[{";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr") // French
                    jsonString += "\"Success\" : \"true\",\"Title\" : \"Bravo\",\"Msg\" : \"Les statistiques de votre personnage ont été réinitialisées !\"";
                else // English
                    jsonString += "\"Success\" : \"true\",\"Title\" : \"Bravo\",\"Msg\" : \"Your character's statistics have been reset !\"";
                jsonString += "}]";

                return Json(JsonConvert.DeserializeObject(jsonString));
            }
            catch (InvalidCastException e)
            {
                string jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Error\",\"Msg\" : \"" + e.Message + "\" }]";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr")
                {
                    if (e.Message == "You do not have enough artefacts, you can buy them in the shop !")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Vous n'avez pas assez d'artéfacts, vous pouvez les acheter dans la boutique !\" }]";
                }
                return Json(JsonConvert.DeserializeObject(jsonString));
            }
        }

        // POST : AjaxCare
        public ActionResult AjaxCare()
        {
            try
            {
                using (var context = new SiteDbContext())
                {
                    var player = context.Characters
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r).FirstOrDefault();

                    if(player == null) throw new InvalidCastException("Internal Error : Characters doesn't exist");
                    int goldPrice = GetCarePrice(player.Level);

                    var gold = context.Gold
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r).FirstOrDefault();

                    if(gold.Number < goldPrice) throw new InvalidCastException("You do not have enough gold.");  
                    gold.Number -= goldPrice;
                    player.Life = 100 + player.VitalityAdd;

                    context.Update(gold);
                    context.Update(player);
                    context.SaveChanges();
                }

                string jsonString = "[{";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr") // French
                    jsonString += "\"Success\" : \"true\",\"Title\" : \"Bravo\",\"Msg\" : \"Votre personnage a la totalité de sa vie !\"";
                else // English
                    jsonString += "\"Success\" : \"true\",\"Title\" : \"Bravo\",\"Msg\" : \"Your character has the totality of her life!\"";
                jsonString += "}]";

                return Json(JsonConvert.DeserializeObject(jsonString));
            }
            catch (InvalidCastException e)
            {
                string jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Error\",\"Msg\" : \"" + e.Message + "\" }]";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr")
                {
                    if (e.Message == "You do not have enough gold.")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Vous n'avez pas assez d'or.\" }]";
                }
                return Json(JsonConvert.DeserializeObject(jsonString));
            }
        }

        // POST : AjaxPurchasePotion
        public ActionResult AjaxPurchasePotion(int id)
        {
            try
            {
                using (var context = new SiteDbContext())
                {
                    var potion = context.Potions
                        .Where(r => r.Id == id)
                        .Select(r => r).FirstOrDefault();

                    if (potion == null) throw new InvalidCastException("Internal error: Potion doesn't exist");

                    var gold = context.Gold
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r).FirstOrDefault();

                    if (gold.Number < potion.Purchase) throw new InvalidCastException("You do not have enough gold.");
                    gold.Number -= potion.Purchase;

                    Buff buff = new Models.GameViewModels.Buff()
                    {
                        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                        PotionId = (int)potion.Id,
                        Start = DateTime.Now
                    };

                    context.Add(buff);
                    context.Update(gold);
                    context.SaveChanges();
                }

                string jsonString = "[{";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr") // French
                    jsonString += "\"Success\" : \"true\",\"Title\" : \"Bravo\",\"Msg\" : \"L'effet est maintenant actif !\"";
                else // English
                    jsonString += "\"Success\" : \"true\",\"Title\" : \"Bravo\",\"Msg\" : \"The effect is now active !\"";
                jsonString += "}]";

                return Json(JsonConvert.DeserializeObject(jsonString));
            }
            catch (InvalidCastException e)
            {
                string jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Error\",\"Msg\" : \"" + e.Message + "\" }]";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr")
                {
                    if (e.Message == "You do not have enough gold.")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Vous n'avez pas assez d'or.\" }]";
                }
                return Json(JsonConvert.DeserializeObject(jsonString));
            }
        }

        /*************************************/
        /*            COMPANIONS             */
        /*************************************/
        // GET : AjaxPurchaseCompanion
        public async Task<ActionResult> AjaxPurchaseCompanion(int id)
        {
            try
            {
                using (var context = new SiteDbContext())
                {
                    var teamList = context.Teams
                        .Where(r => r.CompanionId == id && r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Count();

                    if (teamList > 0) throw new InvalidCastException("You already have this companion !");

                    var companion = context.Companions
                        .Where(r => r.Id == id)
                        .Select(r => r).FirstOrDefault();

                    if (companion == null) throw new InvalidCastException("Internal Error: Companion doesn't exist");

                    ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    var currentCompanionSlot = context.Teams
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Count();

                    if (currentCompanionSlot >= 5 + ((int?)user.SlotForCompanion ?? 0)) throw new InvalidCastException("You do not have enough companion space, buy it in the companion management area.");

                    var gold = context.Gold
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r).FirstOrDefault();

                    if (gold == null) throw new InvalidCastException("Internal Error: Gold doesn't exist");

                    if (gold.Number < companion.Purchase) throw new InvalidCastException("You do not have enough gold.");
                    gold.Number -= companion.Purchase;

                    Teams team = new Teams()
                    {
                        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                        CompanionId = (int)companion.Id,
                        Active = false
                    };

                    context.Update(gold);
                    context.Add(team);
                    context.SaveChanges();
                }

                string jsonString = "[{";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr") // French
                    jsonString += "\"Success\" : \"true\",\"Title\" : \"Bravo\",\"Msg\" : \"Vous avez acheté un compagnon avec succès !\"";
                else // English
                    jsonString += "\"Success\" : \"true\",\"Title\" : \"Bravo\",\"Msg\" : \"You bought an companion successfully !\"";
                jsonString += "}]";
                return Json(JsonConvert.DeserializeObject(jsonString));
            }
            catch (InvalidCastException e)
            {
                string jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Error\",\"Msg\" : \"" + e.Message + "\" }]";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr")
                {
                    if (e.Message == "You already have this companion !")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Vous avez déjà ce compagnon !\" }]";
                    else if(e.Message == "You do not have enough companion space, buy it in the companion management area.")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Vous n'avez plus d'espace pour compagnon, achetez-en dans l'espace de gestion des compagnons.\" }]";
                    else if (e.Message == "You do not have enough gold.")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Vous n'avez pas assez d'or.\" }]";
                }
                return Json(JsonConvert.DeserializeObject(jsonString));
            }
        }

        // POST : AjaxCompanionStartQuest
        public ActionResult AjaxCompanionStartQuest(int? teamId, int? questId)
        {
            try
            {
                if (!(teamId != null && questId != null)) throw new InvalidCastException("Internal Error: Id error");
                using (var context = new SiteDbContext())
                {
                    var onQuest = context.OnQuest
                        .Where(r => r.TeamsId == teamId)
                        .Select(r => r).FirstOrDefault();

                    if (onQuest != null) throw new InvalidCastException("Your companion is on mission now.");
                    onQuest = new OnQuest();
                    onQuest.TeamsId = (int)teamId;
                    onQuest.QuestId = (int)questId;
                    onQuest.Start = DateTime.Now;

                    context.Add(onQuest);
                    context.SaveChanges();
                }

                string jsonString = "[{";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr") // French
                    jsonString += "\"Success\" : \"true\",\"Title\" : \"Bravo\",\"Msg\" : \"Votre compagnon va en quête!\",\"BtnTxt\" : \"Récupérer les récompenses\"";
                else // English
                    jsonString += "\"Success\" : \"true\",\"Title\" : \"Bravo\",\"Msg\" : \"Your companion goes to quest !\",\"BtnTxt\" : \"Récupérer les récompenses\"";
                jsonString += "}]";
                return Json(JsonConvert.DeserializeObject(jsonString));
            }
            catch (InvalidCastException e)
            {
                string jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Error\",\"Msg\" : \"" + e.Message + "\" }]";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr")
                {
                    if (e.Message == "Your companion is on mission now.")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Votre compagnon est actuellement en mission.\" }]";
                }
                return Json(JsonConvert.DeserializeObject(jsonString));
            }
        }

        // GET : AjaxAddFavoriteCompanion
        public ActionResult AjaxAddFavoriteCompanion(int id)
        {
            try
            {
                using (var context = new SiteDbContext())
                {
                    int currentFavoriteCompanions = context.Teams
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && r.Active == true)
                        .Count();
                    if (currentFavoriteCompanions >= 5) throw new InvalidCastException("You already have the maximum number of companions reached.");

                    var companion = context.Teams
                        .Where(r => r.Id == id && r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r).FirstOrDefault();
                    if (companion == null) throw new InvalidCastException("Internal Error: Companion doesn't exist");

                    companion.Active = true;
                    context.Update(companion);
                    context.SaveChanges();
                }

                string jsonString = "[{";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr") // French
                    jsonString += "\"Success\" : \"true\",\"Title\" : \"Bravo\",\"Msg\" : \"Votre compagnon est maintenant dans les favoris !\",\"BtnText\" : \"Retirer des favoris\"";
                else // English
                    jsonString += "\"Success\" : \"true\",\"Title\" : \"Bravo\",\"Msg\" : \"Your companion is now in the favorites !\",\"BtnText\" : \"Remove from favorites\"";
                jsonString += "}]";
                return Json(JsonConvert.DeserializeObject(jsonString));
            }
            catch (InvalidCastException e)
            {
                string jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Error\",\"Msg\" : \"" + e.Message + "\" }]";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr")
                {
                    if (e.Message == "You already have the maximum number of companions reached.")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Vous avez déjà le nombre de compagnon maximum atteint.\" }]";
                }
                return Json(JsonConvert.DeserializeObject(jsonString));
            }
        }

        // GET : AjaxDeleteFavoriteCompanion
        public ActionResult AjaxDeleteFavoriteCompanion(int id)
        {
            try
            {
                using (var context = new SiteDbContext())
                {
                    var companion = context.Teams
                        .Where(r => r.Id == id && r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && r.Active == true)
                        .Select(r => r).FirstOrDefault();
                    if (companion == null) throw new InvalidCastException("Internal Error: Companion doesn't exist");

                    var onQuest = context.OnQuest
                        .Where(r => r.TeamsId == id)
                        .Count();
                    if(onQuest > 0) throw new InvalidCastException("Your companion is currently on a mission.");

                    companion.Active = false;
                    context.Update(companion);
                    context.SaveChanges();
                }

                string jsonString = "[{";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr") // French
                    jsonString += "\"Success\" : \"true\",\"Title\" : \"Bravo\",\"Msg\" : \"Votre compagnon est maintenant dans les favoris !\",\"BtnText\" : \"Ajouter aux Favoris\"";
                else // English
                    jsonString += "\"Success\" : \"true\",\"Title\" : \"Bravo\",\"Msg\" : \"Your companion is now in the favorites !\",\"BtnText\" : \"Add to Favorites\"";
                jsonString += "}]";
                return Json(JsonConvert.DeserializeObject(jsonString));
            }
            catch (InvalidCastException e)
            {
                string jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Error\",\"Msg\" : \"" + e.Message + "\" }]";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr")
                {
                    if (e.Message == "Your companion is currently on a mission.")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Votre compagnon est actuellement en mission.\" }]";
                }
                return Json(JsonConvert.DeserializeObject(jsonString));
            }
        }

        // GET : AjaxSendBackCompanion
        public ActionResult AjaxSendBackCompanion(int id)
        {
            try
            {
                using (var context = new SiteDbContext())
                {
                    var companion = context.Teams
                        .Where(r => r.Id == id && r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && r.Active == false)
                        .Select(r => r).FirstOrDefault();
                    if (companion == null) throw new InvalidCastException("Internal Error: Companion doesn't exist");

                    var onQuest = context.OnQuest
                        .Where(r => r.TeamsId == id)
                        .Count();
                    if (onQuest > 0) throw new InvalidCastException("Your companion is currently on a mission.");

                    context.Remove(companion);
                    context.SaveChanges();
                }

                string jsonString = "[{";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr") // French
                    jsonString += "\"Success\" : \"true\",\"Title\" : \"Bravo\",\"Msg\" : \"Vous avez renvoyé votre compagnon.\"";
                else // English
                    jsonString += "\"Success\" : \"true\",\"Title\" : \"Bravo\",\"Msg\" : \"You have returned your companion.\"";
                jsonString += "}]";
                return Json(JsonConvert.DeserializeObject(jsonString));
            }
            catch (InvalidCastException e)
            {
                string jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Error\",\"Msg\" : \"" + e.Message + "\" }]";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr")
                {
                    if (e.Message == "Your companion is currently on a mission.")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Votre compagnon est actuellement en mission.\" }]";
                }
                return Json(JsonConvert.DeserializeObject(jsonString));
            }
        }

        // POST : AjaxAccelerateCompanionQuest
        public ActionResult AjaxAccelerateCompanionQuest(int id)
        {
            try
            {
                using (var context = new SiteDbContext())
                {
                    var onQuest = context.OnQuest
                        .Where(r => r.TeamsId == id)
                        .Select(r => r).FirstOrDefault();

                    if (onQuest == null) throw new InvalidCastException("Your companion is not in quest.");
                    else
                    {
                        var artifacts = context.Artifacts
                            .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                            .Select(r => r).FirstOrDefault();
                        if (artifacts == null) throw new InvalidCastException("Internal error: Artifacts");

                        var quest = context.CompanionQuests
                            .Where(r => r.Id == onQuest.QuestId)
                            .Select(r => r).FirstOrDefault();
                        if (quest == null) throw new InvalidCastException("Internal error: Quests");

                        int price = (int)Math.Ceiling(((TimeSpan.FromMinutes(quest.Time).TotalSeconds - (DateTime.Now - onQuest.Start).TotalSeconds) / 900) * 3);
                        if (price < 1)
                            price = 1;

                        if (artifacts.Number >= price)
                        {
                            artifacts.Number -= price;
                            onQuest.Start = DateTime.Now.AddDays(-10);

                            context.Update(onQuest);
                            context.Update(artifacts);
                            context.SaveChanges();
                        }
                        else throw new InvalidCastException("You do not have enough artefacts, you can buy them in the shop !");
                    }
                }

                string jsonString = "[{";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr") // French
                    jsonString += "\"Success\" : \"true\", \"BtnNextStep\" : \"Étape suivante\"";
                else // English
                    jsonString += "\"Success\" : \"true\", \"BtnNextStep\" : \"Next Step\"";
                jsonString += "}]";

                return Json(JsonConvert.DeserializeObject(jsonString));
            }
            catch (InvalidCastException e)
            {
                string jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Error\",\"Msg\" : \"" + e.Message + "\" }]";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr")
                {
                    if (e.Message == "Your companion is not in quest.")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Votre compagnon n'est pas en quête.\" }]";
                    else if (e.Message == "You do not have enough artefacts, you can buy them in the shop !")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Vous n'avez pas assez d'artefact, vous pouvez en acheter dans la boutique !\" }]";
                }

                return Json(JsonConvert.DeserializeObject(jsonString));
            }
        }

        // GET : AjaxAddCompanionSlot
        public async Task<ActionResult> AjaxAddCompanionSlot()
        {
            try
            {
                using (var context = new SiteDbContext())
                {
                    var artifact = context.Artifacts
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r).First();

                    if(artifact.Number < 1) throw new InvalidCastException("You do not have enough artefacts, you can buy them in the shop !");
                    artifact.Number -= 1;

                    ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    if (user.SlotForCompanion == null)
                        user.SlotForCompanion = 0;
                    user.SlotForCompanion += 1;

                    await _userManager.UpdateAsync(user);
                    context.Update(artifact);
                    context.SaveChanges();
                }

                string jsonString = "[{";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr") // French
                    jsonString += "\"Success\" : \"true\",\"Title\" : \"Bravo\",\"Msg\" : \"Vous venez d'acheter un emplacement de compagnon supplémentaire !\"";
                else // English
                    jsonString += "\"Success\" : \"true\",\"Title\" : \"Bravo\",\"Msg\" : \"You just bought an additional companion slot !\"";
                jsonString += "}]";
                return Json(JsonConvert.DeserializeObject(jsonString));
            }
            catch (InvalidCastException e)
            {
                string jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Error\",\"Msg\" : \"" + e.Message + "\" }]";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr")
                {
                    if (e.Message == "You do not have enough artefacts, you can buy them in the shop !")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Vous n'avez pas assez d'artefact, vous pouvez en acheter dans la boutique !\" }]";
                }
                return Json(JsonConvert.DeserializeObject(jsonString));
            }
        }


        // GET : AjaxGetOnQuest
        public ActionResult AjaxGetOnQuest()
        {
            try
            {
                string btnTxt = "Recover rewards";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr")
                    btnTxt = "Récupérer les récompenses";

                string jsonString = "[";
                List<OnQuest> onQuest;
                using (var context = new SiteDbContext())
                {
                    var teams = context.Teams
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && r.Active == true)
                        .Select(r => r.Id).ToList();

                    onQuest = context.OnQuest
                        .Where(r => teams.Contains(r.TeamsId)) 
                        .Select(r => r).ToList();

                    // Ajax
                    int i = 0;
                    foreach(var quest in onQuest) {
                        var questTime = context.CompanionQuests
                            .Where(r => r.Id == quest.QuestId)
                            .Select(r => r.Time).FirstOrDefault();

                        if (i > 0) { jsonString += ","; }
                        jsonString += "{\"TeamId\":\"" + quest.TeamsId + "\", \"Start\":\"" + quest.Start.ToString("yyyy-MM-dd HH:mm:ss") + "\", \"Time\":\"" + TimeSpan.FromMinutes(questTime).TotalSeconds + "\",\"BtnText\" : \"" + btnTxt + "\"}";
                        i++;
                    }
                }
                jsonString += "]";

                Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                return Json(JsonConvert.DeserializeObject(jsonString));
            }
            catch
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return Json(new { success = false });
            }
        }

        // POST : AjaxRecoverCompanionQuest
        public ActionResult AjaxRecoverCompanionQuest(int id)
        {
            try
            {
                using (var context = new SiteDbContext())
                {
                    var teams = context.Teams
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && r.Active == true
                        && r.Id == id)
                        .Select(r => r).FirstOrDefault();

                    var onQuest = context.OnQuest
                        .Where(r => r.TeamsId == teams.Id)
                        .Select(r => r).FirstOrDefault();

                    if (onQuest == null) throw new InvalidCastException("Your companion is not currently on mission.");
                    var quest = context.CompanionQuests
                        .Where(r => r.Id == onQuest.QuestId)
                        .Select(r => r).FirstOrDefault();

                        if (quest != null)
                        {
                            if (!(DateTime.Now >= onQuest.Start + TimeSpan.FromMinutes(quest.Time))) throw new InvalidCastException("The work of your companion is not finished yet.");
                            var gold = context.Gold
                                .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                                .Select(r => r).FirstOrDefault();

                            var player = context.Characters
                                .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                                .Select(r => r).FirstOrDefault();

                           
                            if(new Random().Next(1, 100) > quest.Success)
                            {
                                context.Remove(onQuest);
                                context.SaveChanges();
                                throw new InvalidCastException("Your companion has failed the quest.");
                            }

                            player.Strength += quest.StrengthAdd;
                            player.Defense += quest.DefenseAdd;
                            player.Energy += quest.EnergyAdd;

                            // Items
                            if (quest.Items != null)
                            {
                                IList<string> items = quest.Items.Split(',').ToList();
                                foreach (string item in items)
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

                                        context.Add(inventory);
                                    }
                                }
                            }

                            // Potions
                            if (quest.Potions != null)
                            {
                                IList<string> potions = quest.Potions.Split(',').ToList();
                                foreach (string item in potions)
                                {
                                    var itemExist = context.Potions
                                        .Where(r => r.Id == Int32.Parse(item))
                                        .Select(r => r).FirstOrDefault();

                                    if (itemExist != null)
                                    {
                                        Buff buff = new Models.GameViewModels.Buff()
                                        {
                                            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                                            PotionId = (int)itemExist.Id,
                                            Start = DateTime.Now
                                        };

                                        context.Add(buff);
                                    }
                                }
                            }

                            // Care
                            if (quest.Care != 0)
                            {
                                int totalLife = player.Life + (int)((quest.Care * 0.01) * (100 + player.VitalityAdd));
                                if (totalLife < 100 + player.VitalityAdd)
                                    player.Life = totalLife;
                                else
                                    player.Life = 100 + player.VitalityAdd;
                            }

                            // Others
                            gold.Number += quest.Gold;
                            player.Xp += quest.Xp;

                            context.Update(gold);
                            context.Update(player);
                            context.Remove(onQuest);
                        }
                        else
                        {
                            context.Remove(onQuest);
                            throw new InvalidCastException("Internal Error: Job Error");
                        }
                        context.SaveChanges();
                }

                CheckCharacterLevel();

                string jsonString = "[{";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr") // French
                    jsonString += "\"Success\" : \"true\",\"Title\" : \"Bravo\",\"Msg\" : \"Bon travail !\"";
                else // English
                    jsonString += "\"Success\" : \"true\",\"Title\" : \"Bravo\",\"Msg\" : \"Good job !\"";
                jsonString += "}]";
                return Json(JsonConvert.DeserializeObject(jsonString));
            }
            catch (InvalidCastException e)
            {
                string jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Error\",\"Msg\" : \"" + e.Message + "\" }]";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr")
                {
                    if (e.Message == "Your companion is not currently on mission.")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Votre compagnon n'est pas actuellement en mission.\" }]";
                    else if(e.Message == "The work of your companion is not finished yet.")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Le travail de votre compagnon n'est pas encore fini.\" }]";
                    else if (e.Message == "The work of your companion is not finished yet.")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Votre compagnon a échoué la quête.\" }]";
                    else if (e.Message == "Your companion has failed the quest.")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Votre compagnon a échoué à la quête.\" }]";
                }
                return Json(JsonConvert.DeserializeObject(jsonString));
            }
        }

        /*************************************/
        /*             INVENTORY             */
        /*************************************/
        // GET : AjaxEquipItem
        public ActionResult AjaxEquipItem(int id)
        {
            try
            {
                using (var context = new SiteDbContext())
                {
                    var player = context.Characters
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r).FirstOrDefault();

                    if(player == null) throw new InvalidCastException("Internal error: Character not found");

                    Inventory inventoryItem = context.Inventory
                        .Where(r => r.Id == id && r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r).FirstOrDefault();

                    if (inventoryItem == null) throw new InvalidCastException("Internal error: Inventory not found");

                    Items item = context.Items
                        .Where(r => r.Id == inventoryItem.ItemId)
                        .Select(r => r).FirstOrDefault();

                    if (item == null) throw new InvalidCastException("Internal error: Item not found");

                    if (player.Defense < item.DefenseRequired) throw new InvalidCastException("You do not have enough defense.");
                    if (player.Energy < item.EnergyRequired) throw new InvalidCastException("You do not have enough energy.");
                    if (player.Strength < item.StrengthRequired) throw new InvalidCastException("You do not have enough strength.");

                    if (item.Type == 0)
                        player.WeaponInventoryId = inventoryItem.Id;
                    else if (item.Type == 1)
                        player.ArmorInventoryId = inventoryItem.Id;
                    else if (item.Type == 2)
                        player.ShieldInventoryId = inventoryItem.Id;
                    else if (item.Type == 3)
                        player.BootsInventoryId = inventoryItem.Id;
                    else if (item.Type == 4)
                        player.JewelryInventoryId = inventoryItem.Id;
                    else if (item.Type == 5)
                        player.BagInventoryId = inventoryItem.Id;

                    context.Update(player);
                    context.SaveChanges();
                }

                string jsonString = "[{";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr") // French
                    jsonString += "\"Success\" : \"true\",\"Title\" : \"Bravo\",\"Msg\" : \"Votre objet est maintenant équipé !\"";
                else // English
                    jsonString += "\"Success\" : \"true\",\"Title\" : \"Bravo\",\"Msg\" : \"Your item is now equipped !\"";
                jsonString += "}]";
                return Json(JsonConvert.DeserializeObject(jsonString));
            }
            catch (InvalidCastException e)
            {
                string jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Error\",\"Msg\" : \"" + e.Message + "\" }]";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr")
                {
                    if (e.Message == "You do not have enough defense.")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Vous n'avez pas assez de défense.\" }]";
                    else if (e.Message == "You do not have enough dexterity.")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Vous n'avez pas assez de dextérité.\" }]";
                    else if (e.Message == "You do not have enough stamina.")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Vous n'avez pas assez d'endurance.\" }]";
                    else if (e.Message == "You do not have enough strength.")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Vous n'avez pas assez de force.\" }]";
                }
                return Json(JsonConvert.DeserializeObject(jsonString));
            }
        }

        // GET : AjaxUnequipItem
        public ActionResult AjaxUnequipItem(int id)
        {
            try
            {
                using (var context = new SiteDbContext())
                {
                    var player = context.Characters
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r).FirstOrDefault();

                    if (player == null) throw new ArgumentNullException("Character not found");

                    if (id == 0)
                        player.WeaponInventoryId = null;
                    else if (id == 1)
                        player.ArmorInventoryId = null;
                    else if (id == 2)
                        player.ShieldInventoryId = null;
                    else if (id == 3)
                        player.BootsInventoryId = null;
                    else if (id == 4)
                        player.JewelryInventoryId = null;
                    else if (id == 5)
                        player.BagInventoryId = null;

                    context.Update(player);
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
        /*            HERO QUEST             */
        /*************************************/
        // GET : AjaxGetNextStep
        public ActionResult AjaxGetNextStep(int id)
        {
            try
            {
                string jsonString = "[";
                using (var context = new SiteDbContext())
                {
                    var player = context.Characters
                            .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                            .Select(r => r).FirstOrDefault();

                    var heroLastQuest = context.HeroesQuestsState
                        .OrderByDescending(r => r.Id)
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && r.State != 0)
                        .Select(r => r).FirstOrDefault();

                    // Ajax
                    if(heroLastQuest == null) throw new InvalidCastException("Internal Error: Quest not found");
                    var questData = context.HeroQuests
                        .Where(r => r.Id == heroLastQuest.QuestId)
                        .Select(r => r).First();

                    var atBattlePVE = context.AtBattlePVE
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r).FirstOrDefault();

                    if (id == 2 && heroLastQuest.State == 1)
                    {
                        if (DateTime.Now >= heroLastQuest.StartTimer + TimeSpan.FromMinutes(questData.Time))
                        {
                            heroLastQuest.State = 2;
                            if (questData.MonsterId == null)
                                heroLastQuest.State = 3;
                        }
                    }
                    else if(id == 3 && heroLastQuest.State == 2 && atBattlePVE != null)
                    {
                        if (atBattlePVE.EntityLife <= atBattlePVE.UserLife)
                        {
                            if (atBattlePVE.UserLife > 0)
                                player.Life = atBattlePVE.UserLife;
                            else
                                player.Life = 1;

                            Hunt_statistics hunt = new Hunt_statistics()
                            {
                                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                                EntityId = atBattlePVE.EntityId,
                                Date = DateTime.Now
                            };

                            context.Add(hunt);
                            context.Remove(atBattlePVE);
                            heroLastQuest.State = 3;
                        }
                        else
                        {
                            player.Life = 1;
                            player.Death += 1;
                            context.Remove(atBattlePVE);
                            heroLastQuest.State = -1;
                        }
                    }
                    else if(id == 4 && heroLastQuest.State == 3)
                    {
                        heroLastQuest.State = 0;
                        var gold = context.Gold
                                    .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                                    .Select(r => r).FirstOrDefault();

                        gold.Number += questData.Gold;
                        player.Xp += questData.Xp;

                        // Items
                        if (questData.Items != null)
                        {
                            IList<string> items = questData.Items.Split(',').ToList();
                            foreach (string item in items)
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

                                    context.Add(inventory);
                                }
                            }
                        }

                        // Potions
                        if (questData.Potions != null)
                        {
                            IList<string> potions = questData.Potions.Split(',').ToList();
                            foreach (string item in potions)
                            {
                                var itemExist = context.Potions
                                    .Where(r => r.Id == Int32.Parse(item))
                                    .Select(r => r).FirstOrDefault();

                                if (itemExist != null)
                                {
                                    Buff buff = new Models.GameViewModels.Buff()
                                    {
                                        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                                        PotionId = (int)itemExist.Id,
                                        Start = DateTime.Now
                                    };

                                    context.Add(buff);
                                }
                            }
                        }

                        context.Update(gold);
                        context.Update(player);
                    }

                    jsonString += "{\"Step\":\"" + heroLastQuest.State + "\"";

                    if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr")
                    {
                        jsonString += ",\"BtnNextStep\":\"Étape suivante\",\"BtnGetReward\":\"Obtenir la récompense\"";
                    }
                    else
                    {
                        jsonString += ",\"BtnNextStep\":\"Next step\",\"BtnGetReward\":\"Get Reward\"";
                    }

                    switch (heroLastQuest.State)
                    {
                        case (-1):
                            jsonString += "}";
                            break;
                        case 1:
                            jsonString += ",\"Time\":\"" + TimeSpan.FromMinutes(questData.Time).TotalSeconds + "\"";
                            if (heroLastQuest.StartTimer == null) 
                                heroLastQuest.StartTimer = DateTime.Now;

                            jsonString += ",\"Start\":\"" + heroLastQuest.StartTimer.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss") + "\"";
                            if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr")
                            {
                                jsonString += ",\"BtnFinished\":\"Terminer maintenant\",\"ArtifactTxt\":\"Artefacts\"";
                            }
                            else
                            {
                                jsonString += ",\"BtnFinished\":\"Finish now\",\"ArtifactTxt\":\"Artifacts\"";
                            }
                            jsonString += "}";
                            break;
                        case 2:
                            if (atBattlePVE == null)
                            {
                                var currentBattle = context.AtBattlePVE
                                    .Where(r => r.UserId == player.UserId)
                                    .Count();

                                if (currentBattle == 0)
                                {
                                    var monster = context.Monsters
                                        .Where(r => r.Id == (int)questData.MonsterId)
                                        .Select(r => r).First();

                                    AtBattlePVE atBattlePVEAdd = new AtBattlePVE()
                                    {
                                        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                                        UserLife = player.Life,
                                        UserStrength = player.Strength,
                                        UserDefense = player.Defense,
                                        UserEnergy = player.Energy,
                                        UserVitalityAdd = player.VitalityAdd,
                                        UserStrengthAdd = player.StrengthAdd,
                                        UserDefenseAdd = player.DefenseAdd,
                                        UserEnergyAdd = player.EnergyAdd,
                                        UserFirstSkillId = player.FirstSkillId,
                                        UserSecondSkillId = player.SecondSkillId,
                                        UserThirdSkillId = player.ThirdSkillId,
                                        UserCurrentEnergy = 90 + ((player.Energy + player.EnergyAdd) * 10),
                                        EntityId = (int)monster.Id,
                                        EntityLife = monster.Life,
                                        EntityCurrentEnergy = 90 + (monster.Energy * 10),
                                        EntityCurrentPatternId = 0,
                                        TourNumber = 0
                                    };
                                    context.Add(atBattlePVEAdd);
                                }
                            }
                            jsonString += "}";
                            break;
                        case 3:
                            if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr")
                                jsonString += ",\"GoldTxt\":\"Or\",\"XpTxt\":\"Expérience\"";
                            else
                                jsonString += ",\"GoldTxt\":\"Gold\",\"XpTxt\":\"Experience\"";

                            // Items & Potions
                            IList<string> items = null;
                            IList<string> potions = null;

                            int ItemNumber = 0;
                            if (questData.Items != null) {
                                items = questData.Items.Split(',').ToList();
                                ItemNumber += items.Count;
                            }

                            if (questData.Potions != null)
                            {
                                potions = questData.Potions.Split(',').ToList();
                                ItemNumber += potions.Count;
                            }

                            jsonString += ",\"ItemNumber\":\"" + ItemNumber + "\", \"ItemArray\":[";
                            int i = 0;
                            if (questData.Items != null)
                            {
                                foreach (string item in items) // Items
                                {
                                    var itemExist = context.Items
                                        .Where(r => r.Id == Int32.Parse(item))
                                        .Select(r => r).FirstOrDefault();

                                    if (i != 0 && i < ItemNumber)
                                        jsonString += ",";

                                    if (itemExist != null)
                                    {
                                        if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr")
                                            jsonString += "{\"Title\":\"" + itemExist.TitleFr + "\", \"Text\":\"Vous trouverez l'objet dans votre inventaire.\", \"Img\":\"/uploads/game/cards/items/" + itemExist.Id + ".fr.svg\"}";
                                        else
                                            jsonString += "{\"Title\":\"" + itemExist.TitleEn + "\", \"Text\":\"You will find the item in your inventory.\", \"Img\":\"/uploads/game/cards/items/" + itemExist.Id + ".en.svg\"}";
                                    }
                                    i++;
                                }
                            }

                            if (questData.Potions != null)
                            {
                                foreach (string item in potions) // Items
                                {
                                    var itemExist = context.Potions
                                        .Where(r => r.Id == Int32.Parse(item))
                                        .Select(r => r).FirstOrDefault();

                                    if (i != 0 && i < ItemNumber)
                                        jsonString += ",";

                                    if (itemExist != null)
                                    {
                                        jsonString += "{\"Title\":\"" + Util.LanguageSelection(itemExist.TitleEn, itemExist.TitleFr) + "\", \"Text\":\""+  Util.LanguageSelection("The effect of the potion is now activated", "L'effet de la potion est maintenant activé.") + "\", \"Img\":\"none\"}";
                                    }
                                    i++;
                                }
                            }
                            jsonString += "]";
                            jsonString += ",\"Gold\":\"" + questData.Gold + "\",\"Xp\":\"" + questData.Xp + "\"}";
                            break;
                        default:
                            jsonString += "}";
                            break;
                    }

                    if(heroLastQuest.State == -1)
                        context.Remove(heroLastQuest);
                    else
                        context.Update(heroLastQuest);
                    context.SaveChanges();
                }
                jsonString += "]";
                return Json(JsonConvert.DeserializeObject(jsonString));
            }
            catch (InvalidCastException e)
            {
                string jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Error\",\"Msg\" : \"" + e.Message + "\" }]";
                return Json(JsonConvert.DeserializeObject(jsonString));
            }
        }

        // POST : AjaxAccelerateHeroQuest
        public ActionResult AjaxAccelerateHeroQuest()
        {
            try
            {
                using (var context = new SiteDbContext())
                {
                    var atQuest = context.HeroesQuestsState
                        .OrderByDescending(r => r.Id)
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r).FirstOrDefault();

                    if (atQuest == null) throw new InvalidCastException("You are not at hero quest.");
                    else
                    {
                        var artifacts = context.Artifacts
                            .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                            .Select(r => r).FirstOrDefault();

                        if (artifacts == null) throw new InvalidCastException("Internal error: Artifacts");

                        var quest = context.HeroQuests
                            .Where(r => r.Id == atQuest.QuestId)
                            .Select(r => r).FirstOrDefault();

                        if (quest == null) throw new InvalidCastException("Internal error: Quests");

                        int price = (int)Math.Ceiling(((TimeSpan.FromMinutes(quest.Time).TotalSeconds - (DateTime.Now - atQuest.StartTimer).GetValueOrDefault().TotalSeconds) / 900) * 3);
                        if (price < 1)
                            price = 1;

                        if (artifacts.Number >= price)
                        {
                            artifacts.Number -= price;
                            atQuest.StartTimer = DateTime.Now.AddDays(-10);

                            context.Update(atQuest);
                            context.Update(artifacts);
                            context.SaveChanges();
                        }
                        else throw new InvalidCastException("You do not have enough artefacts, you can buy them in the shop !");
                    }
                }

                string jsonString = "[{";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr") // French
                    jsonString += "\"Success\" : \"true\", \"BtnNextStep\" : \"Étape suivante\"";
                else // English
                    jsonString += "\"Success\" : \"true\", \"BtnNextStep\" : \"Next Step\"";
                jsonString += "}]";

                return Json(JsonConvert.DeserializeObject(jsonString));
            }
            catch (InvalidCastException e)
            {
                string jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Error\",\"Msg\" : \"" + e.Message + "\" }]";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr")
                {
                    if (e.Message == "You are not at work.")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Vous n'êtes pas dans une quête de héro.\" }]";
                    else if (e.Message == "You do not have enough artefacts, you can buy them in the shop !")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Vous n'avez pas assez d'artefact, vous pouvez en acheter dans la boutique !\" }]";
                }

                return Json(JsonConvert.DeserializeObject(jsonString));
            }
        }

        /*************************************/
        /*          NARRATIVE QUEST          */
        /*************************************/
        // GET : AjaxNarrativeGetNextStep
        public ActionResult AjaxNarrativeGetNextStep(int? id)
        {  
            try
            {
                string jsonString = "[{";
                using (var context = new SiteDbContext())
                {
                    var player = context.Characters
                            .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                            .Select(r => r).FirstOrDefault();

                    var currentQuest = context.AtNarrativeQuest
                        .OrderByDescending(r => r.Id)
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r).FirstOrDefault();

                    // Ajax
                    if (currentQuest == null) throw new InvalidCastException("Internal Error: At quest not found");
                    var questData = context.NarrativeQuest
                        .Where(r => r.Id == currentQuest.QuestId)
                        .Select(r => r).First();
                    if (questData == null) throw new InvalidCastException("Internal Error: Quest data not found");

                    string[] questStep = questData.StepList.Replace(" ", "").Split(',');

                    if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr") {
                        jsonString += "\"BtnNextStep\":\"Étape suivante\",\"BtnGetReward\":\"Obtenir la récompense\"";
                    }
                    else {
                        jsonString += "\"BtnNextStep\":\"Next step\",\"BtnGetReward\":\"Get Reward\"";
                    }

                    int i = -12;
                    if (currentQuest.Step != -1)
                    {
                        for (i = 0; i < questStep.Length; i++) {
                            if (Int32.Parse(questStep[i]) == currentQuest.Step) { break; }
                        }
                    }

                    if (currentQuest.Step == -10)
                        currentQuest.Step = -11;

                    switch (currentQuest.Step)
                    {
                        case -1:
                            if (currentQuest.StartTimer == null)
                                currentQuest.StartTimer = DateTime.Now;

                            currentQuest.Step = 0;
                            jsonString += ",\"Time\":\"" + TimeSpan.FromMinutes(questData.Time).TotalSeconds + "\"";
                            jsonString += ",\"Start\":\"" + currentQuest.StartTimer.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss") + "\"";

                            if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr")
                            {
                                jsonString += ",\"BtnFinished\":\"Terminer maintenant\",\"ArtifactTxt\":\"Artefacts\"";
                            }
                            else
                            {
                                jsonString += ",\"BtnFinished\":\"Finish now\",\"ArtifactTxt\":\"Artifacts\"";
                            }
                            break;
                        case 0:
                            if (DateTime.Now >= currentQuest.StartTimer + TimeSpan.FromMinutes(questData.Time))
                            {
                                i = -1;
                                goto default;
                            }
                            else
                                goto case -1;
                            break;
                        case -10:
                            if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr")
                                jsonString += ",\"GoldTxt\":\"Or\",\"XpTxt\":\"Expérience\"";
                            else
                                jsonString += ",\"GoldTxt\":\"Gold\",\"XpTxt\":\"Experience\"";

                            // Items & Potions
                            IList<string> items = null;
                            IList<string> potions = null;

                            int ItemNumber = 0;
                            if (questData.Items != null)
                            {
                                items = questData.Items.Split(',').ToList();
                                ItemNumber += items.Count;
                            }

                            if (questData.Potions != null)
                            {
                                potions = questData.Potions.Split(',').ToList();
                                ItemNumber += potions.Count;
                            }

                            jsonString += ",\"ItemNumber\":\"" + ItemNumber + "\", \"ItemArray\":[";
                            int y = 0;
                            if (questData.Items != null)
                            {
                                foreach (string item in items) // Items
                                {
                                    var itemExist = context.Items
                                        .Where(r => r.Id == Int32.Parse(item))
                                        .Select(r => r).FirstOrDefault();

                                    if (y != 0 && y < ItemNumber)
                                        jsonString += ",";

                                    if (itemExist != null)
                                    {
                                        if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr")
                                            jsonString += "{\"Title\":\"" + itemExist.TitleFr + "\", \"Text\":\"Vous trouverez l'objet dans votre inventaire.\", \"Img\":\"/uploads/game/cards/items/" + itemExist.Id + ".fr.svg\"}";
                                        else
                                            jsonString += "{\"Title\":\"" + itemExist.TitleEn + "\", \"Text\":\"You will find the item in your inventory.\", \"Img\":\"/uploads/game/cards/items/" + itemExist.Id + ".en.svg\"}";
                                    }
                                    i++;
                                }
                            }

                            if (questData.Potions != null)
                            {
                                foreach (string item in potions) // Items
                                {
                                    var itemExist = context.Potions
                                        .Where(r => r.Id == Int32.Parse(item))
                                        .Select(r => r).FirstOrDefault();

                                    if (i != 0 && i < ItemNumber)
                                        jsonString += ",";

                                    if (itemExist != null)
                                    {
                                        jsonString += "{\"Title\":\"" + Util.LanguageSelection(itemExist.TitleEn, itemExist.TitleFr) + "\", \"Text\":\"" + Util.LanguageSelection("The effect of the potion is now activated", "L'effet de la potion est maintenant activé.") + "\", \"Img\":\"none\"}";
                                    }
                                    i++;
                                }
                            }
                            jsonString += "]";
                            jsonString += ",\"Gold\":\"" + questData.Gold + "\",\"Xp\":\"" + questData.Xp + "\"";
                            break;
                        case -11:
                            var gold = context.Gold
                                .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                                .Select(r => r).FirstOrDefault();

                            gold.Number += questData.Gold;
                            player.Xp += questData.Xp;

                            // Items
                            if (questData.Items != null)
                            {
                                IList<string> itemsList = questData.Items.Split(',').ToList();
                                foreach (string item in itemsList)
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

                                        context.Add(inventory);
                                    }
                                }
                            }

                            // Potions
                            if (questData.Potions != null)
                            {
                                IList<string> potionsList = questData.Potions.Split(',').ToList();
                                foreach (string item in potionsList)
                                {
                                    var itemExist = context.Potions
                                        .Where(r => r.Id == Int32.Parse(item))
                                        .Select(r => r).FirstOrDefault();

                                    if (itemExist != null)
                                    {
                                        Buff buff = new Models.GameViewModels.Buff()
                                        {
                                            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                                            PotionId = (int)itemExist.Id,
                                            Start = DateTime.Now
                                        };

                                        context.Add(buff);
                                    }
                                }

                                player.NarrativeQuestStep = (int)questData.Id;
                            }

                            context.Remove(currentQuest);
                            context.Update(gold);
                            context.Update(player);
                            break;
                        case -12:
                            context.Remove(currentQuest);
                            break;
                        default:
                            bool stepFinished = false;
                            bool firstTransition = false;
                            if (currentQuest.Step == 0 && i == -1) {
                                i = 0;
                                currentQuest.Step = Int32.Parse(questStep[i]);
                                firstTransition = true;
                            }

                            if (i >= 0)
                            {
                                var currentStepData = context.NarrativeQuestStep
                                    .Where(r => r.Id == currentQuest.Step)
                                    .Select(r => r).FirstOrDefault();

                                if (id != null)
                                {
                                    switch(id)
                                    {
                                        case 1:
                                            id = currentStepData.ChoiceOneIncrease;
                                            break;
                                        case 2:
                                            id = currentStepData.ChoiceTwoIncrease;
                                            break;
                                        case 3:
                                            id = currentStepData.ChoiceThreeIncrease;
                                            break;
                                    }
                                }
                                else
                                    id = 1;

                                if (currentStepData.Type == 2)
                                {
                                    var atBattlePVE = context.AtBattlePVE
                                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                                        .Select(r => r).FirstOrDefault();

                                    if (atBattlePVE != null)
                                    {
                                        if (atBattlePVE.EntityLife <= 0)
                                        {
                                            if (atBattlePVE.UserLife > 0)
                                                player.Life = atBattlePVE.UserLife;
                                            else
                                                player.Life = 1;

                                            Hunt_statistics hunt = new Hunt_statistics()
                                            {
                                                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                                                EntityId = atBattlePVE.EntityId,
                                                Date = DateTime.Now
                                            };
                                            context.Add(hunt);
                                            context.Remove(atBattlePVE);

                                            if (i + 1 < questStep.Length)
                                                currentQuest.Step = Int32.Parse(questStep[i + (int)currentStepData.BattleIncrease]);
                                            else
                                            {
                                                currentQuest.Step = -10;
                                                goto case -10;
                                            }
                                        }
                                        else if (atBattlePVE.UserLife <= 0)
                                        {
                                            player.Life = 1;
                                            player.Death += 1;
                                            context.Remove(atBattlePVE);
                                            currentQuest.Step = -11;
                                            stepFinished = true;
                                        }
                                    }
                                    else { currentQuest.Step = -11; stepFinished = true; }
                                }
                                else if(!firstTransition)
                                {
                                    if (i + (int)id > 0 && i + (int)id < questStep.Length)
                                        currentQuest.Step = Int32.Parse(questStep[i + (int)id]);
                                    else if (i + (int)id < 0)
                                        currentQuest.Step = currentQuest.Step;
                                    else
                                    {
                                        currentQuest.Step = -10;
                                        goto case -10;
                                    }
                                }

                                if (stepFinished == false)
                                {
                                    var stepData = context.NarrativeQuestStep
                                        .Where(r => r.Id == currentQuest.Step)
                                        .Select(r => r).FirstOrDefault();

                                    if (stepData == null) throw new InvalidCastException("Internal Error: At quest not found");
                                    jsonString += ",\"Type\":\"" + stepData.Type + "\"";
                                    if (stepData.Type == 2) // Battle
                                    {
                                        var monster = context.Monsters
                                                .Where(r => r.Id == (int)stepData.MonsterId)
                                                .Select(r => r).First();


                                        AtBattlePVE atBattlePVEAdd = new AtBattlePVE()
                                        {
                                            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                                            UserLife = player.Life,
                                            UserStrength = player.Strength,
                                            UserDefense = player.Defense,
                                            UserEnergy = player.Energy ,
                                            UserVitalityAdd = player.VitalityAdd,
                                            UserStrengthAdd = player.StrengthAdd,
                                            UserDefenseAdd = player.DefenseAdd,
                                            UserEnergyAdd = player.EnergyAdd,
                                            UserFirstSkillId = player.FirstSkillId,
                                            UserSecondSkillId = player.SecondSkillId,
                                            UserThirdSkillId = player.ThirdSkillId,
                                            UserCurrentEnergy = 90 + ((player.Energy + player.EnergyAdd) * 10),
                                            EntityId = (int)monster.Id,
                                            EntityLife = monster.Life,
                                            EntityCurrentEnergy = 90 + (monster.Energy * 10),
                                            EntityCurrentPatternId = 0,
                                            TourNumber = 0
                                        };
                                        context.Add(atBattlePVEAdd);
                                    }
                                }
                            }
                            break;
                    }

                    if(currentQuest != null)
                        jsonString += ",\"Step\":\"" + currentQuest.Step + "\"";

                    context.Update(currentQuest);
                    context.SaveChanges();
                }

                jsonString += "}]";
                return Json(JsonConvert.DeserializeObject(jsonString));
            }
            catch (InvalidCastException e)
            {
                string jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Error\",\"Msg\" : \"" + e.Message + "\" }]";
                return Json(JsonConvert.DeserializeObject(jsonString));
            }
        }

        // POST : AjaxAccelerateNarrativeQuest
        public ActionResult AjaxAccelerateNarrativeQuest()
        {
            try
            {
                using (var context = new SiteDbContext())
                {
                    var atQuest = context.AtNarrativeQuest
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r).FirstOrDefault();

                    if (atQuest == null) throw new InvalidCastException("You are not at quest.");
                    else
                    {
                        var artifacts = context.Artifacts
                            .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                            .Select(r => r).FirstOrDefault();

                        if (artifacts == null) throw new InvalidCastException("Internal error: Artifacts");

                        var quest = context.NarrativeQuest
                            .Where(r => r.Id == atQuest.QuestId)
                            .Select(r => r).FirstOrDefault();

                        if (quest == null) throw new InvalidCastException("Internal error: Quests");

                        int price = (int)Math.Ceiling(((TimeSpan.FromMinutes(quest.Time).TotalSeconds - (DateTime.Now - atQuest.StartTimer).GetValueOrDefault().TotalSeconds) / 900) * 3);
                        if (price < 1)
                            price = 1;

                        if (artifacts.Number >= price)
                        {
                            artifacts.Number -= price;
                            atQuest.StartTimer = DateTime.Now.AddDays(-10);

                            context.Update(atQuest);
                            context.Update(artifacts);
                            context.SaveChanges();
                        }
                        else throw new InvalidCastException("You do not have enough artefacts, you can buy them in the shop !");
                    }
                }

                string jsonString = "[{";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr") // French
                    jsonString += "\"Success\" : \"true\", \"BtnNextStep\" : \"Étape suivante\"";
                else // English
                    jsonString += "\"Success\" : \"true\", \"BtnNextStep\" : \"Next Step\"";
                jsonString += "}]";

                return Json(JsonConvert.DeserializeObject(jsonString));
            }
            catch (InvalidCastException e)
            {
                string jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Error\",\"Msg\" : \"" + e.Message + "\" }]";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr")
                {
                    if (e.Message == "You are not at quest.")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Vous n'êtes pas dans une quête.\" }]";
                    else if (e.Message == "You do not have enough artefacts, you can buy them in the shop !")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Vous n'avez pas assez d'artefact, vous pouvez en acheter dans la boutique !\" }]";
                }

                return Json(JsonConvert.DeserializeObject(jsonString));
            }
        }

        /*************************************/
        /*            BATTLE PVE             */
        /*************************************/
        private struct SKILLRETURN
        {
            public int Dmg;
            public int Pct;
            public int Energy;
            public int Life;
            public int EnergyCost;
            public int LifeCost;
        }

        // GET : SkillState
        private SKILLRETURN SkillState(int skillId, int maxDmg, int maxPct, int energy, int vitalityAdd, int totalLife)
        {
            SKILLRETURN skill = new SKILLRETURN()
            {
                Dmg = 0, Pct = 0, Energy = 0,
                Life = 0, EnergyCost = 0, LifeCost = 0
            };

            switch (skillId)
            {
                //-> Attack
                case 1: // Knee cutters
                    skill.Dmg = (new Random(Guid.NewGuid().GetHashCode()).Next(maxDmg)) / 2;
                    skill.EnergyCost = maxDmg / 2 / energy;
                    break;
                case 2: // Punch
                    skill.Dmg = new Random(Guid.NewGuid().GetHashCode()).Next(maxDmg);
                    skill.EnergyCost = maxDmg / energy;
                    break;
                case 3: // Powerful Attack
                    skill.Dmg = (new Random(Guid.NewGuid().GetHashCode()).Next(maxDmg)) * 2;
                    skill.EnergyCost = maxDmg * 2 / energy;
                    break;
                //-> Defense
                case 4: // Parade
                    skill.Pct = (new Random(Guid.NewGuid().GetHashCode()).Next(maxPct)) / 2;
                    skill.EnergyCost = maxPct / 2 / energy;
                    break;
                case 5: // Blocking
                    skill.Pct = new Random(Guid.NewGuid().GetHashCode()).Next(maxPct);
                    skill.EnergyCost = maxPct / energy;
                    break;
                case 6: // Covering
                    skill.Pct = (new Random(Guid.NewGuid().GetHashCode()).Next(maxPct)) * 2;
                    skill.EnergyCost = maxPct * 2 / energy;
                    break;
                //-> Energy
                case 7: // Rest
                    skill.Energy = ((energy * 10) + 90) / 5;
                    skill.Life = (100 + vitalityAdd) / 5;
                    break;
                case 8: // Regeneration
                    skill.Energy = ((energy * 10) + 90) / 3;
                    break;
                case 9: // Effort
                    skill.Energy = ((energy * 10) + 90) / 2;
                    skill.LifeCost = totalLife / 4;
                    break;
            }

            return skill;
        }

        // GET : AjaxRealTimeBattle
        public ActionResult AjaxRealTimeBattle(int? userSkill)
        {
            try
            {
                int monsterSkillId = 0;
                using (var context = new SiteDbContext())
                {
                    // Load Data
                    var atBattlePVE = context.AtBattlePVE
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r).First();
                    if(atBattlePVE == null) throw new InvalidCastException("Internal Error: Battle not found");

                    var monster = context.Monsters
                        .Where(r => r.Id == atBattlePVE.EntityId)
                        .Select(r => r).First();
                    if (monster == null) {
                        context.Remove(atBattlePVE);
                        context.SaveChanges();
                        throw new InvalidCastException("Internal Error: Monster not found");
                    }

                    //-> End of the Battle ?  
                    if(atBattlePVE.UserLife <= 0 || atBattlePVE.EntityLife <= 0 || atBattlePVE.TourNumber >= 20)
                    {
                        string jsonString = "[{\"finished\":\"true\"";
                        if (atBattlePVE.UserLife >= atBattlePVE.EntityLife)
                            jsonString += ",\"Win\":\"true\"";
                        else
                            jsonString += ",\"Win\":\"false\"";

                        if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr") {
                            if (atBattlePVE.UserLife >= atBattlePVE.EntityLife)
                                jsonString += ",\"BtnNextStep\":\"Etape suivante\"";
                            else
                                jsonString += ",\"BtnNextStep\":\"Revenir à l'accueil\",\"HeaderTitle\":\"Vous avez perdu le combat\"";
                        }
                        else {
                            if (atBattlePVE.UserLife >= atBattlePVE.EntityLife)
                                jsonString += ",\"BtnNextStep\":\"Next step\"";
                            else
                                jsonString += ",\"BtnNextStep\":\"Back to homepage\",\"HeaderTitle\":\"You lost the fight\"";
                        }
                        jsonString += "}]";
                        return Json(JsonConvert.DeserializeObject(jsonString));
                    }

                    //-> else new tour
                    if (userSkill != null)
                    {
                        if (!(userSkill >= 1 && userSkill <= 3)) throw new InvalidCastException("Invalid skill, cheating is not friendly");

                        // User attack 
                        int totalStrength = atBattlePVE.UserStrength + atBattlePVE.UserStrengthAdd;
                        int totalDefense = atBattlePVE.UserDefense + atBattlePVE.UserDefenseAdd;
                        int totalEnergy = (atBattlePVE.UserEnergy + atBattlePVE.UserEnergyAdd);

                        int maxDmg = totalStrength * 10;
                        int maxPct = totalDefense * 10;

                        SKILLRETURN userSkillData;
                        switch (userSkill)
                        {
                            case 1:
                                userSkillData = SkillState(atBattlePVE.UserFirstSkillId, maxDmg, maxPct, totalEnergy, atBattlePVE.UserVitalityAdd, atBattlePVE.UserLife);
                                break;
                            case 2:
                                userSkillData = SkillState(atBattlePVE.UserSecondSkillId, maxDmg, maxPct, totalEnergy, atBattlePVE.UserVitalityAdd, atBattlePVE.UserLife);
                                break;
                            default:
                                userSkillData = SkillState(atBattlePVE.UserThirdSkillId, maxDmg, maxPct, totalEnergy, atBattlePVE.UserVitalityAdd, atBattlePVE.UserLife);
                                break;
                        }

                        // Monster attack
                        maxDmg = monster.Strength * 10;
                        maxPct = monster.Defense * 10;

                        SKILLRETURN entitySkillData = new SKILLRETURN();
                        if (atBattlePVE.EntityCurrentEnergy <= monster.StartEnergyPattern.GetValueOrDefault() && monster.StartEnergyPattern.GetValueOrDefault() != 0)
                        {
                            var patternArray = monster.EnergyPattern.Split(',');
                            if (atBattlePVE.EntityCurrentPatternId > patternArray.Length-1)
                                atBattlePVE.EntityCurrentPatternId = 0;

                            switch(Int32.Parse(patternArray[atBattlePVE.EntityCurrentPatternId]))
                            {
                                case 1:
                                    monsterSkillId = monster.FirstSkillId;
                                    entitySkillData = SkillState(monster.FirstSkillId,
                                        maxDmg, maxPct, monster.Energy, 0, monster.Life);
                                    break;
                                case 2:
                                    monsterSkillId = monster.SecondSkillId;
                                    entitySkillData = SkillState(monster.SecondSkillId,
                                        maxDmg, maxPct, monster.Energy, 0, monster.Life);
                                    break;
                                case 3:
                                    monsterSkillId = monster.ThirdSkillId;
                                    entitySkillData = SkillState(monster.ThirdSkillId,
                                        maxDmg, maxPct, monster.Energy, 0, monster.Life);
                                    break;
                            }
                            atBattlePVE.EntityCurrentPatternId++;
                        }
                        else if (atBattlePVE.EntityLife >= monster.StartFirstPattern.GetValueOrDefault() && monster.StartFirstPattern.GetValueOrDefault() != 0)
                        {
                            var patternArray = monster.FirstPattern.Split(',');
                            if (atBattlePVE.EntityCurrentPatternId > patternArray.Length - 1)
                                atBattlePVE.EntityCurrentPatternId = 0;

                            switch (Int32.Parse(patternArray[atBattlePVE.EntityCurrentPatternId]))
                            {
                                case 1:
                                    monsterSkillId = 1;
                                    entitySkillData = SkillState(monster.FirstSkillId,
                                        maxDmg, maxPct, monster.Energy, 0, monster.Life);
                                    break;
                                case 2:
                                    monsterSkillId = 2;
                                    entitySkillData = SkillState(monster.SecondSkillId,
                                        maxDmg, maxPct, monster.Energy, 0, monster.Life);
                                    break;
                                case 3:
                                    monsterSkillId = 3;
                                    entitySkillData = SkillState(monster.ThirdSkillId,
                                        maxDmg, maxPct, monster.Energy, 0, monster.Life);
                                    break;
                            }
                            atBattlePVE.EntityCurrentPatternId++;
                        }
                        else if (atBattlePVE.EntityLife >= monster.StartSecondPattern.GetValueOrDefault() && monster.StartSecondPattern.GetValueOrDefault() != 0)
                        {
                            var patternArray = monster.SecondPattern.Split(',');
                            if (atBattlePVE.EntityCurrentPatternId > patternArray.Length - 1)
                                atBattlePVE.EntityCurrentPatternId = 0;

                            switch (Int32.Parse(patternArray[atBattlePVE.EntityCurrentPatternId]))
                            {
                                case 1:
                                    monsterSkillId = 1;
                                    entitySkillData = SkillState(monster.FirstSkillId,
                                        maxDmg, maxPct, monster.Energy, 0, monster.Life);
                                    break;
                                case 2:
                                    monsterSkillId = 2;
                                    entitySkillData = SkillState(monster.SecondSkillId,
                                        maxDmg, maxPct, monster.Energy, 0, monster.Life);
                                    break;
                                case 3:
                                    monsterSkillId = 3;
                                    entitySkillData = SkillState(monster.ThirdSkillId,
                                        maxDmg, maxPct, monster.Energy, 0, monster.Life);
                                    break;
                            }
                            atBattlePVE.EntityCurrentPatternId++;
                        }
                        else if (atBattlePVE.EntityLife >= monster.StartThirdPattern.GetValueOrDefault() && monster.StartThirdPattern.GetValueOrDefault() != 0)
                        {
                            var patternArray = monster.ThirdPattern.Split(',');
                            if (atBattlePVE.EntityCurrentPatternId > patternArray.Length - 1)
                                atBattlePVE.EntityCurrentPatternId = 0;

                            switch (Int32.Parse(patternArray[atBattlePVE.EntityCurrentPatternId]))
                            {
                                case 1:
                                    monsterSkillId = 1;
                                    entitySkillData = SkillState(monster.FirstSkillId,
                                        maxDmg, maxPct, monster.Energy, 0, monster.Life);
                                    break;
                                case 2:
                                    monsterSkillId = 2;
                                    entitySkillData = SkillState(monster.SecondSkillId,
                                        maxDmg, maxPct, monster.Energy, 0, monster.Life);
                                    break;
                                case 3:
                                    monsterSkillId = 3;
                                    entitySkillData = SkillState(monster.ThirdSkillId,
                                        maxDmg, maxPct, monster.Energy, 0, monster.Life);
                                    break;
                            }
                            atBattlePVE.EntityCurrentPatternId++;
                        }
                        else
                        {
                            switch (new Random().Next(1, 4))
                            {
                                case 1:
                                    monsterSkillId = 1;
                                    entitySkillData = SkillState(monster.FirstSkillId,
                                        maxDmg, maxPct, monster.Energy, 0, monster.Life);
                                    break;
                                case 2:
                                    monsterSkillId = 2;
                                    entitySkillData = SkillState(monster.SecondSkillId,
                                        maxDmg, maxPct, monster.Energy, 0, monster.Life);
                                    break;
                                case 3:
                                    monsterSkillId = 3;
                                    entitySkillData = SkillState(monster.ThirdSkillId,
                                        maxDmg, maxPct, monster.Energy, 0, monster.Life);
                                    break;
                            }
                        }

                        //-> Tour result
                        if (atBattlePVE.UserCurrentEnergy - userSkillData.EnergyCost < 0)
                            userSkillData = new SKILLRETURN(); // Reset Skill

                        if (atBattlePVE.EntityCurrentEnergy - userSkillData.EnergyCost < 0)
                            entitySkillData = new SKILLRETURN(); // Reset Skill

                        // Player
                        atBattlePVE.UserLife -= userSkillData.LifeCost;

                        if (atBattlePVE.UserLife + userSkillData.Life > 100 + (atBattlePVE.UserVitalityAdd * 10))
                            atBattlePVE.UserLife = 100 + (atBattlePVE.UserVitalityAdd * 10); // Limit Life
                        else
                            atBattlePVE.UserLife += userSkillData.Life;

                        int finalDmg = entitySkillData.Dmg - userSkillData.Pct;
                        if (finalDmg < 0)
                            finalDmg = 0;
                        atBattlePVE.UserLife -= finalDmg;

                        if (atBattlePVE.UserCurrentEnergy + userSkillData.Energy > 90 + (totalEnergy * 10))
                            atBattlePVE.UserCurrentEnergy = 90 + (totalEnergy * 10); // Limit Energy
                        else
                            atBattlePVE.UserCurrentEnergy += userSkillData.Energy;

                        atBattlePVE.UserCurrentEnergy -= userSkillData.EnergyCost;

                        // Entity
                        atBattlePVE.EntityLife -= entitySkillData.LifeCost;

                        if (atBattlePVE.EntityLife + entitySkillData.Life > monster.Life)
                            atBattlePVE.EntityLife = monster.Life; // Limit Life
                        else
                            atBattlePVE.EntityLife += entitySkillData.Life;

                        finalDmg = userSkillData.Dmg - entitySkillData.Pct;
                        if (finalDmg < 0)
                            finalDmg = 0;
                        atBattlePVE.EntityLife -= finalDmg;

                        if (atBattlePVE.EntityCurrentEnergy + entitySkillData.Energy > 90 + (monster.Energy * 10))
                            atBattlePVE.EntityCurrentEnergy = 90 + (monster.Energy * 10); // Limit Energy
                        else
                            atBattlePVE.EntityCurrentEnergy += entitySkillData.Energy;

                        atBattlePVE.EntityCurrentEnergy -= entitySkillData.EnergyCost;
                    }

                    //-> Finished
                    atBattlePVE.TourNumber++;
                    context.Update(atBattlePVE);
                    context.SaveChanges();

                    string jsonReturn = "[{";
                    // Player
                    jsonReturn += "\"UserLife\":\"" + atBattlePVE.UserLife + "\", \"UserLifePercentage\":\"" + atBattlePVE.UserLife * 100 / (100 + atBattlePVE.UserVitalityAdd) + "\",";
                    jsonReturn += "\"UserEnergy\":\"" + atBattlePVE.UserCurrentEnergy + "\", \"UserEnergyPercentage\":\"" + (atBattlePVE.UserCurrentEnergy * 100 / (90 + ((atBattlePVE.UserEnergy+atBattlePVE.UserEnergyAdd) * 10))) + "\",";

                    // Monster
                    jsonReturn += "\"EntityLife\":\"" + atBattlePVE.EntityLife + "\", \"EntityLifePercentage\":\"" + atBattlePVE.EntityLife * 100 / monster.Life + "\",";
                    jsonReturn += "\"EntityEnergy\":\"" + atBattlePVE.EntityCurrentEnergy + "\", \"EntityEnergyPercentage\":\"" + (atBattlePVE.EntityCurrentEnergy * 100 / (90 + (monster.Energy * 10))) + "\", \"EntitySkillId\":\"" + monsterSkillId + "\"";

                    jsonReturn += "}]";
                    return Json(JsonConvert.DeserializeObject(jsonReturn));
                }
            }
            catch (InvalidCastException e)
            {
                string jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Error\",\"Msg\" : \"" + e.Message + "\" }]";
                return Json(JsonConvert.DeserializeObject(jsonString));
            }
        }

        // GET : AjaxRealTimeBattleAbandon
        public ActionResult AjaxRealTimeBattleAbandon()
        {
            try
            {
                using (var context = new SiteDbContext())
                {
                    var atBattlePVE = context.AtBattlePVE
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r).First();
                    if (atBattlePVE == null) throw new InvalidCastException("System error: AtBattlePVE doesn't exist");

                    atBattlePVE.UserLife = 0;

                    context.Update(atBattlePVE);
                    context.SaveChanges();
                }
                return Json(JsonConvert.DeserializeObject("[{ \"Success\" : \"true\" }]"));
            }
            catch (InvalidCastException e)
            {
                string jsonString = "[{ \"Success\" : \"false\", \"Title\" : \"Error\",\"Msg\" : \"" + e.Message + "\" }]";
                return Json(JsonConvert.DeserializeObject(jsonString));
            }
        }

        // GET : AjaxRealTimeBattleBuff
        public ActionResult AjaxRealTimeBattleBuff()
        {
            try
            {
                using (var context = new SiteDbContext())
                {
                    var atBattlePVE = context.AtBattlePVE
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r).First();
                    if (atBattlePVE == null) throw new InvalidCastException("System error: AtBattlePVE doesn't exist");

                    var artifacts = context.Artifacts
                        .Where(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                        .Select(r => r).FirstOrDefault();
                    if (artifacts == null) throw new InvalidCastException("System error: Artifact doesn't exist");

                    if (artifacts.Number >= 1)
                    {
                        artifacts.Number -= 1;

                        atBattlePVE.UserDefense *= 2;
                        atBattlePVE.UserStrength *= 2;
                        atBattlePVE.UserCurrentEnergy = 90 + ((atBattlePVE.UserEnergy + atBattlePVE.UserEnergyAdd) * 10);

                        context.Update(atBattlePVE);
                        context.SaveChanges();
                    }
                    else throw new InvalidCastException("You do not have enough artifacts.");
                }

                string jsonString = "[{";
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr") // French
                    jsonString += "\"Success\" : \"true\",\"Title\" : \"Bravo\",\"Msg\" : \"Vous êtes maintenant plus fort pour le combat! (Bonus de multiplication par 2)\"";
                else // English
                    jsonString += "\"Success\" : \"true\",\"Title\" : \"Bravo\",\"Msg\" : \"You are now stronger for combat ! (Bonus multiplication by 2)\"";
                jsonString += "}]";

                return Json(JsonConvert.DeserializeObject(jsonString));
            }
            catch (InvalidCastException e)
            {
                string jsonString = "[{ \"Success\" : \"false\", \"Title\" : \"Error\",\"Msg\" : \"" + e.Message + "\" }]";

                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr")
                {
                    if (e.Message == "You do not have enough artifacts.")
                        jsonString = "[{ \"Success\" : \"false\",\"Title\" : \"Erreur\",\"Msg\" : \"Vous n'avez pas assez d'artefacts.\" }]";
                }
                return Json(JsonConvert.DeserializeObject(jsonString));
            }
        }
    }
}