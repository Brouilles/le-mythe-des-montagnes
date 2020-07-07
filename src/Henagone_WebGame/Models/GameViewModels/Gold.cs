using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Henagone_WebGame.Models.GameViewModels
{
    public class Gold
    {
        [Key]
        public string UserId { get; set; }
        public int Number { get; set; }
    }
}
