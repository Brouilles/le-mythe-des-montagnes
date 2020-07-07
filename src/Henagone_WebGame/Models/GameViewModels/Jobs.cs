using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Henagone_WebGame.Models.GameViewModels
{
    public class Jobs
    {
        [Key]
        public int? Id { get; set; }

        [Required]
        public string TitleEn { get; set; }

        [Required]
        public string TitleFr { get; set; }

        [Required]
        public int Gold { get; set; }

        [Required]
        public int Time { get; set; } 
    }
}
