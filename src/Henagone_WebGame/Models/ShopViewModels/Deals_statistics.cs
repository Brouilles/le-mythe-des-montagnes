using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Henagone_WebGame.Models.ShopViewModels
{
    public class Deals_statistics
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime Date { get; set; }
        public string Code { get; set; }
        public int Nb_Artifacts { get; set; }
        public string PaymentType { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
    }
}
