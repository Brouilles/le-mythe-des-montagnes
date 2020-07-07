using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Henagone_WebGame.Models.GameViewModels
{
    public class Potions
    {
        [Key]
        public int? Id { get; set; }
        [Required]
        public string TitleEn { get; set; }
        [Required]
        public string TitleFr { get; set; }

        [Required]
        public int VitalityAdd { get; set; }
        [Required]
        public int StrengthAdd { get; set; }
        [Required]
        public int DefenseAdd { get; set; }
        [Required]
        public int EnergyAdd { get; set; }
        [Required]
        public int Time { get; set; }

        [Required]
        public int Purchase { get; set; }
    }
}
