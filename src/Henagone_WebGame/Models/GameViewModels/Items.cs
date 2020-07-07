using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Henagone_WebGame.Models.GameViewModels
{
    public class Items
    {
        [Key]
        public int? Id { get; set; }
        public string TitleFr { get; set; }
        public string TitleEn { get; set; }
        [Required]
        public int Type { get; set; }

        [Required]
        public int VitalityAdd { get; set; }
        [Required]
        public int StrengthAdd { get; set; }
        [Required]
        public int DefenseAdd { get; set; }
        [Required]
        public int EnergyAdd { get; set; }
        [Required]
        public int InventorySpaceAdd { get; set; }

        [Required]
        public int StrengthRequired { get; set; }
        [Required]
        public int DefenseRequired { get; set; }
        [Required]
        public int EnergyRequired { get; set; }

        [Required]
        public int Purchase { get; set; }
        [Required]
        public int PurchaseArtifact { get; set; }
        [Required]
        public int Sale { get; set; }

        [Required]
        public int Scarcity { get; set; } // Percentage
        [Required]
        public int Durability { get; set; }
        [Required]
        public int Weight { get; set; }
    }
}
