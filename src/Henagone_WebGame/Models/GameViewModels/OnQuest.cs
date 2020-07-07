using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Henagone_WebGame.Models.GameViewModels
{
    public class OnQuest
    {
        [Key]
        public int TeamsId { get; set; }
        public int QuestId { get; set; }
        public DateTime Start { get; set; }
    }
}
