using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using DBSeniorLearnApp.DataAccess.Models;

namespace DBSeniorLearnApp.UI.Models;

public class ViewManyCoursesViewModel {
	
	public int MemberId { get; set; }
	public List<Course> Courses { get; set; }
}
