using System.ComponentModel.DataAnnotations;

namespace DBSeniorLearnApp.UI.Models;

public class CreateProfessionalMemberViewModel {
	[Required]
	public string? SmMemberNumber { get; set; }
	public string? Description { get; set; }
}
