using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using DBSeniorLearnApp.Services;

namespace DBSeniorLearnApp.UI.Models;

public class CreateCourseViewModel
{
    [Required]
    public string? Title { get; set; }
	
	[Required]
	public DateOnly CourseDate { get; set; }
    [Required]
    public TimeOnly StartTime { get; set; }
    [Required]
    public TimeOnly EndTime { get; set; }
	
    public string? Description { get; set; }
    public string? Prerequisites { get; set; }
	
    public string? Status { get; set; }
    public SelectList? Statuses { get; set; }
	
	public CourseRecurrenceFrequency? Frequency { get; set; } = CourseRecurrenceFrequency.None;
	
	public bool IsRecurring { get; set; } = false;
    public DateOnly? StartRecurring { get; set; }
    public DateOnly? EndRecurring { get; set; }
}
