using ShopIn_API.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShopIn_API.Models
{
    public class PromoCode
    {
        public int id { get; set; }
        [Required]
        public string promoCode { get; set; }
        [Required]
        public int DiscountValue { get; set; }
        public PromoCode()
        {
            applicationUsers = new List<ApplicationUser>();
        }
        public virtual List<ApplicationUser> applicationUsers { get; set; }
    }
}
