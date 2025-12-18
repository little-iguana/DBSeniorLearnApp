using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using DbModels = DBSeniorLearnApp.DataAccess.Models;

namespace DBSeniorLearnApp.UI.Models;

public class UpdateCourseViewModel {
	
	[Required]
	public int? Id { get; set; }
	
	[Required]
	[StringLength(50, ErrorMessage = "Title cannot be longer than 50 characters.")]
	public string? Title { get; set; }
	
	[Required]
	public System.DateTime? StartTime { get; set; }
	
	[Required]
	public System.DateTime? EndTime { get; set; }
	
	[StringLength(255, ErrorMessage = "Description cannot be longer than 255 characters.")]
	public string? Description { get; set; }
	
	[StringLength(255, ErrorMessage = "Prerequisites list cannot be longer than 255 characters.")]
	public string? Prerequisites { get; set; }
	
	[Required]
    public string? Status { get; set; }
    public SelectList? Statuses { get; set; } 
	
	public override string ToString() {
		return $"UpdateCourseViewModel: {Title}\n{Description}";
	}
}
