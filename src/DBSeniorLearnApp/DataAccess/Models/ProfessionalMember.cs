using System.Collections.Generic;

namespace DBSeniorLearnApp.DataAccess.Models;

public class ProfessionalMember
{
	public int StandardMemberId { get; set; } // foreign key to Member table
	public string? Description { get; set; }
	public bool Deactivated { get; set; } = false;
	public bool IsHonoraryMember { get; set; } = false;

	//Navigation Properties
	public Member StandardMember { get; set; }
	public ICollection<Course> Courses { get; set; }
}
