using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopIn_API.ViewModel
{
    public class UpdateProfile_VM
    {
        public string Username { get; set; }
        public string UserId { get; set; }
        public string Token { get; set; }
        public string PhoneNumber { get; set; }
        public string NewPassword { get; set; }
        public string CurrentPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public string Gender { get; set; }
    }
}
