using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Henagone_WebGame.Models.ShopViewModels
{
    public class StripeChargeForm
    {
        public string Token { get; set; }
        public string Name { get; set; }
        public int DealsId { get; set; }
    }
}
