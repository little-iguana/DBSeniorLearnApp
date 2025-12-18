using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using DBSeniorLearnApp.DataAccess;
using DBSeniorLearnApp.DataAccess.Models;

namespace DBSeniorLearnApp.Services;

public class ScheduleCourseService : Interfaces.IScheduleCourseService
{
    private ServiceDbContext _context;
	private ILogger<ScheduleCourseService> _logger;
	
    public ScheduleCourseService(ServiceDbContext context, ILogger<ScheduleCourseService> logger)
    {
        _context = context;
		_logger = logger;
    }
	
	public ScheduleCourseService(ServiceDbContext context)
	{
		_context = context;
	}
	
	private Course? CreateSingleCourseNoChecks(
			int instructorId,
			string title,
			string desc,
			string? prereqs,
			DateTime start,
			DateTime end,
			string status,
			Guid recurrenceId,
			ProfessionalMember pMember/*,
			CourseType type*/
		)
	{
        Course newCourse = new Course()
		{
			InstructorId = instructorId,
			RecurrenceGuid = recurrenceId,
			Title = title,
			Description = desc,
			Prerequisites = prereqs,
			StartTime = start,
			EndTime = end,
			Status = status,
			Instructor = pMember
			// ReferenceCode = ReferenceCodeGenerator.New(type)
		};
        
		//
		
        _context.Courses.Add(newCourse);
        _context.SaveChanges();

        return newCourse;
	}

    public Course? CreateSingleCourse(
			int instructorId,
			string title,
			string desc,
			string? prereqs,
			DateTime start,
			DateTime end,
			string status,
			Guid recurrenceId = new Guid()/*,
			CourseType type*/
		)
    {
		ProfessionalMember? pMember = _context.ProfessionalMembers.FirstOrDefault(m => m.StandardMemberId == instructorId);
		if (pMember == null)
        {
			_logger.LogWarning("professional member returned null");
			return null;
        }
		
		if (start.Year != end.Year)
		{
			_logger.LogWarning("class goes over the new year");
			return null;
		}
		
		int overlaps = _context.Courses.Count(c => c.StartTime < end && c.EndTime > start);
		if (overlaps > 0)
		{
			_logger.LogWarning("class overlaps with another");
			return null;
		}
		
        Course newCourse = new Course()
		{
			InstructorId = instructorId,
			RecurrenceGuid = recurrenceId,
			Title = title,
			Description = desc,
			Prerequisites = prereqs,
			StartTime = start,
			EndTime = end,
			Status = status,
			Instructor = pMember
			// ReferenceCode = ReferenceCodeGenerator.New(type)
		};
        
        _context.Courses.Add(newCourse);
        _context.SaveChanges();

        return newCourse;
    }
	
	public async Task<List<Course>?> CreateRecurringCourses(
			int instructorId,
			string title,
			string desc,
			string? prereqs,
			TimeOnly startTime,
			TimeOnly endTime,
			DateOnly? startRecurrence,
			DateOnly? endRecurrence,
			CourseRecurrenceFrequency frequency = CourseRecurrenceFrequency.Weekly,
			string status = "Draft",
			Guid recurrenceId = new Guid()/*,
			CourseType type*/
		)
	{
		if (startRecurrence == null)
		{
			startRecurrence = DateOnly.FromDateTime(DateTime.Now);
		}
		if (endRecurrence == null)
		{
			endRecurrence = new DateOnly(startRecurrence.Value.Year, 12, 31);
		}
		if (endRecurrence < startRecurrence)
		{
			return null;
		}
		
		ProfessionalMember? pMember = _context.ProfessionalMembers.FirstOrDefault(m => m.StandardMemberId == instructorId);
		if (pMember == null)
        {
			_logger.LogWarning("professional member returned null");
			return null;
        }
		Member? member = _context.Members.FirstOrDefault(m => m.Id == instructorId);
		if (member == null)
		{
			_logger.LogWarning("member returned null");
			return null;
		}
		
		if (startRecurrence.Value.Year != endRecurrence.Value.Year)
		{
			_logger.LogWarning("course goes over the new year");
			return null;
		}
		
		
		DayOfWeek? weeklyDay = null;
		if (frequency == CourseRecurrenceFrequency.Weekly)
		{
			weeklyDay = startRecurrence.Value.DayOfWeek;
		}
		List<Course> courses = new List<Course>();
		
		await using var transaction = await _context.Database.BeginTransactionAsync();
		try
		{
			for (DateOnly current = startRecurrence.Value; current < endRecurrence.Value; current = current.AddDays(1))
			{
				if (ThisDayShouldBeIncluded(current, frequency, weeklyDay))
				{
					courses.Add(CreateSingleCourseNoChecks(
						instructorId,
						title,
						desc,
						prereqs,
						new DateTime(current, startTime),
						new DateTime(current, endTime),
						status,
						recurrenceId,
						pMember
					));
				}
			}
			transaction.Commit();
		}
		catch (Exception exception)
		{
			_logger.LogError(exception, "");
			return null;
		}
		return courses;
	}
	
	private bool ThisDayShouldBeIncluded(
			DateOnly date,
			CourseRecurrenceFrequency frequency,
			DayOfWeek? weeklyDay = null
		)
	{
		// | daily     |  M T W T F S S  |
		// | weekly    |      W          |
		// | weekdays  |  M T W T F      |
		// | weekends  |            S S  |
		if (frequency == CourseRecurrenceFrequency.Daily)
		{
			return true;
		}
		if (frequency == CourseRecurrenceFrequency.Weekly)
		{
			if (weeklyDay == null)
			{
				throw new InvalidOperationException("Day of the week must be specified if the frequency is weekly");
			}
			if (date.DayOfWeek == weeklyDay)
			{
				return true;
			}
		}
		if (frequency == CourseRecurrenceFrequency.Weekdays)
		{
			if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
			{
				return true;
			}
		}
		if (frequency == CourseRecurrenceFrequency.Weekends)
		{
			if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
			{
				return true;
			}
		}
		return false;
	}
	
	
	[Obsolete("Service methods should be used for everything except R in CRUD")]
	public Course? RetrieveSingleCourseById(int courseId)
	{
		return _context.Courses.FirstOrDefault(c => c.Id == courseId);
	}
	
	[Obsolete("Instructors no longer have a member number - reference the instructor using the object's Id.")]
	public List<Course>? RetrieveInstructorCourses(
		string instructorMemberNumber,
		DateTime start,
		DateTime end
		)
	{
		Member? member = _context.Members.FirstOrDefault(m => m.MemberNumber == instructorMemberNumber);
		
		if (member == null) {
			return null;
		}
		
		List<Course> courses = _context.Courses
									.Where(c => c.InstructorId == member.Id)
									.Where(c => c.StartTime > start && c.StartTime < end)
									.OrderBy(v => v.StartTime)
									.ToList();
		
		if (courses == null || courses.Count < 1) {
			return null;
		}
		
		return courses;
	}
	
	public List<Course>? RetrieveInstructorCourses(
		int instructorId,
		DateTime start,
		DateTime end
		)
	{
		Member? member = _context.Members.FirstOrDefault(m => m.Id == instructorId);
		
		if (member == null) {
			return null;
		}
		
		List<Course> courses = _context.Courses
									.Where(c => c.InstructorId == member.Id)
									.Where(c => c.StartTime > start && c.StartTime < end)
									.OrderBy(v => v.StartTime)
									.ToList();
		
		return courses;
	}
	
	public List<Course>? RetrieveCoursesWithinTimeframe(DateTime start, DateTime end)
	{
		List<Course> courses = _context.Courses
									.Where(c => c.StartTime > start && c.StartTime < end)
									.OrderBy(v => v.StartTime)
									.ToList();
		return courses;
	}
	
	public List<Course>? RetrieveAllRecurringCourses(Guid recurrenceId)
	{
		// all crud operations involving recurrence are limited to within the current year
		List<Course> courses = _context.Courses
									.Where(c => c.RecurrenceGuid == recurrenceId)
									.Where(c => c.StartTime < new DateTime(DateTime.Now.Year, 12, 31))
									.OrderBy(v => v.StartTime)
									.ToList();
		return courses;
	}
	
	public List<Course>? RetrieveRecurringCoursesWithinTimeframe(
			Guid recurrenceId,
			DateTime start,
			DateTime end
		)
	{
		List<Course> courses = _context.Courses
									.Where(c => c.RecurrenceGuid == recurrenceId)
									.Where(c => c.StartTime > start && c.StartTime < end)
									.OrderBy(v => v.StartTime)
									.ToList();
		return courses;
	}
	
	
	public Course? UpdateCourse(
			int courseId,
			int? instructorId = null,
			string? title = null,
			string? description = null,
			DateTime? startTime = null,
			DateTime? endTime = null,
			string? prerequisites = null,
			string? referenceCode = null,
			string? status = "Draft"
		)
	{
		Course? retrievedCourse = _context.Courses.FirstOrDefault(c => c.Id == courseId);
		if (retrievedCourse == null)
		{
			return null;
		}

		retrievedCourse.InstructorId = instructorId ?? retrievedCourse.InstructorId;
		retrievedCourse.Title = title ?? retrievedCourse.Title;
		retrievedCourse.Description = description ?? retrievedCourse.Description;
		retrievedCourse.StartTime = startTime ?? retrievedCourse.StartTime;
		retrievedCourse.EndTime = endTime ?? retrievedCourse.EndTime;
		retrievedCourse.Prerequisites = prerequisites ?? retrievedCourse.Prerequisites;
		// retrievedCourse.ReferenceCode = referenceCode ?? retrievedCourse.ReferenceCode;
		retrievedCourse.Status = status ?? retrievedCourse.Status;
		
		_context.SaveChanges();

		return retrievedCourse;
	}
	
	public List<Course>? UpdateRecurringCourses(
			Guid recurrenceId,
			int? instructorId = null,
			string? title = null,
			string? description = null,
			DateTime? startTime = null,
			DateTime? endTime = null,
			string? prerequisites = null,
			string? referenceCode = null,
			string? status = "Draft"
		)
	{
		List<Course> courses = _context.Courses
									.Where(c => c.RecurrenceGuid == recurrenceId)
									.Where(c => c.StartTime < new DateTime(DateTime.Now.Year, 12, 31))
									.OrderBy(v => v.StartTime)
									.ToList();
		
		foreach (Course Cse in courses)
		{
			Cse.InstructorId = instructorId ?? Cse.InstructorId;
			Cse.Title = title ?? Cse.Title;
			Cse.Description = description ?? Cse.Description;
			Cse.StartTime = startTime ?? Cse.StartTime;
			Cse.EndTime = endTime ?? Cse.EndTime;
			Cse.Prerequisites = prerequisites ?? Cse.Prerequisites;
			// Cse.ReferenceCode = referenceCode ?? Cse.ReferenceCode;
			Cse.Status = status ?? Cse.Status;
		}
		
		_context.SaveChanges();
		
		return courses;
	}
	
	public Course? DeleteCourse(int courseId)
	{
		Course? courseToDelete = _context.Courses.FirstOrDefault(c => c.Id == courseId);
		if (courseToDelete == null) {
			return null;
		}
		
		_context.Courses.Remove(courseToDelete);
		_context.SaveChanges();
		
		return courseToDelete;
	}
	
	
	public List<Course>? DeleteRecurringCourses(Guid recurrenceId)
	{
		IEnumerable<Course> courses = _context.Courses.Where(c => c.RecurrenceGuid == recurrenceId);
		
		_context.Courses.RemoveRange(courses);
		_context.SaveChanges();
		
		return courses.ToList();
	}
	
	
	// returns true if course1 times overlap with course2 times
	// [Obsolete("Genuinely nothing uses this method - only the tests which cover this method use it.")]
	public static bool CheckIfCoursesOverlap(Course course1, Course course2)
	{
		return course1.StartTime < course2.EndTime && course2.StartTime < course1.EndTime;
	}
}







