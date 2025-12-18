using System.Collections.Generic;
using System.Threading.Tasks;
using DBSeniorLearnApp.DataAccess.Models;

namespace DBSeniorLearnApp.Services.Interfaces;

public interface IScheduleCourseService
{
    public Course? CreateSingleCourse(
			int instructorId,
			string title,
			string desc,
			string? prereqs,
			System.DateTime start,
			System.DateTime end,
			string status,
			System.Guid recurrenceId = new System.Guid()/*,
			CourseType type*/
		);
	
	public Task<List<Course>?> CreateRecurringCourses(
			int instructorId,
			string title,
			string desc,
			string? prereqs,
			System.TimeOnly start,
			System.TimeOnly end,
			System.DateOnly? startRecurrence,
			System.DateOnly? endRecurrence,
			CourseRecurrenceFrequency frequency = CourseRecurrenceFrequency.Weekly,
			string status = "Draft",
			System.Guid recurrenceId = new System.Guid()/*,
			CourseType type*/
		);
	
	
	public List<Course>? RetrieveInstructorCourses(
			int instructorId,
			System.DateTime start,
			System.DateTime end
		);
	
	public List<Course>? RetrieveCoursesWithinTimeframe(System.DateTime start, System.DateTime end);
	
	public List<Course>? RetrieveAllRecurringCourses(System.Guid recurrenceId);
	
	public List<Course>? RetrieveRecurringCoursesWithinTimeframe(
			System.Guid recurrenceId,
			System.DateTime start,
			System.DateTime end
		);
	
	
	public Course? UpdateCourse(
			int courseId,
			int? instructorId = null,
			string? title = null,
			string? description = null,
			System.DateTime? startTime = null,
			System.DateTime? endTime = null,
			string? prerequisites = null,
			string? referenceCode = null,
			string? status = "Draft"
		);
	
	public List<Course>? UpdateRecurringCourses(
			System.Guid recurrenceId,
			int? instructorId = null,
			string? title = null,
			string? description = null,
			System.DateTime? startTime = null,
			System.DateTime? endTime = null,
			string? prerequisites = null,
			string? referenceCode = null,
			string? status = "Draft"
		);
	
	
	public Course? DeleteCourse(int courseId);
	
	public List<Course>? DeleteRecurringCourses(System.Guid recurrenceId);
}







