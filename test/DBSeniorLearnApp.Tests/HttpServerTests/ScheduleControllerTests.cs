using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Moq;
using DBSeniorLearnApp.DataAccess.Models;

namespace DBSeniorLearnApp.UI.Controllers.Tests;

public class ScheduleControllerTests : BaseHttpTestingClass
{
	// https://gist.github.com/nthdeveloper/13ded7fdd9dabc80b973c49df081f987
	
	[Test]
	public async Task TestScheduleIndex_ShouldReturnCorrectResponseCodeAndCoursesAsync()
	{
		System.DateTime nowish = System.DateTime.Now;
		
		scheduleCourseServiceMock.scheduleService
			.Setup(x => x.RetrieveInstructorCourses(1, nowish, nowish.AddDays(7)))
			.Returns(new List<Course>());
		
		
		HttpClient client = GetClient();
		HttpResponseMessage response = await client.GetAsync("/Schedule"); // redirected to login
        string responseMessage = await response.Content.ReadAsStringAsync();
		
		Assert.True(response.IsSuccessStatusCode);
		
		// scheduleCourseServiceMock.scheduleService.Verify(x => x.RetrieveInstructorCourses(1, nowish, nowish.AddDays(7)), Times.Once);
		
		// System.Console.WriteLine(response);
		// System.Console.WriteLine(responseMessage);
		// Assert.That();
		
		
		// Setup
		/*
			Seed data to db OR create mock to return acc
			Post request to login
			Authenticate client using cookie
		*/
		
		// Excercise
		/*
			Request Schedule controller
		*/
		
		// Assertions
		/*
			Validate certain information is in the response
		*/
		
		// TearDown
		/*
			Reset db
			Dispose webserver and client
		*/
	}
}






