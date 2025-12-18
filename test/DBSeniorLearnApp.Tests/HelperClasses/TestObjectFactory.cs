using DBSeniorLearnApp.DataAccess;
using DBSeniorLearnApp.DataAccess.Models;

namespace DBSeniorLearnApp.Tests;

internal static class TestObjectFactory {
	
	internal static Course TestCourseData = new Course() {
		Title = "How to bake a cake",
		Description = "Cake baking 101",
		Prerequisites = null,
		StartTime = System.DateTime.Now,
		EndTime = System.DateTime.Now.AddMinutes(Duration),
		// ReferenceCode = "BK-101",
		Status = "Draft"
	};
	internal static int Duration = 120;
	internal static int Tolerance = 30;
	
	internal static Member CreateStandardMember() {
		return new Member() {
			Firstname = "Bob",
			Lastname = "Jobs",
			PhoneNumber = "0404123456",
			EmailAddress = "bob.jobs@hobbs.com",
			
			PaidStatus = true,
			MemberNumber = "YY-8888",
			
			DateRegistered = System.DateTime.Now,
			DateUpdatedPaidStatus = System.DateTime.Now
		};
	}
	
	internal static ProfessionalMember CreateProfessionalMember() {
		return new ProfessionalMember() {
			Description = "",
			
			StandardMember = CreateStandardMember()
		};
	}
	
	internal static Course CreateCourse(ProfessionalMember? instructor = null) {
		
		if (instructor != null) {
			return new Course() {
				Title = TestCourseData.Title,
				Description = TestCourseData.Description,
				Prerequisites = TestCourseData.Prerequisites,
				StartTime = System.DateTime.Now,
				EndTime = System.DateTime.Now.AddMinutes(Duration),
				// ReferenceCode = TestCourseData.ReferenceCode,
				Status = TestCourseData.Status,
				
				Instructor = instructor
			};
		}
		
		return new Course() {
			Title = TestCourseData.Title,
			Description = TestCourseData.Description,
			Prerequisites = TestCourseData.Prerequisites,
			StartTime = System.DateTime.Now,
			EndTime = System.DateTime.Now.AddMinutes(Duration),
			// ReferenceCode = TestCourseData.ReferenceCode,
			Status = TestCourseData.Status,
			
			Instructor = CreateProfessionalMember()
		};
	}
	
	internal static int AddCourseToDatabase(ProfessionalMember? instructor = null) {
		using (ServiceDbContext context = DatabaseManager.GenerateAppDbContext()) {
			Course testCourse = CreateCourse(instructor);
			
			context.Courses.Add(testCourse);
			context.SaveChanges();
			
			return testCourse.Id;
		}
	}
}




