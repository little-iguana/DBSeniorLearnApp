using System;
using System.Collections.Generic;

namespace DBSeniorLearnApp.DataAccess.Models;

public class Course
{
    public Course()
    {
        Instructor = new ProfessionalMember();
		Title = "UNDEFINED";
        StartTime = DateTime.Now;
        Status = "Draft";
    }
	
    public int Id { get; set; }
    public int InstructorId { get; set; }
	public Guid RecurrenceGuid { get; set; }
	
    public string Title { get; set; }
    public string? Description { get; set; }
	
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
	
    public string? Prerequisites { get; set; }
    // public string ReferenceCode { get; set; }
	
	public string Status { get; set; }
	
	
    //Navigation properties
    public ProfessionalMember Instructor { get; set; }
	public ICollection<CourseEnrolment>? CourseEnrolments { get; set; }
	
	
	public override string ToString() {
		return $"Course {Id}: {Title} @ {StartTime}";
	}
}
