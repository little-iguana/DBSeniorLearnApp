using DBSeniorLearnApp.DataAccess.Models;

namespace DBSeniorLearnApp.Services.Interfaces;

public interface IMemberApplicationService
{
	public Member? CreateMember(
			string fn,
			string ln,
			string phoneNo,
			string email,
			string password,
			bool hasPaid = false,
			bool SaveToDb = false
		);
	
	public void SaveMemberToDatabase(Member newMember);

	public Member? RetrieveMember(int memberId);
	
	[System.Obsolete("Member number is no longer the primary way of retrieving members - use Id instead.")]
	public Member? RetrieveMemberByMemberNumber(string memNumber);
	
	public Member? RetrieveMemberByEmail(string email);
	
	public Member? UpdateMember(
			int memberId,
			string? firstname = null,
			string? lastname = null,
			string? phoneNo = null,
			string? email = null,
			bool? hasPaid = null
		);
	
	public Member? DeleteMember(int memberId);
}
