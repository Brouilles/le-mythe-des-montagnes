using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Henagone_WebGame.Models.GameViewModels
{
    public class AtWork
    {
        [Key]
        public string UserId { get; set; }
        public int JobId { get; set; }
        public DateTime Start { get; set; }
    }
}
