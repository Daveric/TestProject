
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
    [ApiController]
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;

        public AccountController(IUserHelper userHelper, IMailHelper mailHelper)
        {
            _userHelper = userHelper;
            _mailHelper = mailHelper;
        }

        [HttpPost]
        public async Task<IActionResult> PostUser([FromBody] NewUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "Bad request"
                });
            }

            var user = await _userHelper.GetUserByEmailAsync(request.Email);
            if (user != null)
            {
                return BadRequest(new Response
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

            var result = await _userHelper.AddUserAsync(user, request.Password);
            if (result != IdentityResult.Success)
            {
                return BadRequest(result.Errors.FirstOrDefault()?.Description);
            }

            var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
            var tokenLink = Url.Action("ConfirmEmail", "Account", new
            {
                userid = user.Id,
                token = myToken
            }, protocol: HttpContext.Request.Scheme);

            _mailHelper.SendMail(request.Email, "Email confirmation", $"<h1>Email Confirmation</h1>" +
                $"To allow the user, " +
                $"plase click in this link:</br></br><a href = \"{tokenLink}\">Confirm Email</a>");

            return Ok(new Response
            {
                IsSuccess = true,
                Message = "A Confirmation email was sent. Please confirm your account and log into the App."
            });
        }

        [HttpPost]
        [Route("RecoverPassword")]
        public async Task<IActionResult> RecoverPasswordApi([FromBody] RecoverPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "Bad request"
                });
            }

            var user = await _userHelper.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "This email is not assigned to any user."
                });
            }

            var myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);
            var link = Url.Action("ResetPassword", "Account", new { token = myToken }, protocol: HttpContext.Request.Scheme);
            _mailHelper.SendMail(request.Email, "Password Reset", $"<h1>Recover Password</h1>" +
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
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "Bad request"
                });
            }

            var user = await _userHelper.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                return BadRequest(new Response
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
                return BadRequest(ModelState);
            }

            var userEntity = await _userHelper.GetUserByEmailAsync(user.Email);
            if (userEntity == null)
            {
                return BadRequest("User not found.");
            }


            userEntity.FirstName = user.FirstName;
            userEntity.LastName = user.LastName;
            userEntity.Address = user.Address;
            userEntity.PhoneNumber = user.PhoneNumber;

            var response = await _userHelper.UpdateUserAsync(userEntity);
            if (!response.Succeeded)
            {
                return BadRequest(response.Errors.FirstOrDefault()?.Description);
            }

            var updatedUser = await _userHelper.GetUserByEmailAsync(user.Email);
            return Ok(updatedUser);
        }

        [HttpPost]
        [Route("ChangePassword")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ChangePasswordApi([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "Bad request"
                });
            }

            var user = await _userHelper.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "This email is not assigned to any user."
                });
            }

            var result = await _userHelper.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = result.Errors.FirstOrDefault()?.Description
                });
            }

            return Ok(new Response
            {
                IsSuccess = true,
                Message = "The password was changed successfully!"
            });
        }

        [HttpPost("AccessOn")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AccessToApplicationsOn([FromQuery] string email)
        {
            var user = await _userHelper.GetUserByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("User not found.");
            }
            await _userHelper.AddUserToRoleAsync(user, "ThirdAuthLevel");
            user.HasAccess = await _userHelper.IsUserInRoleAsync(user, "ThirdAuthLevel");
            var response = await _userHelper.UpdateUserAsync(user);
            if (!response.Succeeded)
            {
                return BadRequest(response.Errors.FirstOrDefault()?.Description);
            }
            return Ok();
        }

        [HttpPost("AccessOff")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AccessToApplicationsOff([FromQuery] string email)
        {
            var user = await _userHelper.GetUserByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("User not found.");
            }
            await _userHelper.RemoveUserFromRoleAsync(user, "ThirdAuthLevel");
            user.HasAccess = await _userHelper.IsUserInRoleAsync(user, "ThirdAuthLevel");
            var response = await _userHelper.UpdateUserAsync(user);
            if (!response.Succeeded)
            {
                return BadRequest(response.Errors.FirstOrDefault()?.Description);
            }
            return Ok();
        }
    }
}
