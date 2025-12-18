using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DBSeniorLearnApp.Services.Interfaces;
using DBSeniorLearnApp.DataAccess;
using ViewModels = DBSeniorLearnApp.UI.Models;
using DbModels = DBSeniorLearnApp.DataAccess.Models;

namespace DBSeniorLearnApp.UI.Controllers;

[Authorize]
public class BrowseController : Controller
{
    public readonly ILogger<BrowseController> _logger;
    private ServiceDbContext _context;
    private IScheduleCourseService _schedulerService;
	private IMemberApplicationService _memberService;
	private IEnrolmentService _enrolmentService;
	private readonly UserManager<IdentityUser> _userManager;
	
    public BrowseController(
			ILogger<BrowseController> logger,
			ServiceDbContext context,
			IScheduleCourseService courseService,
			IMemberApplicationService memberService,
			IEnrolmentService enrolmentService,
			UserManager<IdentityUser> userManager
		)
    {
        _logger = logger;
        _context = context;
        _schedulerService = courseService;
        _memberService = memberService;
		_enrolmentService = enrolmentService;
		_userManager = userManager;
    }
	
	
	[HttpGet]
	public async Task<IActionResult> Index()
	{
		IdentityUser? user = await _userManager.GetUserAsync(HttpContext.User);
		if (user == null || user.UserName == null)
		{
			throw new System.InvalidOperationException("user is null");
		}
		DbModels::Member? member = _memberService.RetrieveMemberByEmail(user.UserName);
		if (member == null)
		{
			throw new System.InvalidOperationException("member is null");
		}
		List<DbModels::Course> courses = _context.Courses
			.Where(c => c.StartTime > System.DateTime.Now && c.StartTime < System.DateTime.Now.AddMonths(1))
			.Include(e => e.CourseEnrolments)
			.OrderBy(v => v.StartTime)
			.ToList();
		
		if (courses == null)
		{
			_logger.LogError("no courses");
			return View(new ViewModels::ViewManyCoursesViewModel() { Courses = new List<DbModels::Course>() });
		}
		
		ViewModels::ViewManyCoursesViewModel model = new ViewModels::ViewManyCoursesViewModel()
		{
			MemberId = member.Id,
			Courses = courses
		};
		
		return View(model);
	}
	
	[HttpGet] // enroll is american, enrol is british
	public IActionResult Enrol(int id) {
		// strictly using id as a query parameter could lead to an IDOR attack,
		// but that is a problem for another time
		DbModels::Course? course = _context.Courses.FirstOrDefault(c => c.Id == id);
		if (course == null) {
			// the only time this should be triggered is if the query parameter is modified by the user
			// (IDOR attack) - otherwise the only IDs are the ones listed in the Index page
			throw new System.Exception("What the fuck");
		}
		
		DbModels::Member? instructor = _context.Members.FirstOrDefault(m => m.Id == course.InstructorId);
		if (instructor == null) {
			// Courses should not be able to have invalid InstructorIds.
			throw new System.Exception("What the fuck");
		}
		string instructorName = instructor.Firstname + " " + instructor.Lastname;
		
		var model = new ViewModels::ViewCourseViewModel() {
			Title = course.Title,
			InstructorName = instructorName,
			
			StartTime = course.StartTime,
			EndTime = course.EndTime,
			
			Description = course.Description ?? "",
			Prerequisites = course.Prerequisites ?? "",
			
			Status = course.Status
		};
		return View(model);
	}
	
	[HttpPost]
	public async Task<IActionResult> Enrol(int id, ViewModels::ViewCourseViewModel model)
	{
		IdentityUser? user = await _userManager.GetUserAsync(HttpContext.User);
		if (user == null || user.UserName == null)
		{
			throw new System.InvalidOperationException("user is null");
		}
		DbModels::Member? member = _memberService.RetrieveMemberByEmail(user.UserName);
		if (member == null)
		{
			throw new System.InvalidOperationException("member is null");
		}
		
		
		DbModels::CourseEnrolment? potentialEnrolment = _context.CourseEnrolments
			.FirstOrDefault(m => m.MemberId == member.Id && m.CourseId == id);
		
		if (potentialEnrolment == null)
		{
			DbModels::CourseEnrolment? enrolment = _enrolmentService.EnrolMember(member.Id, /*course*/id);
			
			if (enrolment == null)
			{
				// idk suffer or sumthing
			}
		}
		else
		{
			DbModels::CourseEnrolment? enrolment = _enrolmentService.UnenrolMember(member.Id, /*course*/id);
			
			if (enrolment == null)
			{
				// idk suffer or sumthing
			}
		}
		
		return RedirectToAction("Index");
	}
}







