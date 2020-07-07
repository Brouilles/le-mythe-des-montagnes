using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Henagone_WebGame.Models.GameViewModels
{
    public class Hunt_statistics
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public int EntityId { get; set; }
        public DateTime Date { get; set; }
    }

    public class HuntViewModel
    {
        public MonstersType monsters { get; set; }
        public int kill { get; set; }
    }
}
