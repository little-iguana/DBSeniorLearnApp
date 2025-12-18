namespace DBSeniorLearnApp.DataAccess.Models;

public class CourseEnrolment
{
    public CourseEnrolment()
    {
        Member = new Member();
		Course = new Course();
    }
	
    public int Id { get; set; }
    public int MemberId { get; set; }
	public int CourseId { get; set; }
	
    //Navigation properties
    public Member Member { get; set; }
	public Course Course { get; set; }
	
	
	public override string ToString() {
		return $"Enrolment {Id}: {MemberId} {CourseId}";
	}
}
