﻿
namespace WebAPI.Controllers.API
{
    using System.Linq;
    using System.Threading.Tasks;
    using Common.Models;
    using Helper;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[Controller]")]
    public class AccountController : Controller
    {
        private readonly IUserHelper userHelper;
        private readonly IMailHelper mailHelper;

        public AccountController(IUserHelper userHelper, IMailHelper mailHelper)
        {
            this.userHelper = userHelper;
            this.mailHelper = mailHelper;
        }

        [HttpPost]
        public async Task<IActionResult> PostUser([FromBody] NewUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "Bad request"
                });
            }

            var user = await this.userHelper.GetUserByEmailAsync(request.Email);
            if (user != null)
            {
                return this.BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "This email is already registered."
                });
            }

            user = new Data.Entities.User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                UserName = request.Email,
                Address = request.Address,
                PhoneNumber = request.Phone
            };

            var result = await userHelper.AddUserAsync(user, request.Password);
            if (result != IdentityResult.Success)
            {
                return this.BadRequest(result.Errors.FirstOrDefault().Description);
            }

            var myToken = await userHelper.GenerateEmailConfirmationTokenAsync(user);
            var tokenLink = Url.Action("ConfirmEmail", "Account", new
            {
                userid = user.Id,
                token = myToken
            }, protocol: HttpContext.Request.Scheme);

            this.mailHelper.SendMail(request.Email, "Email confirmation", $"<h1>Email Confirmation</h1>" +
                $"To allow the user, " +
                $"plase click in this link:</br></br><a href = \"{tokenLink}\">Confirm Email</a>");

            return Ok(new Response
            {
                IsSuccess = true,
                Message = "A Confirmation email was sent. Plese confirm your account and log into the App."
            });
        }

        [HttpPost]
        [Route("RecoverPassword")]
        public async Task<IActionResult> RecoverPassword([FromBody] RecoverPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "Bad request"
                });
            }

            var user = await this.userHelper.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                return this.BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "This email is not assigned to any user."
                });
            }

            var myToken = await userHelper.GeneratePasswordResetTokenAsync(user);
            var link = Url.Action("ResetPassword", "Account", new { token = myToken }, protocol: HttpContext.Request.Scheme);
            this.mailHelper.SendMail(request.Email, "Password Reset", $"<h1>Recover Password</h1>" +
                $"To reset the password click in this link:</br></br>" +
                $"<a href = \"{link}\">Reset Password</a>");

            return Ok(new Response
            {
                IsSuccess = true,
                Message = "An email with instructions to change the password was sent."
            });
        }

        [HttpPost]
        [Route("GetUserByEmail")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUserByEmail([FromBody] RecoverPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "Bad request"
                });
            }

            var user = await this.userHelper.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                return this.BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "User don't exists."
                });
            }

            return Ok(user);
        }

        [HttpPut]
        public async Task<IActionResult> PutUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest(ModelState);
            }

            var userEntity = await this.userHelper.GetUserByEmailAsync(user.Email);
            if (userEntity == null)
            {
                return this.BadRequest("User not found.");
            }

            
            userEntity.FirstName = user.FirstName;
            userEntity.LastName = user.LastName;
            userEntity.Address = user.Address;
            userEntity.PhoneNumber = user.PhoneNumber;

            var respose = await userHelper.UpdateUserAsync(userEntity);
            if (!respose.Succeeded)
            {
                return this.BadRequest(respose.Errors.FirstOrDefault().Description);
            }

            var updatedUser = await this.userHelper.GetUserByEmailAsync(user.Email);
            return Ok(updatedUser);
        }

        [HttpPost]
        [Route("ChangePassword")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "Bad request"
                });
            }

            var user = await this.userHelper.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                return this.BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "This email is not assigned to any user."
                });
            }

            var result = await userHelper.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
            if (!result.Succeeded)
            {
                return this.BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = result.Errors.FirstOrDefault()?.Description
                });
            }

            return this.Ok(new Response
            {
                IsSuccess = true,
                Message = "The password was changed successfully!"
            });
        }

    }
}
