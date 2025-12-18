using System;
using System.Linq;
// using System.Threading.Tasks;
using NUnit;
// using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using DBSeniorLearnApp.Tests; // DatabaseManager
using static DBSeniorLearnApp.Tests.TestObjectFactory;
using DBSeniorLearnApp.DataAccess;
using DBSeniorLearnApp.DataAccess.Models;

namespace DBSeniorLearnApp.Services.Tests;

// [Ignore("Test isolation")]
public class MemberApplicationServiceTests
{
	private Member memberData = new Member
	{
        Firstname = "John",
        Lastname = "Smith",
        PhoneNumber = "0404123456",
        EmailAddress = "john@john.com",
        PaidStatus = true,
	};
	
    private Member testMember = new Member
    {
        Firstname = "John",
        Lastname = "Smith",
        PhoneNumber = "0404123456",
        EmailAddress = "john@john.com",
        PaidStatus = true,
		MemberNumber = "JF-7788",
		DateRegistered = System.DateTime.Now,
		DateUpdatedPaidStatus = System.DateTime.Now
    };
	
	
	[OneTimeSetUp]
	public void SuiteSetUp()
	{
		DatabaseManager.ResetDatabase();
	}
	
	// [Ignore("Test broken - member needs password now")]
	// Will have to change because member needs a password now.
    [Test]
    public void TestCanCreateMember_ShouldSaveToDatabase()
    {
		using (ServiceDbContext context = DatabaseManager.GenerateAppDbContext())
		{
			MemberApplicationService memberAppService = new MemberApplicationService(context);

			memberAppService.CreateMember(
				memberData.Firstname,
				memberData.Lastname,
				memberData.PhoneNumber,
				memberData.EmailAddress,
				"Password123",
				memberData.PaidStatus,
				SaveToDb: true
			);
			
			Member? newMember = context.Members.FirstOrDefault(m => m.EmailAddress == memberData.EmailAddress);
			
			Assert.NotNull(newMember);
			
			Assert.That(newMember.Firstname, Is.EqualTo("John"));
			Assert.That(newMember.Lastname, Is.EqualTo("Smith"));
			
			Assert.That(newMember.PaidStatus, Is.True);
			
			Assert.That(newMember.EmailAddress, Is.EqualTo("john@john.com"));
			Assert.That(newMember.PhoneNumber, Is.EqualTo("0404123456"));
			
			Assert.That(MemberNumberService.Validate(newMember.MemberNumber), Is.True);
			Assert.That(newMember.DateRegistered, Is.EqualTo(DateTime.Now).Within(30).Seconds);
			// Add DateUpdatedPaidStatus Assert
		}
    }
	
    [Test]
    public void TestCanRetrieveMember_ShouldReturnCorrectly()
    {
		using (ServiceDbContext context = DatabaseManager.GenerateAppDbContext())
		{
			MemberApplicationService memberAppService = new MemberApplicationService(context);
			
			Member addedMember = CreateStandardMember();
			
			context.Members.Add(addedMember);
			context.SaveChanges();
			
			Member? retrievedMember = memberAppService.RetrieveMember(addedMember.Id);
			
			Assert.NotNull(retrievedMember);
			Assert.That(addedMember, Is.EqualTo(retrievedMember));
		}
    }

    [Test]
    public void TestCanUpdateMember_ShouldUpdateAccordingly()
    {
		using (ServiceDbContext context = DatabaseManager.GenerateAppDbContext())
		{
			MemberApplicationService memberAppService = new MemberApplicationService(context);
			
			Member addedMember = CreateStandardMember();
			
			context.Members.Add(addedMember);
			context.SaveChanges();
			

			Member? updatedMember = memberAppService.UpdateMember(
				memberId: addedMember.Id,
				firstname: "Bobby",
				hasPaid: false
			);

			Assert.NotNull(updatedMember);
			Assert.That(updatedMember.Firstname, Is.Not.EqualTo("Bob"));
			Assert.That(updatedMember.Lastname, Is.EqualTo("Jobs"));
			Assert.That(updatedMember.PaidStatus, Is.Not.EqualTo(true));
		}
    }

    [Test]
    public void TestCanDeleteMember_ShouldRemoveFromDatabase()
    {
		using (ServiceDbContext context = DatabaseManager.GenerateAppDbContext())
		{
			MemberApplicationService memberAppService = new MemberApplicationService(context);

			context.Members.Add(testMember);
			context.SaveChanges();

			Member? deletedMember = memberAppService.DeleteMember(testMember.Id);
			Assert.That(deletedMember, Is.EqualTo(testMember));

			Member? attemptedRetrievedMember = memberAppService.RetrieveMember(deletedMember.Id);
			Assert.Null(attemptedRetrievedMember);
		}
    }


    [TearDown]
    public void Cleanup()
    {
		DatabaseManager.ResetDatabase();
		
		testMember = new Member
		{
			Firstname = "John",
			Lastname = "Smith",
			PhoneNumber = "0404123456",
			EmailAddress = "john@john.com",
			PaidStatus = true,
			MemberNumber = "JF-7788",
			DateRegistered = System.DateTime.Now,
			DateUpdatedPaidStatus = System.DateTime.Now
		};
    }
}






