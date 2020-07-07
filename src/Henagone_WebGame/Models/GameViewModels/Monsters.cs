using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Henagone_WebGame.Models.GameViewModels
{
    public class Monsters
    {
        [Key]
        public int? Id { get; set; }
        public int? Type { get; set; }
        [Required]
        public string TitleFr { get; set; }
        [Required]
        public string TitleEn { get; set; }

        [Required]
        public int Life { get; set; }
        [Required]
        public int Strength { get; set; }
        [Required]
        public int Defense { get; set; }
        [Required]
        public int Energy { get; set; }

        [Required]
        public int FirstSkillId { get; set; }
        [Required]
        public int SecondSkillId { get; set; }
        [Required]
        public int ThirdSkillId { get; set; }

        // First Pattern
        public int? StartFirstPattern { get; set; }
        public string FirstPattern { get; set; }

        // Second Pattern
        public int? StartSecondPattern { get; set; }
        public string SecondPattern { get; set; }

        // Third Pattern
        public int? StartThirdPattern { get; set; }
        public string ThirdPattern { get; set; }

        // Energy Pattern
        public int? StartEnergyPattern { get; set; }
        public string EnergyPattern { get; set; }

        [Required]
        public int Scarcity { get; set; }
    }
}
