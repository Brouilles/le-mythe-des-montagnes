using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Henagone_WebGame.Models.GameViewModels
{
    public class Companions
    {
        [Key]
        public int? Id { get; set; }
        public string TitleFr { get; set; }
        public string TitleEn { get; set; }

        public int Class { get; set; }

        public int Purchase { get; set; }
        public int Scarcity { get; set; } // Percentage
    }
}
