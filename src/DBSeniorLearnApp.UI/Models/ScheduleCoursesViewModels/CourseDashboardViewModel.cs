using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using DBSeniorLearnApp.DataAccess.Models;

namespace DBSeniorLearnApp.UI.Models;

public class CourseDashboardViewModel {
	// public ProfessionalMember professionalMember { get; set; }
	public List<Course> Courses { get; set; }
	
	public CourseDashboardViewModel(List<Course> courses) {
		this.Courses = courses;
	}
}
