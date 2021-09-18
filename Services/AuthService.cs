using ShopIn_API.ViewModel;
using ShopIn_API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ShopIn_API.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT_VM _jwt;
        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT_VM> jwt)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt.Value;
        }

        public async Task<string> AddRoleAsync(AddRole_VM model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user is null || !await _roleManager.RoleExistsAsync(model.Role))
            {
                return "Invalid User Id or Role!";
            }

            if (await _userManager.IsInRoleAsync(user, model.Role))
            {
                return "User already assigned to this role!";
            }

            var result = await _userManager.AddToRoleAsync(user, model.Role);

            return result.Succeeded ? "Success" : "Something went wrong!";
        }

        public async Task<Auth_VM> GetTokenAsync(TokenRequest_VM model)
        {
            var Auth_VM = new Auth_VM();
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                Auth_VM.Message = "Email or Password is incorrect!";
                return Auth_VM;
            }

            var jwtSecurityToken = await CreateJwtToken(user);
            var roleList = await _userManager.GetRolesAsync(user);

            Auth_VM.IsAuthenticated = true;
            Auth_VM.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            Auth_VM.Email = user.Email;
            Auth_VM.Username = user.UserName;
            Auth_VM.ExpiresOn = jwtSecurityToken.ValidTo;
            Auth_VM.Roles = roleList.ToList();
            Auth_VM.Message = "Success";
            Auth_VM.UserId = user.Id;
            Auth_VM.PhoneNumber = user.PhoneNumber;
            Auth_VM.FirstName = user.FirstName;
            Auth_VM.LastName = user.LastName;

            return Auth_VM;
        }

        public async Task<Auth_VM> RegisterAsync(Register_VM model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
            {
                return new Auth_VM { Message = "Email is already registered" };
            }

            if (await _userManager.FindByNameAsync(model.UserName) is not null)
            {
                return new Auth_VM { Message = "Username is already registered" };
            }

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Country = model.Country,
                TermsAndConditions = model.TermsAndConditions

            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var Errors = string.Empty;

                foreach (var error in result.Errors)
                {
                    Errors += $"{error.Description} ,";
                }

                return new Auth_VM { Message = Errors };
            }
            await _userManager.AddToRoleAsync(user, "User");

            var jwtSecurityToken = await CreateJwtToken(user);

            return new Auth_VM
            {
                Email = user.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Message = "Success",
                Roles = new List<string> { "User" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Username = user.UserName
            };

        }

        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }

    }

}
