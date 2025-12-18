using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
// using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DBSeniorLearnApp.Services;
using DBSeniorLearnApp.Services.Interfaces;
using DBSeniorLearnApp.DataAccess;
using ViewModels = DBSeniorLearnApp.UI.Models;
using DbModels = DBSeniorLearnApp.DataAccess.Models;

namespace DBSeniorLearnApp.UI.Controllers;

[Authorize(Roles = "Professional, Admin")]
public class ScheduleController : Controller
{
    public readonly ILogger<ScheduleController> _logger;
    private ServiceDbContext _context;
    private IScheduleCourseService _schedulerService;
	private IMemberApplicationService _memberService;
	private readonly UserManager<IdentityUser> _userManager;
    public List<string> statuses = new List<string>() {
		"Draft",
		"Scheduled",
		"Closed",
		"Complete",
		"Cancelled"
	};
	
    public ScheduleController(
			ILogger<ScheduleController> logger,
			ServiceDbContext context,
			IScheduleCourseService courseService,
			IMemberApplicationService memberService,
			UserManager<IdentityUser> userManager
		)
    {
        _logger = logger;
        _context = context;
        _schedulerService = courseService;
        _memberService = memberService;
		_userManager = userManager;
    }
	
	
	[HttpGet]
	public async Task<IActionResult> Index() {
		
		IdentityUser? user = await _userManager.GetUserAsync(HttpContext.User);
		if (user == null || user.UserName == null)
		{
			throw new InvalidOperationException("user is null");
		}
		DbModels::Member? member = _memberService.RetrieveMemberByEmail(user.UserName);
		if (member == null)
		{
			throw new InvalidOperationException("member is null");
		}
		
		List<DbModels::Course>? courses = _schedulerService.RetrieveInstructorCourses(
			member.Id,
			DateTime.Now,
			DateTime.Now.AddMonths(1)
		);
		
		if (courses == null) {
			_logger.LogWarning("Sechedule/Index: courses returned null");
			var modelFail = new ViewModels::CourseDashboardViewModel(new List<DbModels::Course>());
			return View(modelFail);
		}
		
		var model = new ViewModels::CourseDashboardViewModel(courses);
		
		return View(model);
	}
	
	
	[HttpGet]
	public IActionResult New()
	{
		ViewModels::CreateCourseViewModel model = new ViewModels::CreateCourseViewModel();
		model.Statuses = new SelectList(statuses);
		
		model.CourseDate = DateOnly.FromDateTime(DateTime.Now);
		
		model.StartRecurring = DateOnly.FromDateTime(DateTime.Now);
		model.EndRecurring = DateOnly.FromDateTime(DateTime.Now);
		
		return View(model);
	}
	
	[HttpPost]
	public async Task<IActionResult> New(ViewModels::CreateCourseViewModel model)
	{
		if (!ModelState.IsValid)
		{
			_logger.LogWarning("ModelState returned invalid: " + ModelState);
			foreach (ModelError e in ModelState.Values.SelectMany(v => v.Errors))
			{
				_logger.LogWarning(e.ErrorMessage);
			}
			return View(model);
		}
		
		IdentityUser? user = await _userManager.GetUserAsync(HttpContext.User);
		if (user == null || user.UserName == null)
		{
			throw new InvalidOperationException("user is null");
		}
		DbModels::Member? member = _memberService.RetrieveMemberByEmail(user.UserName);
		if (member == null)
		{
			throw new InvalidOperationException("member is null");
		}
		_logger.LogWarning("member id: " + member.Id);
		
		if (!model.IsRecurring)
		{
			DbModels::Course? course = _schedulerService.CreateSingleCourse(
				member.Id,
				model.Title,
				model.Description,
				model.Prerequisites,
				new DateTime(model.CourseDate, model.StartTime),
				new DateTime(model.CourseDate, model.EndTime),
				model.Status
				
			);
			
			if (course == null)
			{
				_logger.LogWarning("single course is null");
				return View(model);
			}
		}
		else
		{
			// _logger.LogCritical("model.Freq: [" + model.Frequency + "], type: " + model.Frequency.GetType());
			if (model.Frequency.Value == CourseRecurrenceFrequency.None)
			{
				_logger.LogWarning("Frequency selectlist value is none");
				return View(model);
			}
			
			List<DbModels::Course> courses = await _schedulerService.CreateRecurringCourses(
				member.Id,
				model.Title,
				model.Description,
				model.Prerequisites,
				model.StartTime,
				model.EndTime,
				model.StartRecurring,
				model.EndRecurring,
				model.Frequency.Value,
				model.Status
			);
			
			if (courses == null)
			{
				_logger.LogWarning("attempt to create recurring courses returned null");
				return View(model);
			}
		}
		
		return RedirectToAction("Index");
	}
	
	
	[HttpGet]
	public IActionResult Update(int id)
	{
		ViewModels::UpdateCourseViewModel model = null!;//_context.Courses
			// .FirstOrDefault(c => c.Id == id)
			// .Select(a => new ViewModels::UpdateCourseViewModel()
			// {
				// Id = a.Id,
				// Title = a.Title,
				// StartTime = a.StartTime,
				// EndTime = a.EndTime,
				// Description = a.Description,
				// Prerequisites = a.Prerequisites,
				// Status = a.Status
			// });
		
		if (model == null) {
			throw new Exception("What the fuck");
		}
		model.Statuses = new SelectList(statuses);
		return View(model);
	}
	
	[HttpPost]
	public IActionResult Update(ViewModels::UpdateCourseViewModel model)
	{
		if (ModelState.IsValid && model.Id != null)
		{
			_schedulerService.UpdateCourse(
				courseId: model.Id ?? default(int),
				title: model.Title,
				description: model.Description,
				prerequisites: model.Prerequisites,
				startTime: model.StartTime,
				endTime: model.EndTime,
				status: model.Status
			);
			
			return RedirectToAction("Index");
		}
		
		return View(model);
	}
}







