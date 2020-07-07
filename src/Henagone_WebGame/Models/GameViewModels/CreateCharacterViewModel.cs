using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Henagone_WebGame.Models.GameViewModels
{
    public class CreateCharacterViewModel
    {
        [Required]
        public string AvatarId { get; set; }
    }
}
