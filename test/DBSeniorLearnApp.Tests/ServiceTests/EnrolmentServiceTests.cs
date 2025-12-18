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
public class EnrolmentServiceTests
{
	[OneTimeSetUp]
	public void SuiteSetUp()
	{
		DatabaseManager.ResetDatabase();
	}
	
    [Test]
    public void TestCanEnrolMemberInCourse_ShouldAddCourseEnrolmentToDb() {
		using (ServiceDbContext context = DatabaseManager.GenerateAppDbContext()) {
			EnrolmentService enroller = new EnrolmentService(context);
			
			Member member = CreateStandardMember();
			// CreateCourse() creates the same object that CreateStandardMember() creates
			member.MemberNumber = "BS-1001";
			
			Course course = CreateCourse();
			
			context.Members.Add(member);
			context.Courses.Add(course);
			
			context.SaveChanges();
			
			
			CourseEnrolment? enrolment = enroller.EnrolMember(member.Id, course.Id);
			
			Assert.NotNull(enrolment);
			
			
			CourseEnrolment? retrievedEnrolment = context.CourseEnrolments.FirstOrDefault(r => r.Id == enrolment.Id);
			
			Assert.NotNull(retrievedEnrolment);
			
			
			Assert.That(retrievedEnrolment, Is.EqualTo(enrolment));
			
			Assert.That(retrievedEnrolment.MemberId, Is.EqualTo(member.Id));
			Assert.That(retrievedEnrolment.CourseId, Is.EqualTo(course.Id));
		}
	}
	
	[Test]
	public void TestCanUnenrolMember_ShouldRemoveCourseEnrolment() {
		using (ServiceDbContext context = DatabaseManager.GenerateAppDbContext()) {
			EnrolmentService enroller = new EnrolmentService(context);
			
			Member member = CreateStandardMember();
			// CreateCourse() creates the same object that CreateStandardMember() creates
			member.MemberNumber = "BS-1001";
			
			Course course = CreateCourse();
			
			context.Members.Add(member);
			context.Courses.Add(course);
			
			CourseEnrolment enrolment = new CourseEnrolment(){
				Course = course,
				Member = member
			};
			context.CourseEnrolments.Add(enrolment);
			context.SaveChanges();
			
			
			CourseEnrolment? unenrolment = enroller.UnenrolMember(enrolment.Id);
			
			Assert.NotNull(unenrolment);
			
			
			CourseEnrolment? retrievedEnrolment = context.CourseEnrolments.FirstOrDefault(r => r.Id == unenrolment.Id);
			
			Assert.Null(retrievedEnrolment);
		}
	}

    [TearDown]
    public void Cleanup()
    {
		DatabaseManager.ResetDatabase();
    }
}






