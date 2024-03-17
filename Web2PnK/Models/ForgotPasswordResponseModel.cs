using System.ComponentModel.DataAnnotations;

namespace Web2PnK.Models
{
	public class ForgotPasswordResponseModel
	{
		[Required]
		public string? Email { get; set; }
	}
}
