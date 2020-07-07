using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Henagone_WebGame.Models.GameViewModels
{
    public class Characters
    {
        [Key]
        public string UserId { get; set; }
        public int AvatarId { get; set; }
        public int Level { get; set; }
        public int SkillsPoints { get; set; }
        public int Life { get; set; }
        public int Xp { get; set; }

        public int Strength { get; set; }
        public int Defense { get; set; }
        public int Energy { get; set; }

        public int VitalityAdd { get; set; }
        public int StrengthAdd { get; set; }
        public int DefenseAdd { get; set; }
        public int EnergyAdd { get; set; }

        public int? WeaponInventoryId { get; set; }
        public int? ArmorInventoryId { get; set; }
        public int? ShieldInventoryId { get; set; }
        public int? BootsInventoryId { get; set; }
        public int? JewelryInventoryId { get; set; }
        public int? BagInventoryId { get; set; }

        public int FirstSkillId { get; set; }
        public int SecondSkillId { get; set; }
        public int ThirdSkillId { get; set; }

        public int NarrativeQuestStep { get; set; }
        public DateTime Register { get; set; }
        public int Death { get; set; }

        public bool Tutorial { get; set; }
    }
}
