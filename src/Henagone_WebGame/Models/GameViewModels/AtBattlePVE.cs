using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Henagone_WebGame.Models.GameViewModels
{
    public class AtBattlePVE
    {
        // User
        [Key]
        public string UserId { get; set; }
        public int UserLife { get; set; }
        public int UserCurrentEnergy { get; set; }

        public int UserFirstSkillId { get; set; }
        public int UserSecondSkillId { get; set; }
        public int UserThirdSkillId { get; set; }

        public int UserStrength { get; set; }
        public int UserDefense { get; set; }
        public int UserEnergy { get; set; }
        public int UserVitalityAdd { get; set; }
        public int UserStrengthAdd { get; set; }
        public int UserDefenseAdd { get; set; }
        public int UserEnergyAdd { get; set; }

        // Monster
        public int EntityId { get; set; }
        public int EntityLife { get; set; }
        public int EntityCurrentEnergy { get; set; }
        public int EntityCurrentPatternId { get; set; }

        public int TourNumber { get; set; }
    }

    public class UserSkillStruct
    {
        public int PlayerSkillId { get; set; }
        public int SkillId { get; set; }
    }
}
