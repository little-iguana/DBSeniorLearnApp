using DBSeniorLearnApp.DataAccess.Models;

namespace DBSeniorLearnApp.Services.Interfaces;

public interface IEnrolmentService
{
	public CourseEnrolment? EnrolMember(int memberId, int courseId);
	
	public CourseEnrolment? UnenrolMember(int enrolmentId);
	
	public CourseEnrolment? UnenrolMember(int memberId, int courseId);
}
