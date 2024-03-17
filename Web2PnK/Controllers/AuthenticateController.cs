using Core.Entities;
using Core.Enums;
using Core.Helpers;
using Core.Interfaces.Service;
using Core.Models;
using Core.ResourcesFiles;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Web2PnK.Models;

namespace Web2PnK.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthenticateController : Controller
	{
		private readonly UserManager<PnKUser> _userManager;
		private readonly SignInManager<PnKUser> _signInManager;
		private readonly IConfiguration _configuration;
		private readonly IPnkUserService _pnkUserService;
		private readonly AppConfig _appConfig;

		public AuthenticateController(
			AppConfig appConfig,
			UserManager<PnKUser> userManager,
			SignInManager<PnKUser> signInManager,
			IConfiguration configuration,
            IPnkUserService pnkUserService)
			
		{
			_appConfig = appConfig;
			_userManager = userManager;
			_configuration = configuration;
			_signInManager = signInManager;
			_pnkUserService = pnkUserService;
		}

		[HttpPost]
		[Route("login")]
		public async Task<IActionResult> Login([FromBody] LoginModel model)
		{
			var user = await _userManager.FindByEmailAsync(model.UserNameOrEmail) ??
					   await _userManager.FindByNameAsync(model.UserNameOrEmail);

			if (user != null)
			{
				var isActionAllow = _pnkUserService.IsAllowAction(user.Id, ActionType.Login);
				if (isActionAllow.Result)
				{
					var signIn = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
					if (signIn.Succeeded)
					{
						var userRoles = await _userManager.GetRolesAsync(user);

						var authClaims = new List<Claim>
						{
							new Claim(ClaimTypes.Name, user.UserName),
							new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
						};

						foreach (var userRole in userRoles)
						{
							authClaims.Add(new Claim(ClaimTypes.Role, userRole));
						}

						var token = GetToken(authClaims);

						var requestUser = new
						{
							id = user.Id,
							login = user.UserName,
							email = user.Email,
							description = user.Description,
							fullName = string.IsNullOrEmpty(user.FullName) ? user.UserName : user.FullName,
							isAdmin = await _userManager.IsInRoleAsync(user, "Admin")
						};

						_ = Logger.Instance.LogInfoAsync(nameof(AuthenticateController) + Constants.Arrow + nameof(Login), requestUser);
                        
						return Ok(new
						{
							token = new JwtSecurityTokenHandler().WriteToken(token),
							expiration = token.ValidTo,
							user = requestUser
						});
					}
					else if (signIn.IsNotAllowed)
					{
						return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = nameof(ResponseType.EmailConfirmError), Message = DefaultLanguage.ConfirmEmail, Value = user.Email });
					}
					else if (signIn.IsLockedOut)
					{
						return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = nameof(ResponseType.LockoutError), Message = DefaultLanguage.UserBlocked });
					}
                }
                else
				{
					return StatusCode(StatusCodes.Status405MethodNotAllowed, new Response { Status = nameof(ResponseType.Error), Message = isActionAllow.Message });
				}
			}

			return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = nameof(ResponseType.Error), Message = DefaultLanguage.BadLoginOrPassword });
		}

		private JwtSecurityToken GetToken(List<Claim> authClaims)
		{
			var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKeyManager.Instance.GetSecretKey()));

			var token = new JwtSecurityToken(
				issuer: _configuration["JWT:ValidIssuer"],
				audience: _configuration["JWT:ValidAudience"],
				expires: DateTime.Now.Add(_appConfig.BearerTokenLifeSpan),
				claims: authClaims,
				signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
			);

			return token;
		}
	}
}