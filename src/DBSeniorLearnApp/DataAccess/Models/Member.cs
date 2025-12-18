using System.Collections.Generic;

namespace DBSeniorLearnApp.DataAccess.Models;

public class Member
{
	public int Id { get; set; }
	
	public string Firstname { get; set; }
	public string Lastname { get; set; }
	public string PhoneNumber { get; set; }
	public string EmailAddress { get; set; }
	
	public bool PaidStatus { get; set; }
	public string MemberNumber { get; set; }
	public string? Password { get; set; }
	
	public System.DateTime DateRegistered { get; set; }
	public System.DateTime DateUpdatedPaidStatus { get; set; }

	//Navigation Properties
	public ProfessionalMember? ProfessionalMember { get; set; }
	public ICollection<CourseEnrolment>? CourseEnrolments { get; set; }
	
	
	public override string ToString() {
		return $"Member {Id}: {Firstname} {Lastname} ({MemberNumber})";
	}
}
