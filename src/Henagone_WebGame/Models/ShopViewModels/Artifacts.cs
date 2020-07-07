using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Henagone_WebGame.Models.ShopViewModels
{
    public class Artifacts
    {
        [Key]
        public string UserId { get; set; }
        public int Number { get; set; }
    }
}
