using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Henagone_WebGame.Models.GameViewModels
{
    public class AtNarrativeQuest
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public int QuestId { get; set; }
        public int Step { get; set; } // Current state of the quest
        public DateTime? StartTimer { get; set; }
    }
}
