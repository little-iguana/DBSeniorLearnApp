namespace DBSeniorLearnApp.UI.Models;

public class ViewCourseViewModel {
	
	public string Title { get; set; }
	public string InstructorName { get; set; }
	
	public System.DateTime StartTime { get; set; }
	public System.DateTime EndTime { get; set; }
	
	public string Description { get; set; }
	public string Prerequisites { get; set; }
	
	public string Status { get; set; }
}
