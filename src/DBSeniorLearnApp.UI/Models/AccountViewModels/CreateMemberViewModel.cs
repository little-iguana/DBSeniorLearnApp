using System.ComponentModel.DataAnnotations;

namespace DBSeniorLearnApp.UI.Models;

public class CreateMemberViewModel {
	
	[Display(Name = "First Name")]
	[StringLength(255, ErrorMessage = "First name cannot be longer than 255 characters.")]
	public string Firstname { get; set; }
	
	[Required]
	[Display(Name = "Last Name")]
	[StringLength(255, ErrorMessage = "Last name cannot be longer than 255 characters.")]
	public string Lastname { get; set; }
	
	[Display(Name = "Phone Number")]
	[Phone(ErrorMessage = "Please enter a valid phone number")]
	public string PhoneNumber { get; set; }
	
	[Required]
	[Display(Name = "Email Address")]
	[EmailAddress(ErrorMessage = "Invalid email address")]
	public string EmailAddress { get; set; }
	
	[Required]
	// [DataType(DataType.Password)]
	[StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
	public string Password { get; set; }

	[Required]
	// [DataType(DataType.Password)]
	[Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
	[Display(Name = "Confirm Password")]
	public string ConfirmPassword { get; set; }
	
	public override string ToString() {
		return $"CreateMemberViewModel: {Firstname} {Lastname}";
	}
}
