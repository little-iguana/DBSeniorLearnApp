using System.ComponentModel.DataAnnotations;

namespace DBSeniorLearnApp.UI.Models;

public class RenewRegistrationViewModel {
	
	[Required]
	[Display(Name = "Email address")]
	public string EmailAddress { get; set; }
	
	[Required]
	[Display(Name = "Name on card")]
	public string CardName { get; set; }
	
	[Required]
	[Display(Name = "Card number")]
	[StringLength(19, MinimumLength = 8, ErrorMessage = "Card number must be between 8-19 digits")]
	public string CardNumber { get; set; }
	
	[Required]
	[Display(Name = "CCV number")]
	[StringLength(3, MinimumLength = 3, ErrorMessage = "Invalid CCV")]
	public string CardCCV { get; set; }
	
	[Required]
	[Display(Name = "Card expiry (mm/yy)")]
	[StringLength(5, ErrorMessage = "Expiry must be in the form MM/YY, and be in the future")]
	public string CardExpiry { get; set; }
	
	public string ErrorMessage { get; set; } = "";
}
