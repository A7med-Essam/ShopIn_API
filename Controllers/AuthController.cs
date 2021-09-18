using ShopIn_API.ViewModel;
using ShopIn_API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ShopIn_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ShopIn_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ShopInContext db;
        private readonly UserManager<ApplicationUser> _userManager;

        #region Constructor
        public AuthController(IAuthService authService, ShopInContext db, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _authService = authService;
            this.db = db;
        }
        #endregion

        #region Sign up & Sign In
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] Register_VM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            model.UserName = model.Email;
            var result = await _authService.RegisterAsync(model);

            if (!result.IsAuthenticated)
            {
                return BadRequest(result.Message);
            }

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] TokenRequest_VM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.GetTokenAsync(model);

            if (!result.IsAuthenticated)
            {
                return BadRequest(result.Message);
            }

            return Ok(result);
        }
        #endregion

        #region Roles Controller
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("addRole")]
        public async Task<IActionResult> AddRoleAsync([FromBody] AddRole_VM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.AddRoleAsync(model);

            if (result == "Success")
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete("deleteRole")]
        public async Task<IActionResult> DeleteRolesAsync(string id)
        {
            //[FromQuery(Name = "userId")] string userId
            //string userId = HttpContext.Request.Query["userId"].ToString();
            if (id != null)
            {
                var user = await _userManager.FindByIdAsync(id);
                await _userManager.RemoveFromRoleAsync(user, "Admin");
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
        #endregion

        #region Get Users
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("getAllAdmins")]
        public IActionResult getAllAdmins()
        {
            ////The first step: get all Admins id collection as AdminsIds based on role from db.UserRoles
            List<string> AdminsIds = db.UserRoles.Where(a => a.RoleId == "2").Select(b => b.UserId).Distinct().ToList();
            ////get all SuperAdmins id collection
            List<string> SuperAdminsIds = db.UserRoles.Where(a => a.RoleId == "3").Select(b => b.UserId).Distinct().ToList();
            //// new list to add admins only
            var Admins = new List<string>();

            //// filter Admins from superAdmin
            foreach (var AdminsId in AdminsIds)
            {
                    foreach (var SuperAdminsId in SuperAdminsIds)
                    {
                        if (AdminsId != SuperAdminsId)
                        {
                            Admins.Add(AdminsId);
                        }
                    }
            }
            ////The second step: find all users collection from _db.Users which 's Id is only contained at AdminsIds;
            List<ApplicationUser> listOfAdmins = db.Users.Where(a => Admins.Any(c => c == a.Id)).ToList();
            return Ok(listOfAdmins);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("getAllUsers")]
        public IActionResult getAllUsers([FromQuery] PagingParameter_VM pagingparametermodel)
        {
            ////The first step: get all Admins id collection as AdminsIds based on role from db.UserRoles
            List<string> AdminsIds = db.UserRoles.Where(a => a.RoleId == "2").Select(b => b.UserId).Distinct().ToList();
            ////get all Users id collection
            List<string> UserIds = db.UserRoles.Where(a => a.RoleId == "1").Select(b => b.UserId).Distinct().ToList();
            ////get all SuperAdmins id collection
            List<string> SuperAdminsIds = db.UserRoles.Where(a => a.RoleId == "3").Select(b => b.UserId).Distinct().ToList();
            //// new list to add admins only
            var Admins = new List<string>();

            //// The second step: filter Admins from Users
            foreach (var UserId in UserIds)
            {
                foreach (var AdminsId in AdminsIds)
                {
                    foreach (var SuperAdminsId in SuperAdminsIds)
                    {
                        if (UserId == AdminsId || UserId == SuperAdminsId)
                        {
                            if (!Admins.Contains(UserId))
                            {
                                Admins.Add(UserId);
                            }
                        }
                    }
                }
            }

            // The third step: get users only by filtering list of admins from all users
            var filtered = UserIds
                   .Where(x => !Admins.Any(y => y == x));

            ////The fourth step: find all users collection from _db.Users which 's Id is only contained at AdminsIds;
            List<ApplicationUser> ListOfUsers = db.Users.Where(a => filtered.Any(c => c == a.Id)).ToList();


            int count = ListOfUsers.Count();
            int CurrentPage = pagingparametermodel.pageNumber;
            int PageSize = pagingparametermodel.pageSize;
            int TotalCount = count;
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            var items = ListOfUsers.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            var previousPage = CurrentPage > 1 ? "Yes" : "No";
            var nextPage = CurrentPage < TotalPages ? "Yes" : "No";
            var paginationMetadata = new
            {
                data = items,
                totalCount = TotalCount,
                pageSize = PageSize,
                currentPage = CurrentPage,
                totalPages = TotalPages,
                previousPage,
                nextPage
            };
            return Ok(paginationMetadata);
        }
        #endregion

    }
}
