using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShopIn_API.ViewModel
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Country { get; set; }
        
        [Required]
        public bool TermsAndConditions { get; set; }

        [MaxLength(10)]
        public string Gender { get; set; }

        public string ProfileImageUrl { get; set; }
        public string ImagePublicId { get; set; }

    }
}
