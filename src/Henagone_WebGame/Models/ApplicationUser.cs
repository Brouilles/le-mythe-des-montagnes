using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Henagone_WebGame.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string StripeId { get; set; }
        public int? SlotForCompanion { get; set; }
        public DateTime Register { get; set; }
    }
}
