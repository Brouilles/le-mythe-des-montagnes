using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Henagone_WebGame.Models.GameViewModels
{
    public class Buff
    {
        [Key]
        public int? Id { get; set; }
        public string UserId { get; set; }
        public int PotionId { get; set; }
        public DateTime Start { get; set; }
    }
}
