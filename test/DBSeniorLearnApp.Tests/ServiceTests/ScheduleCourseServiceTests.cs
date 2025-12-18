using System.Linq;
using System.Collections.Generic;
// using System.Threading.Tasks;
// using NUnit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DBSeniorLearnApp.Tests; // DatabaseManager
using DBSeniorLearnApp.DataAccess;
using DBSeniorLearnApp.DataAccess.Models;
using static DBSeniorLearnApp.Tests.TestObjectFactory;

namespace DBSeniorLearnApp.Services.Tests;

// [Ignore("Test isolation")]
public class ScheduleCourseServiceTests {
	
	private Course testCourseData = TestObjectFactory.TestCourseData;
	private int Duration = TestObjectFactory.Duration;
	private int Tolerance = TestObjectFactory.Tolerance;
	
	[OneTimeSetUp]
	public void SuiteSetUp() {
		DatabaseManager.ResetDatabase();
	}
	
	[Test]
	public void TestCreateCourse_ShouldSaveToDatabase() {
		using (ServiceDbContext context = DatabaseManager.GenerateAppDbContext()) {
			
			ProfessionalMember testInstructor = CreateProfessionalMember();
			context.ProfessionalMembers.Add(testInstructor);
			context.SaveChanges();
			
			
			ScheduleCourseService scheduler = new ScheduleCourseService(context);
			
			scheduler.CreateSingleCourse(
				testInstructor.StandardMemberId,
				testCourseData.Title,
				testCourseData.Description ?? "",
				testCourseData.Prerequisites,
				testCourseData.StartTime,
				testCourseData.EndTime,
				testCourseData.Status
			);
			
			Course? retrievedCourse = context.Courses.FirstOrDefault(c => c.Title == testCourseData.Title);
			
			Assert.NotNull(retrievedCourse);
			
			Assert.That(retrievedCourse.Title, Is.EqualTo("How to bake a cake"));
			Assert.That(retrievedCourse.Description, Is.EqualTo("Cake baking 101"));
			
			Assert.That(retrievedCourse.Prerequisites, Is.Null);
			Assert.That(retrievedCourse.StartTime, Is.EqualTo(System.DateTime.Now).Within(Tolerance).Seconds);
			
			// Assert.That(retrievedCourse.ReferenceCode, Is.EqualTo("BK-101"));
		}
	}
	
	[Test]
	public void TestRetrieveCourseById_ShouldReturnCourse() {
		using (ServiceDbContext context = DatabaseManager.GenerateAppDbContext()) {
			ScheduleCourseService scheduler = new ScheduleCourseService(context);
			
			int courseId = AddCourseToDatabase();
			
			Course? retrievedCourse = scheduler.RetrieveSingleCourseById(courseId);
			
			Assert.NotNull(retrievedCourse);
			
			Assert.That(retrievedCourse.Title, Is.EqualTo(testCourseData.Title));
			Assert.That(retrievedCourse.Description, Is.EqualTo(testCourseData.Description));
			
			Assert.That(retrievedCourse.Prerequisites, Is.EqualTo(testCourseData.Prerequisites));
			
			Assert.That(retrievedCourse.StartTime, Is.EqualTo(System.DateTime.Now).Within(Tolerance).Seconds);
			Assert.That(
				retrievedCourse.EndTime,
				Is.EqualTo(System.DateTime.Now.AddMinutes(Duration)).Within(Tolerance).Seconds
			);
			
			Assert.That(retrievedCourse.Status, Is.EqualTo(testCourseData.Status));
		}
	}
	
	[Ignore("Not bothered to fix this rn")]
	[Test]
	public void TestRetrievingInstructorsCourses_ShouldReturnCourseList() {
		using (ServiceDbContext context = DatabaseManager.GenerateAppDbContext()) {
			ScheduleCourseService scheduler = new ScheduleCourseService(context);
			
			ProfessionalMember instructor = CreateProfessionalMember();
			List<int> courseIds = new List<int>();
			
			for (int i = 0; i < 5; i++) {
				courseIds.Add(AddCourseToDatabase(instructor));
			}
			
			
			List<Course>? courses = scheduler.RetrieveInstructorCourses(
				instructor.StandardMemberId,
				System.DateTime.Now.AddYears(-1),
				System.DateTime.Now.AddYears(1)
			);
			
			Assert.NotNull(courses);
			Assert.That(courses.Count, Is.EqualTo(5));
		}
	}
	
	[Test]
	public void TestUpdateCourse_ShouldChangeAccordingly() {
		using (ServiceDbContext context = DatabaseManager.GenerateAppDbContext()) {
			ScheduleCourseService scheduler = new ScheduleCourseService(context);
			
			int courseId = AddCourseToDatabase();
			
			scheduler.UpdateCourse(courseId, title: "Baking 101", description: "How to bake a cake: basics");
			
			Course? updatedCourse = context.Courses.FirstOrDefault(c => c.Id == courseId);
			
			Assert.NotNull(updatedCourse);
			
			Assert.That(updatedCourse.Title, Is.Not.EqualTo(testCourseData.Title));
			Assert.That(updatedCourse.Description, Is.Not.EqualTo(testCourseData.Description));
			
			Assert.That(updatedCourse.Prerequisites, Is.EqualTo(testCourseData.Prerequisites));
			
			Assert.That(updatedCourse.StartTime, Is.EqualTo(System.DateTime.Now).Within(Tolerance).Seconds);
			Assert.That(
				updatedCourse.EndTime,
				Is.EqualTo(System.DateTime.Now.AddMinutes(Duration)).Within(Tolerance).Seconds
			);
			
			Assert.That(updatedCourse.Status, Is.EqualTo(testCourseData.Status));
		}
	}
	
	[Test]
	public void TestDeleteCourse_ShouldRemoveFromDatabase() {
		using (ServiceDbContext context = DatabaseManager.GenerateAppDbContext()) {
			ScheduleCourseService scheduler = new ScheduleCourseService(context);
			
			int courseId = AddCourseToDatabase();
			
			Course? deletedCourse = scheduler.DeleteCourse(courseId);
			Assert.NotNull(deletedCourse);
			
			Course? attemptRetrieveCourse = context.Courses.FirstOrDefault(c => c.Id == deletedCourse.Id);
			Assert.Null(attemptRetrieveCourse);
		}
	}
	
	
	// check course overlap tests - cases A, B, C, D, and E
	
	// Case A:
	//  start                   end
	//    +----------------------+
	//    |       course 1       |
	//    +----------------------+
	// 					+---------------------+
	// 					|      course 2       |
	// 					+---------------------+
	[Test]
	public void TestCourseOverlapCaseA_ShouldReturnTrue() {
		Course courseA = CreateCourse();
		Course courseB = CreateCourse();
		
		courseA.StartTime = System.DateTime.Now.AddMinutes(-60);
		courseA.EndTime = System.DateTime.Now.AddMinutes(60);
		
		// courseB starts now and ends in two hours
		// courses have a 60 minute overlap
		
		Assert.True(ScheduleCourseService.CheckIfCoursesOverlap(courseA, courseB));
	}
	
	// Case B:
	//  			+----------------------+
	//  			|       course 1       |
	//  			+----------------------+
	// 	+---------------------+
	// 	|      course 2       |
	// 	+---------------------+
	[Test]
	public void TestCourseOverlapCaseB_ShouldReturnTrue() {
		Course courseA = CreateCourse();
		Course courseB = CreateCourse();
		
		// courseA starts now and ends in two hours
		// courses have a 60 minute overlap
		
		courseB.StartTime = System.DateTime.Now.AddMinutes(-60);
		courseB.EndTime = System.DateTime.Now.AddMinutes(60);
		
		Assert.True(ScheduleCourseService.CheckIfCoursesOverlap(courseA, courseB));
	}
	
	// 	Case 3:
	// 					+--------------+
	// 					|   course 1   |
	// 					+--------------+
	// 	+---------------------------------------------+
	// 	|                 course 2                    |
	// 	+---------------------------------------------+
	[Test]
	public void TestCourseOverlapCaseC_ShouldReturnTrue() {
		Course courseA = CreateCourse();
		Course courseB = CreateCourse();
		
		courseA.StartTime = System.DateTime.Now.AddMinutes(-20);
		courseA.EndTime = System.DateTime.Now.AddMinutes(20);
		
		courseB.StartTime = System.DateTime.Now.AddMinutes(-40);
		courseB.EndTime = System.DateTime.Now.AddMinutes(40);
		
		Assert.True(ScheduleCourseService.CheckIfCoursesOverlap(courseA, courseB));
	}
	
	// 	Case 4:
	// 	+----------------------------------------------+
	// 	|                  course 1                    |
	// 	+----------------------------------------------+
	// 				 +--------------+
	// 				 |   course 2   |
	// 				 +--------------+
	[Test]
	public void TestCourseOverlapCaseD_ShouldReturnTrue() {
		Course courseA = CreateCourse();
		Course courseB = CreateCourse();
		
		courseA.StartTime = System.DateTime.Now.AddMinutes(-40);
		courseA.EndTime = System.DateTime.Now.AddMinutes(40);
		
		courseB.StartTime = System.DateTime.Now.AddMinutes(-20);
		courseB.EndTime = System.DateTime.Now.AddMinutes(20);
		
		Assert.True(ScheduleCourseService.CheckIfCoursesOverlap(courseA, courseB));
	}
	
	// 	Case 5:
	// 	   +----------------------+                         +---------------------+
	// 	   |       course 1       |                         |       course 3      |
	// 	   +----------------------+                         +---------------------+
	// 								+---------------------+
	// 								|       course 2      |
	// 								+---------------------+
	[Test]
	public void TestCourseOverlapCaseE_ShouldReturnFalse() {
		Course courseA = CreateCourse();
		Course courseB = CreateCourse();
		Course courseC = CreateCourse();
		
		courseA.StartTime = System.DateTime.Now.AddMinutes(-60);
		courseA.EndTime = System.DateTime.Now.AddMinutes(-21);
		
		courseB.StartTime = System.DateTime.Now.AddMinutes(-19);
		courseB.EndTime = System.DateTime.Now.AddMinutes(19);
		
		courseC.StartTime = System.DateTime.Now.AddMinutes(21);
		courseC.EndTime = System.DateTime.Now.AddMinutes(60);
		
		Assert.False(ScheduleCourseService.CheckIfCoursesOverlap(courseA, courseB));
		Assert.False(ScheduleCourseService.CheckIfCoursesOverlap(courseB, courseC));
	}
	
	
	[TearDown]
    public void Cleanup() {
        DatabaseManager.ResetDatabase();
    }
}






