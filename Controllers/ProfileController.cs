using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShopIn_API.Models;
using ShopIn_API.Interfaces;
using ShopIn_API.ViewModel;
using Microsoft.AspNetCore.Identity;
using ShopIn_API.Services;

namespace ShopIn_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly ShopInContext db;
        private readonly IphotoServices _photoService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthService _authService;

        #region Constructor
        public ProfileController(
            IphotoServices photoService,
            ShopInContext db,
            UserManager<ApplicationUser> userManager,
            IAuthService authService)
        {
            _photoService = photoService;
            _userManager = userManager;
            _authService = authService;
            this.db = db;
        }
        #endregion

        #region get profile
        [HttpGet("getUserProfileByEmail")]
        public async Task<IActionResult> getUserProfileByEmail([FromQuery] string email)
        {
            if (email == null)
            {
                return BadRequest();
            }
            ApplicationUser result = await _userManager.FindByEmailAsync(email);

            var User = new { result.Email, result.FirstName, result.Country, result.LastName, result.UserName, result.PhoneNumber, result.Gender, result.ProfileImageUrl };

            return Ok(User);
        }
        #endregion

        #region update profile

        [HttpPut("updateUserName")]
        public async Task<IActionResult> updateUserName([FromBody] UpdateProfile_VM model)
        {
            if (model.Username == null || model.UserId == null)
            {
                return BadRequest("Username is required!");
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return BadRequest("ERROR OCCURRED!");
            }
            var result = await _userManager.SetUserNameAsync(user, model.Username);
            if (!result.Succeeded)
            {
                if (await _userManager.FindByNameAsync(model.Username) is not null)
                {
                    return BadRequest("Username is already taken!");
                }
                return BadRequest("Error occurred while updating username!");
            }

            return Ok();
        }

        [HttpPut("updatePhoneNumber")]
        public async Task<IActionResult> updatePhoneNumber([FromBody] UpdateProfile_VM model)
        {
            if (model.UserId == null || model.PhoneNumber == null)
            {
                return BadRequest("Phone number is required!");
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return BadRequest("ERROR OCCURRED!");
            }

            var result = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);

            if (!result.Succeeded)
            {
                return BadRequest("Error occurred while updating phone number!");
            }

            return Ok();
        }

        [HttpPut("updatePassword")]
        public async Task<IActionResult> updatePassword([FromBody] UpdateProfile_VM model)
        {
            if (model.UserId == null || model.NewPassword == null || model.CurrentPassword == null)
            {
                return BadRequest("Password is required!");
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return BadRequest("ERROR OCCURRED!");
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                if (!await _userManager.CheckPasswordAsync(user, model.CurrentPassword))
                {
                    return BadRequest("Current Password is Invalid");

                }
                return BadRequest("Error occurred while changing password!");
            }
            return Ok();
        }

        [HttpPut("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfile_VM model)
        {
            if (model.FirstName == null)
            {
                return BadRequest("First name is required");
            }
            else if(model.LastName == null)
            {
                return BadRequest("Last name is required");
            }
            else if (model.Gender == null)
            {
                return BadRequest("Gender is required");
            }
            else if (model.Country == null)
            {
                return BadRequest("Country is required");
            }
            else if (model.UserId == null)
            {
                return BadRequest("Error Occurred!");
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return BadRequest("ERROR OCCURRED!");
            }
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Country = model.Country;
            user.Gender = model.Gender;
            db.SaveChanges();
            return Ok();
        }

        [HttpPut("UpdateProfileImage")]
        public async Task<IActionResult> UpdateProfileImage([FromQuery] string UserId ,[FromForm] IFormFile file)
        {
            if (file == null)
            {
                return BadRequest("image is required!");
            }
            if (UserId == null)
            {
                return BadRequest("Error Occurred!");
            }

            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                return BadRequest("ERROR OCCURRED!");
            }

            // delete old image through pubic id
            var DeletionResult = await _photoService.DeletePhotoAsync(user.ImagePublicId);
            if (DeletionResult.Error != null)
            {
                return BadRequest(DeletionResult.Error.Message);
            }

            // add new image and new public id
            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null)
            {
                return BadRequest(result.Error.Message);
            }

            user.ProfileImageUrl = result.SecureUrl.AbsoluteUri;
            user.ImagePublicId = result.PublicId;

            db.SaveChanges();
            return Ok();
        }

        #endregion
    }
}
