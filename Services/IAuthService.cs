using ShopIn_API.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopIn_API.Services
{
    public interface IAuthService
    {
        Task<Auth_VM> RegisterAsync(Register_VM model);
        Task<Auth_VM> GetTokenAsync(TokenRequest_VM model);
        Task<string> AddRoleAsync(AddRole_VM model);
    }
}
