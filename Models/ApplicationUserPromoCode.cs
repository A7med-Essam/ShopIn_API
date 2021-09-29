using ShopIn_API.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ShopIn_API.Models
{
    public class ApplicationUserPromoCode
    {
        public bool isUsedPromoCode { get; set; }

        public string ApplicationUserId { get; set; }
        public int PromoCodeId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual PromoCode PromoCode { get; set; }
    }
}
