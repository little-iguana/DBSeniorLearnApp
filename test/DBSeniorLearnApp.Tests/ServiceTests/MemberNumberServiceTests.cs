using NUnit;
using DBSeniorLearnApp.Tests; // DatabaseManager
using DataAccess = DBSeniorLearnApp.DataAccess;

namespace DBSeniorLearnApp.Services.Tests;

// [Ignore("Test isolation")]
public class MemberNumberServiceTests
{
	[Test]
	public void TestValidMemberNumber_ShouldReturnTrue()
	{
		string ValidMemberNumber = "BC-4567";

		Assert.True(MemberNumberService.Validate(ValidMemberNumber));
	}
	
	[Test]
	public void TestGeneratedMemberNumber_ShouldReturnTrue()
	{
		using (DataAccess::ServiceDbContext context = DatabaseManager.GenerateAppDbContext())
		{
			string ValidMemberNumber = MemberNumberService.NewNumber(context);

			Assert.True(MemberNumberService.Validate(ValidMemberNumber));
		}

	}
	[Test]
	public void TestInvalidLengthMemberNumber_ShouldReturnFalse()
	{
		string InvalidLengthMemberNumber = "AAA-00000";

		Assert.False(MemberNumberService.Validate(InvalidLengthMemberNumber));
	}

	[TestCase("1A-0000")]
	[TestCase("A1-0000")]
	[TestCase("AA+0000")]
	[TestCase("AA-A000")]
	[TestCase("AA-0A00")]
	[TestCase("AA-00A0")]
	[TestCase("AA-000A")]
	[TestCase("AA-000+")]
	[TestCase("AAA-000")]
	public void TestInvalidMemberNumber_ShouldReturnFalse(string invalidMemberNumber)
	{
		Assert.False(MemberNumberService.Validate(invalidMemberNumber));
	}
}





