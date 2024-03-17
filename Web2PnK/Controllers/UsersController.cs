using Core.Entities;
using Core.Enums;
using Core.Helpers;
using Core.Interfaces;
using Core.Interfaces.Service;
using Core.Models;
using Core.ResourcesFiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web2PnK.Models;

namespace Web2PnK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private UserManager<PnKUser> _userManager;
        private readonly IPnkUserService _pnkUserService;
        private readonly IEmailSender _emailSender;
        private readonly IUserPhotosService _userPhotosService;
        private readonly IServiceProvider _serviceProvider;
        private readonly AppConfig _config;

        public UsersController(UserManager<PnKUser> userManager,
                                IPnkUserService pnkUserService,
                                IEmailSender emailSender,
                                IServiceProvider serviceProvider,
                                AppConfig config,
                                IUserPhotosService userPhotosService)
        {
            _userManager = userManager;
            _pnkUserService = pnkUserService;
            _emailSender = emailSender;
            _config = config;
            _serviceProvider = serviceProvider;
            _userPhotosService = userPhotosService;
        }

        [HttpGet("{isCurrentPeriod}")]
        [Authorize]
        public async Task<IActionResult> GetUsers(bool isCurrentPeriod)
        {
            var users = await _pnkUserService.GetUsersRatingAsync(isCurrentPeriod);
            return Ok(users);
        }

        //[HttpGet("user/{userId}")]
        //[Authorize(Roles = UserRoles.Admin)]
        //public async Task<IActionResult> GetUser(string userId)
        //{
        //    PnKUser user = await _userManager.FindByIdAsync(userId.ToString());

        //    if (user != null)
        //    {
        //        return Ok(new Response
        //        {
        //            Value = new
        //            {
        //                id = user.Id,
        //                login = user.UserName,
        //                email = user.Email,
        //                description = user.Description,
        //                fullName = user.FullName,
        //                blockedStatus = user.LockoutEnd
        //            }
        //        });
        //    }

        //    return StatusCode(StatusCodes.Status404NotFound, new ErrorResponseModel(ResponseType.Error, DefaultLanguage.UserDoesntExist));
        //}

        [HttpGet("admin")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> GetAllUsersInfo()
        {
            try
            {
                var users = await _userManager.GetUsersInRoleAsync(nameof(AspNetRoles.User));

                return Ok(users.Select(x =>
                {
                    return new
                    {
                        id = x.Id,
                        login = x.UserName,
                        email = x.Email,
                        fullName = string.IsNullOrEmpty(x.FullName) ? x.UserName : x.FullName,
                        description = x.Description,
                        blockedStatus = x.LockoutEnd,
                        emailConfirmStatus = x.EmailConfirmed
                    };
                }
                ));
            }
            catch (Exception ex)
            {
                _ = Logger.Instance.LogErrorAsync(nameof(UsersController) + Constants.Arrow + nameof(GetAllUsersInfo), ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = nameof(ResponseType.Error), Message = DefaultLanguage.SomethingWrong });
            }
        }

        [HttpPatch("lockout")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> LockoutUser([FromBody] LockoutModel model)
        {
            try
            {
                PnKUser user = await _userManager.FindByIdAsync(model.UserId.ToString());
                PnKUser userAdmin = await _userManager.FindByNameAsync(nameof(AspNetRoles.Admin));

                if (model.UserId == userAdmin.Id)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new Response { Status = nameof(ResponseType.Error), Message = "Не дозволено" });
                }

                if (user != null)
                {
                    if (model.LockoutStatus)
                    {
                        var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.Add(_config.DefaultLockoutTimeSpan));
                        if (result.Succeeded)
                        {
                            return Ok(new Response
                            {
                                Status = nameof(ResponseType.Success),
                                Message = "Користувача заблоковано",
                            });
                        }
                    }
                    else
                    {
                        var result = await _userManager.SetLockoutEndDateAsync(user, null);
                        if (result.Succeeded)
                        {
                            return Ok(new Response
                            {
                                Status = nameof(ResponseType.Success),
                                Message = "Користувача розблоковано",
                            });
                        }
                    }
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = nameof(ResponseType.Error), Message = "Неможливо виконати дію" });
            }
            catch (Exception ex)
            {
                _ = Logger.Instance.LogErrorAsync(nameof(UsersController) + Constants.Arrow + nameof(LockoutUser), ex);
                return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = nameof(ResponseType.Error), Message = DefaultLanguage.SomethingWrong });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                PnKUser user = new()
                {
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = model.Nickname,
                    FullName = model.FullName,
                    Description = model.Description
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                    return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponseModel(ResponseType.Error, DefaultLanguage.UserCreationError));

                await UploadPhotoAsync(null, model.Email, model.Photo);

                result = await _userManager.AddToRoleAsync(user, nameof(AspNetRoles.User));
                if (!result.Succeeded)
                    return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponseModel(ResponseType.Error, DefaultLanguage.UserCreationError));

                var isActionAllow = _pnkUserService.IsAllowAction(user.Id, ActionType.ConfirmEmail);
                if (isActionAllow.Result)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    SengingModelForConfirmEmail registrationUserForConfirmEmail = new()
                    {
                        Link = Request.CombineToFrontUrl("user/email/confirm", new { token, email = user.Email }),
                        RootLink = Request.RootFrontUrl(),
                        UserId = user.Id,
                        Nickname = model.Nickname,
                        UserEmail = user.Email,
                        TemplatePath = "Views\\EmailTemplates\\RegistrationUser.cshtml",
                        Title = "Підтвердження електронної адреси",
                    };

                    _ = Logger.Instance.LogInfoAsync(nameof(UsersController) + Constants.Arrow + nameof(Register), registrationUserForConfirmEmail);

                    _ = Task.Run(async () =>
                    {
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();
                            await emailSender.SendEmail(registrationUserForConfirmEmail);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                _ = Logger.Instance.LogErrorAsync(nameof(UsersController) + Constants.Arrow + nameof(Register), ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = nameof(ResponseType.Error), Message = DefaultLanguage.SomethingWrong });
            }

            return Json(new Response { Status = nameof(ResponseType.Success), Message = string.Format(DefaultLanguage.UserCreationSuccess, model.Nickname) });
        }

        [HttpPost(nameof(ConfirmEmail))]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailModel confirmEmail)
        {
            var user = await _userManager.FindByEmailAsync(confirmEmail.Email);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, confirmEmail.Token);
                if (result.Succeeded)
                {
                    return Ok(new Response { Status = nameof(ResponseType.Success), Message = DefaultLanguage.EmailConfirmSuccess });
                }

            }
            return StatusCode(StatusCodes.Status404NotFound, new ErrorResponseModel(ResponseType.Error, DefaultLanguage.UserDoesntExist));
        }

        [HttpPost(nameof(ForgotPassword))]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordResponseModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {

                    var isActionAllow = _pnkUserService.IsAllowAction(user.Id, ActionType.ResetPassword);
                    if (isActionAllow.Result)
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        SendingModelForForgotPassword userForgotPassword = new()
                        {
                            Link = Request.CombineToFrontUrl("user/password/submit", new { token, email = user.Email }),
                            RootLink = Request.RootFrontUrl(),
                            Nickname = user.UserName,
                            UserEmail = user.Email,
                            UserId = user.Id,
                            TemplatePath = "Views\\EmailTemplates\\ForgotPassword.cshtml",
                            Title = "Скидання паролю",
                            TokenLifeTo = DateTime.Now.Add(_config.ResetPasswordTokenLifeTime)
                        };

                        await _emailSender.SendEmail(userForgotPassword);
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status405MethodNotAllowed, new Response { Status = nameof(ResponseType.Error), Message = isActionAllow.Message });
                    }
                }
            }
            catch (Exception ex)
            {
                _ = Logger.Instance.LogErrorAsync(nameof(UsersController) + Constants.Arrow + nameof(ForgotPassword), ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = nameof(ResponseType.Error), Message = DefaultLanguage.SomethingWrong });
            }

            return Json(new Response { Status = nameof(ResponseType.Success), Message = String.Format(DefaultLanguage.PasswordChangeRequest, model.Email) });
        }

        [HttpPost(nameof(ResendVerify))]
        public async Task<IActionResult> ResendVerify([FromBody] ForgotPasswordResponseModel emailToVerify)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(emailToVerify.Email);
                if (user != null)
                {

                    var isActionAllow = _pnkUserService.IsAllowAction(user.Id, ActionType.ConfirmEmail);
                    if (isActionAllow.Result)
                    {
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                        SengingModelForConfirmEmail registrationUserForConfirmEmail = new()
                        {
                            Link = Request.CombineToFrontUrl("user/email/confirm", new { token, email = user.Email }),
                            RootLink = Request.RootFrontUrl(),
                            UserId = user.Id,
                            Nickname = user.UserName,
                            UserEmail = user.Email,
                            TemplatePath = "Views\\EmailTemplates\\RegistrationUser.cshtml",
                            Title = "Підтвердження електронної адреси",
                        };

                        await _emailSender.SendEmail(registrationUserForConfirmEmail);
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status405MethodNotAllowed, new Response { Status = nameof(ResponseType.Error), Message = isActionAllow.Message });
                    }
                }
            }
            catch (Exception ex)
            {
                _ = Logger.Instance.LogErrorAsync(nameof(UsersController) + Constants.Arrow + nameof(ResendVerify), ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = nameof(ResponseType.Error), Message = DefaultLanguage.SomethingWrong });
            }

            return Ok(new Response { Status = nameof(ResponseType.Success), Message = DefaultLanguage.EmailConfirmWasSendAgain });
        }


        [HttpPost(nameof(ResetPassword))]
        public async Task<IActionResult> ResetPassword(ResetPassword resetPassword)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(resetPassword.Email);

                if (user != null)
                {
                    var resetRPassResult = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);
                    if (!resetRPassResult.Succeeded)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponseModel(ResponseType.Error, DefaultLanguage.TokenIsNotValid));
                    }
                }
            }
            catch (Exception ex)
            {
                _ = Logger.Instance.LogErrorAsync(nameof(UsersController) + Constants.Arrow + nameof(ResetPassword), ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = nameof(ResponseType.Error), Message = DefaultLanguage.SomethingWrong });
            }

            return Ok(new Response { Status = nameof(ResponseType.Success), Message = DefaultLanguage.PasswordChangeSuccess });
        }

        [HttpGet("email/{email}")]
        public IActionResult CheckEmail(string email)
        {
            return Json(new { isSuccess = !_pnkUserService.IsEmailUsed(email), messages = DefaultLanguage.EmailIsUsed });
        }

        [HttpGet("nickname/{nickname}")]
        public IActionResult CheckNickname(string nickname)
        {
            return Json(new { isSuccess = !_pnkUserService.IsNicknameUsed(nickname), messages = DefaultLanguage.NicknameIsUsed });
        }

        [Authorize]
        [HttpPatch("password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            try
            {
                if (model.UserId.Equals(Guid.Empty))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = nameof(ResponseType.Error), Message = "Юзер Id обов'язкове поле" });
                }

                if (model.UserId.ToString() != User.GetUserId() && !User.IsInRole(nameof(AspNetRoles.Admin)))
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new Response { Status = nameof(ResponseType.Error), Message = "Не дозволено" });
                }

                PnKUser user = await _userManager.FindByIdAsync(model.UserId.ToString());

                if (user != null)
                {
                    if (User.IsInRole(nameof(AspNetRoles.Admin)))
                    {
                        user.PasswordHash = null;
                        var result = await _userManager.AddPasswordAsync(user, model.NewPassword);
                        if (result.Succeeded)
                        {
                            return StatusCode(StatusCodes.Status200OK, new Response { Status = nameof(ResponseType.Success), Message = "Пароль змінено" });
                        }
                    }
                    else
                    {
                        bool isOldPasswordValid = await _userManager.CheckPasswordAsync(user, model.OldPassword);
                        if (isOldPasswordValid)
                        {
                            if (model.NewPassword == model.ConfirmPassword)
                            {
                                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                                if (result.Succeeded)
                                {
                                    return StatusCode(StatusCodes.Status200OK, new Response { Status = nameof(ResponseType.Success), Message = "Пароль змінено" });
                                }
                                return StatusCode(StatusCodes.Status400BadRequest, result);
                            }
                            return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = nameof(ResponseType.Error), Message = "Паролі не співпадають" });
                        }
                        return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = nameof(ResponseType.Error), Message = "Теперішній пароль не вірний" });
                    }
                }

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = nameof(ResponseType.Error), Message = DefaultLanguage.SomethingWrong });
            }
            catch (Exception ex)
            {
                _ = Logger.Instance.LogErrorAsync(nameof(UsersController) + Constants.Arrow + nameof(ChangePassword), ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = nameof(ResponseType.Error), Message = DefaultLanguage.SomethingWrong });
            }

        }

        [Authorize]
        [HttpPatch]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserModel model)
        {
            try
            {
                if (model.UserId.Equals(Guid.Empty))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = nameof(ResponseType.Error), Message = "Юзер Id обов'язкове поле" });
                }

                if (model.UserId.ToString() != User.GetUserId() && !User.IsInRole(nameof(AspNetRoles.Admin)))
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new Response { Status = nameof(ResponseType.Error), Message = "Не дозволено" });
                }

                PnKUser user = await _userManager.FindByIdAsync(model.UserId.ToString());

                if (user != null)
                {
                    user.UserName = model.Login;
                    user.FullName = model.FullName;
                    user.Description = model.Description;

                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return Ok(new Response
                        {
                            Status = nameof(ResponseType.Success),
                            Message = "Дані успішно оновлено",
                            Value = new
                            {
                                id = user.Id,
                                login = user.UserName,
                                email = user.Email,
                                description = user.Description,
                                fullName = user.FullName,
                                photo = await _userPhotosService.GetPhotoAsync(user.Id),
                                blockedStatus = user.LockoutEnd
                            }
                        });
                    }

                }

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = nameof(ResponseType.Error), Message = "Неможливо оновити дані" });
            }
            catch (Exception ex)
            {
                _ = Logger.Instance.LogErrorAsync(nameof(UsersController) + Constants.Arrow + nameof(UpdateUser), ex);
                return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = nameof(ResponseType.Error), Message = DefaultLanguage.SomethingWrong });
            }
        }

        [Authorize]
        [HttpPatch(nameof(UpdatePhoto))]
        public async Task<IActionResult> UpdatePhoto(UserPhotosCore userPhotosCore)
        {
            await UploadPhotoAsync(userPhotosCore.UserId, null, userPhotosCore.Photo);

            return Ok(new Response
            {
                Status = nameof(ResponseType.Success),
                Message = "Фото успішно оновлено",
                Value = new
                {
                    id = userPhotosCore.UserId,
                    photo = userPhotosCore.Photo
                }
            });
        }


        [Authorize]
        [HttpGet("{userId}/photo")]
        public async Task<IActionResult> GetPhoto(Guid userId)
        {
            try
            {
                var resultPhoto = await _userPhotosService.GetPhotoAsync(userId);
                if (resultPhoto != null)
                {
                    return Ok(resultPhoto);
                }

                return StatusCode(StatusCodes.Status404NotFound, new Response { Status = nameof(ResponseType.Error), Message = DefaultLanguage.PhotoNotFound });
            }
            catch (Exception ex)
            {
                _ = Logger.Instance.LogErrorAsync(nameof(UsersController) + Constants.Arrow + "{userId}/photo", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = nameof(ResponseType.Error), Message = DefaultLanguage.SomethingWrong });
            }
        }

        private async Task<bool> UploadPhotoAsync(Guid? userId, string? email, string? photo)
        {
            try
            {
                var dbUser = userId != null && !userId.Value.Equals(Guid.Empty) ?
                    await _userManager.FindByIdAsync(userId.Value.ToString()) :
                    await _userManager.FindByEmailAsync(email);

                if (photo != null)
                {

                    if (dbUser != null)
                    {
                        UserPhotosCore userPhotos = new()
                        {
                            UserId = dbUser.Id,
                            Photo = photo
                        };

                        return await _userPhotosService.UploadPhotoAsync(userPhotos.UserId, userPhotos.Photo);
                    }
                }
                else
                {
                    await DeletePhoto(dbUser.Id);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            return false;
        }

        [HttpDelete("{userId}/photo")]
        [Authorize]
        public async Task<IActionResult> DeletePhoto(Guid userId)
        {
            try
            {
                if (userId.ToString() != User.GetUserId() && !User.IsInRole(nameof(AspNetRoles.Admin)))
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new Response { Status = nameof(ResponseType.Error), Message = "Не дозволено" });
                }

                var result = await _userPhotosService.DeletePhotoAsync(userId);
                if (result)
                {
                    return Ok(new Response { Status = nameof(ResponseType.Success), Message = "Фото видалено" });
                }

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = nameof(ResponseType.Error), Message = "Видалення не здійснено !" });
            }
            catch (Exception ex)
            {
                _ = Logger.Instance.LogErrorAsync(nameof(UsersController) + Constants.Arrow + nameof(UpdateUser), ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = nameof(ResponseType.Error), Message = DefaultLanguage.SomethingWrong });
            }
        }
    }
}
