
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Henagone_WebGame.Models.GameViewModels
{
    public class NarrativeQuestStep
    {
        [Key]
        public int? Id { get; set; }
        public int NarrativeQuestId { get; set; }
        public int Type { get; set; } // 1 = Choice / 2 = Battle
        public int ZIndex { get; set; }

        public string DescriptionFr { get; set; }
        public string DescriptionEn { get; set; }

        // 1 = Choice
        public string ChoiceOneFr { get; set; }
        public string ChoiceTwoFr { get; set; }
        public string ChoiceThreeFr { get; set; }

        public string ChoiceOneEn { get; set; }
        public string ChoiceTwoEn { get; set; }
        public string ChoiceThreeEn { get; set; }

        public string TextOneFr { get; set; } // If empty, directly next step
        public string TextTwoFr { get; set; }
        public string TextThreeFr { get; set; }

        public string TextOneEn { get; set; } 
        public string TextTwoEn { get; set; }
        public string TextThreeEn { get; set; }

        public int? ChoiceOneIncrease { get; set; }
        public int? ChoiceTwoIncrease { get; set; }
        public int? ChoiceThreeIncrease { get; set; }

        // 2 = Battle
        public int? MonsterId { get; set; }
        public int? BattleIncrease { get; set; }
    }
}
