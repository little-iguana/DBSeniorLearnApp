using System.ComponentModel.DataAnnotations;

namespace DBSeniorLearnApp.UI.Models;

public class LoginMemberViewModel {
	
	[Required]
	[EmailAddress(ErrorMessage = "Invalid email address")]
	public string EmailAddress { get; set; }
	
	[Required]
	// [DataType(DataType.Password)]
	public string Password { get; set; }
	
	public override string ToString() {
		return $"LoginMemberViewModel: {EmailAddress}";
	}
}
