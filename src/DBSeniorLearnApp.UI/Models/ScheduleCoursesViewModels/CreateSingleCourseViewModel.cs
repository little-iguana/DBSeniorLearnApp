using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DBSeniorLearnApp.UI.Models;

public class CreateSingleCourseViewModel()
{
    [Required]
    public string? Title { get; set; }
    [Required]
    public string? InstructorMemberNumber { get; set; }
    [Required]
    public DateTime StartTime { get; set; }
    [Required]
    public DateTime EndTime { get; set; }
    public string? Description { get; set; }
    public string? Prerequisites { get; set; }
    public string? Status { get; set; }
    public SelectList? Statuses { get; set; } 
}
