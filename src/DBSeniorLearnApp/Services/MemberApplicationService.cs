using System.Linq;
using DBSeniorLearnApp.DataAccess.Models;
using DataAccess = DBSeniorLearnApp.DataAccess;
using Models = DBSeniorLearnApp.DataAccess.Models;

namespace DBSeniorLearnApp.Services;

// Refactor if this becomes overly responsible
public class MemberApplicationService : Interfaces.IMemberApplicationService
{
	private DataAccess::ServiceDbContext _context;
	
	public MemberApplicationService(DataAccess::ServiceDbContext context)
	{
		this._context = context;
	}

	// Create
	public Models::Member? CreateMember(
			string fn,
			string ln,
			string phoneNo,
			string email,
			string password,
			bool hasPaid = false,
			bool SaveToDb = false
		)
	{
		Models::Member newMember = new Models::Member()
		{
			Firstname = fn,
			Lastname = ln,

			PhoneNumber = phoneNo,
			EmailAddress = email,

			PaidStatus = hasPaid,

			MemberNumber = MemberNumberService.NewNumber(_context),

			DateRegistered = System.DateTime.Now,
			DateUpdatedPaidStatus = System.DateTime.Now,

			Password = password
		};
		
		if (SaveToDb) {
			_context.Members.Add(newMember);
			_context.SaveChanges();
		}
		
		return newMember;
	}
	
	public void SaveMemberToDatabase(Member newMember)
    {
        _context.Members.Add(newMember);
		_context.SaveChanges();
    }

	// Retrieve
	public Models::Member? RetrieveMember(int memberId)
	{

		Models::Member? member = _context.Members.FirstOrDefault(m => m.Id == memberId);

		if (member == null)
		{
			return null;
		}

		// update PaidStatus before returning member
		PassiveUpdateMemberPaymentStatus(member);

		return member;
	}
	
	[System.Obsolete("Member number is no longer the primary way of retrieving members - use Id instead.")]
	public Models::Member? RetrieveMemberByMemberNumber(string memNumber)
    {
		Models::Member? member = _context.Members.FirstOrDefault(m => m.MemberNumber == memNumber);
		if (member == null)
		{
			return null;
		}
		return member;
    }
	
	public Models::Member? RetrieveMemberByEmail(string email) {
		
		Models::Member? member = _context.Members.FirstOrDefault(m => m.EmailAddress == email);
		
		if (member == null) {
			return null;
		}
		
		PassiveUpdateMemberPaymentStatus(member);
		
		return member;
	}
	
	// Update
	public Models::Member? UpdateMember(
			int memberId,
			string? firstname = null,
			string? lastname = null,
			string? phoneNo = null,
			string? email = null,
			bool? hasPaid = null
		)
	{
		// null is treated as no update
		
		Models::Member? memberToUpdate = _context.Members.FirstOrDefault(m => m.Id == memberId);
		
		if (memberToUpdate == null) {
			return null;
		}
		
		memberToUpdate.Firstname = firstname ?? memberToUpdate.Firstname;
		memberToUpdate.Lastname = lastname ?? memberToUpdate.Lastname;
		memberToUpdate.PhoneNumber = phoneNo ?? memberToUpdate.PhoneNumber;
		memberToUpdate.EmailAddress = email ?? memberToUpdate.EmailAddress;
		memberToUpdate.PaidStatus = hasPaid ?? memberToUpdate.PaidStatus;
		
		PassiveUpdateMemberPaymentStatus(memberToUpdate);
		
		_context.SaveChanges();
		
		return memberToUpdate;
	}
	
	// Delete
	public Models::Member? DeleteMember(int memberId) {
		
		Models::Member? memberToRemove = _context.Members.FirstOrDefault(m => m.Id == memberId);
		
		if (memberToRemove == null) {
			return null;
		}
		_context.Members.Remove(memberToRemove);
		_context.SaveChanges();
		
		return memberToRemove;
	}
	
	
	private void PassiveUpdateMemberPaymentStatus(Models::Member member) {
		
		// if they registered less than a year ago, membership has not expired
		if (member.DateRegistered.AddYears(1) > System.DateTime.Now) {
			return;
		}
		
		// if updating their paid status was less than a year ago, membership has not expired
		if (member.DateUpdatedPaidStatus.AddYears(1) > System.DateTime.Now) {
			return;
		}
		
		// if this point has been reached, membership must have expired
		member.PaidStatus = false;
		return;
	}
}





