using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Web2PnK.Helpers
{
	public class CustomPasswordResetTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
	{
		public CustomPasswordResetTokenProvider(IDataProtectionProvider dataProtectionProvider,
			IOptions<CustomPasswordResetTokenProviderOptions> options,
			ILogger<DataProtectorTokenProvider<TUser>> logger)
			: base(dataProtectionProvider, options, logger)
		{
		}
	}

	public class CustomPasswordResetTokenProviderOptions
		: DataProtectionTokenProviderOptions
	{

	}
}
