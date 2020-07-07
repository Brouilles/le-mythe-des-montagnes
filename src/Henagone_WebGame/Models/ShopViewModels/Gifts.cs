using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Henagone_WebGame.Models.ShopViewModels
{
    public class Gifts
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }

        [Required]
        public DateTime Start { get; set; }

        [Required]
        public DateTime End { get; set; }

        public int? Artifacts { get; set; }
        public int? Experiences { get; set; }
        public string Items { get; set; }

        public bool SinglePerAccount { get; set; }
        public bool Active { get; set; }
    }
}
