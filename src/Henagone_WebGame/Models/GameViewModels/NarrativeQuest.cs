using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Henagone_WebGame.Models.GameViewModels
{
    public class NarrativeQuest
    {
        [Key]
        public int? Id { get; set; }
        public string StepList { get; set; }
        public string TitleFr { get; set; }
        public string TitleEn { get; set; }

        // Travel
        public string DescriptionFr { get; set; }
        public string DescriptionEn { get; set; }
        public int Time { get; set; }

        public int Xp { get; set; }
        public int Gold { get; set; }
        public string Items { get; set; }
        public string Potions { get; set; }
    }
}
