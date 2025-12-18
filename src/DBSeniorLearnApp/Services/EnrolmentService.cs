using System.Linq;
using DBSeniorLearnApp.DataAccess;
using DBSeniorLearnApp.DataAccess.Models;

namespace DBSeniorLearnApp.Services;

public class EnrolmentService : Interfaces.IEnrolmentService
{
    private ServiceDbContext _context;
	
	public EnrolmentService(ServiceDbContext context)
	{
        _context = context;
	}
	
	public CourseEnrolment? EnrolMember(int memberId, int courseId)
	{
		Member? member = _context.Members.FirstOrDefault(m => m.Id == memberId);
		Course? course = _context.Courses.FirstOrDefault(c => c.Id == courseId);
		
		if (member == null || course == null)
		{
			return null;
		}
		
		CourseEnrolment enrolment = new CourseEnrolment()
		{
			Member = member,
			Course = course
		};
		
		_context.CourseEnrolments.Add(enrolment);
		_context.SaveChanges();
		
		return enrolment;
	}
	
	public CourseEnrolment? UnenrolMember(int enrolmentId)
	{
		CourseEnrolment? enrolment = _context.CourseEnrolments.FirstOrDefault(e => e.Id == enrolmentId);
		if (enrolment == null)
		{
			return null;
		}
		
		_context.CourseEnrolments.Remove(enrolment);
		_context.SaveChanges();
		
		return enrolment;
	}
	
	public CourseEnrolment? UnenrolMember(int memberId, int courseId)
	{
		CourseEnrolment? enrolment = _context.CourseEnrolments
			.FirstOrDefault(e => e.MemberId == memberId && e.CourseId == courseId);
		if (enrolment == null)
		{
			return null;
		}
		
		_context.CourseEnrolments.Remove(enrolment);
		_context.SaveChanges();
		
		return enrolment;
	}
}






