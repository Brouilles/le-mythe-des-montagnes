using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Henagone_WebGame.Models.GameViewModels
{
    public class HeroesQuestsState
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public int QuestId { get; set; }
        public int State { get; set; } // 0 = finish | 1 = Description | 2 = Timer Finish | 3 = Battle Finish 
        public DateTime? StartTimer { get; set; }
    }
}
