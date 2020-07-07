using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Henagone_WebGame.Models.ShopViewModels
{
    public class ArtifactsPrices
    {
        [Key]
        public int Id { get; set; }
        public decimal Price { get; set; }
        public int Artifacts { get; set; }
    }
}
