using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Henagone_WebGame.Models.GameViewModels
{
    public class CompanionQuests
    {
        [Key]
        public int? Id { get; set; }
        public string TitleFr { get; set; }
        public string TitleEn { get; set; }

        public string DescriptionFr { get; set; }
        public string DescriptionEn { get; set; }

        public int Class { get; set; }
        public int Time { get; set; }

        public int StrengthAdd { get; set; }
        public int DefenseAdd { get; set; }
        public int EnergyAdd { get; set; }
        public int Care { get; set; }

        public string Items { get; set; }
        public string Potions { get; set; }

        public int Xp { get; set; }
        public int Gold { get; set; }
        public int Success { get; set; }
    }
}
