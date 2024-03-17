using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Core.Helpers
{
	public static class ClaimsHelper
	{
		public static string? GetUserId(this ClaimsPrincipal principal)
		{
			if (principal == null)
			{
				throw new ArgumentNullException(nameof(principal));
			}
			var claim = principal.FindFirst(ClaimTypes.NameIdentifier);
			return claim != null ? claim.Value : null;
		}
	}
}
