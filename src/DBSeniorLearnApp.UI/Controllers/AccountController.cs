using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using DBSeniorLearnApp.Services;
using DBSeniorLearnApp.Services.Interfaces;
using DBSeniorLearnApp.DataAccess;
using DBSeniorLearnApp.UI.Data;
using DbModels = DBSeniorLearnApp.DataAccess.Models;
using ViewModels = DBSeniorLearnApp.UI.Models;


namespace DBSeniorLearnApp.UI.Controllers;

public class AccountController : Controller
{
	private readonly ILogger<AccountController> _logger;
	
	private readonly ServiceDbContext _context;
	private readonly IdentificationDbContext _identityContext;
	
	private readonly IMemberApplicationService _memberApplicationService;
	
	private readonly UserManager<IdentityUser> _userManager;
	private readonly SignInManager<IdentityUser> _signInManager;
	
	public AccountController(
			ILogger<AccountController> logger,
			ServiceDbContext context,
			IdentificationDbContext identityContext,
			IMemberApplicationService memberAppService,
			UserManager<IdentityUser> userManager,
			SignInManager<IdentityUser> signInManager
		)
	{
		this._logger = logger;
		this._context = context;
		this._identityContext = identityContext;
		this._memberApplicationService = memberAppService;
		this._userManager = userManager;
		this._signInManager = signInManager;
	}


	public IActionResult Index()
	{
		return RedirectToAction("Index", "Home");
	}
	
	[HttpGet]
	public IActionResult Login()
	{
		_logger.LogWarning("Login GET called");
		return View();
	}
	
	[HttpPost]
	public async Task<IActionResult> Login(ViewModels::LoginMemberViewModel model)
	{
		_logger.LogWarning("Login POST called");
		_logger.LogWarning("model email: " + model.EmailAddress);
		
		IdentityUser? user = await _userManager.FindByNameAsync(model.EmailAddress);
		if (user == null) {
			_logger.LogWarning("User is null");
			return View(model);
		}
		
		var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
		
		if (!result.Succeeded) {
			_logger.LogWarning("result did not succeed: " + result);
			return View(model);
		}
		
		_logger.LogWarning("redirect success");
		return RedirectToAction("Index", "Home");
	}

	[HttpGet]
	public IActionResult Register()
	{
		return View();
	}

	[HttpPost]
	public IActionResult Register(ViewModels::CreateMemberViewModel model)
	{
		// _logger.LogWarning("Register Post called");
		
		//Need to check view model is valid
		if (!ModelState.IsValid)
		{
			return View(model);
		}
		else if (_context.Members.FirstOrDefault(e => e.EmailAddress == model.EmailAddress) != null)
		{
			return View(model);
		}
		else if (true)
		{}
		
		//Need to call member creation service.
		DbModels::Member? newMember = _memberApplicationService.CreateMember(
			model.Firstname,
			model.Lastname,
			model.PhoneNumber,
			model.EmailAddress,
			model.Password
		);
		
		if (newMember == null)
		{
			return View(model);
		}
		
		return RedirectToAction("RegisterPayment", newMember);
    }
	
	[HttpGet]
	public IActionResult RegisterPayment(DbModels::Member newMember)
	{
		return View();
	}
	
	[HttpPost]
	public async Task<IActionResult> RegisterPayment(
			ViewModels::RegisterPaymentViewModel model,
			DbModels::Member newMember
		)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}
		if (newMember.Password == null) {
			return View(model);
		}
		if (!PaymentService.ValidateCardNumber(model.CardNumber))
		{
			model.ErrorMessage = "Must provide a valid card number with 8-19 digits. Remove any spaces.";
			return View(model);
		}
		if (!PaymentService.ValidateCCV(model.CardCCV))
		{
			model.ErrorMessage = "Invalid CCV";
			return View(model);
		}
		if (!PaymentService.ValidateExpiry(model.CardExpiry))
		{
			model.ErrorMessage = "Expiry must be in the form MM/YY, and be in the future";
			return View(model);
		}
		
		if (!PaymentService.TakePayment(
				model.CardName,
				model.CardNumber,
				model.CardCCV,
				model.CardExpiry
			))
		{
			model.ErrorMessage = "Payment failed";
			_logger.LogError("Payment failed");
			return View(model);
		}
		_logger.LogInformation("Card payment taken successfully");
		
		newMember.PaidStatus = true;

		IdentityUser newUser = new IdentityUser(newMember.EmailAddress);
		// making a user can fail just because the password requires non-alphanumeric characters
		// password requirements have been configured in program.cs
		var result = await _userManager.CreateAsync(newUser, newMember.Password);
		
		if (result.Succeeded)
		{
			await _userManager.SetEmailAsync(newUser, newMember.EmailAddress);
			
			//Assign 'Standard' role to new user account, and save member to database.
			_memberApplicationService.SaveMemberToDatabase(newMember);
			await _userManager.AddToRoleAsync(newUser, "Standard");
			await _signInManager.SignInAsync(newUser, isPersistent: false);
		}
        else
        {
			_logger.LogInformation($"newuser: {newUser}");
			_logger.LogInformation($"result: {result}");
			throw new InvalidOperationException("Failed to create user, try again.");
        }
		
		return RedirectToAction("Index", "Home");
	}
	
	[HttpGet]
	public IActionResult RenewRegistration()
	{
		return View();
	}
	
	[HttpPost]
	public IActionResult RenewRegistration(ViewModels::RenewRegistrationViewModel model)
	{
		
		if (!ModelState.IsValid)
		{
			_logger.LogInformation("Model invalid");
			
			foreach (var modelState in ViewData.ModelState.Values)
			{
				foreach (var error in modelState.Errors)
				{
					System.Console.WriteLine(error.ErrorMessage);
				}
			}
			
			return View(model);
		}
		if (!PaymentService.ValidateCardNumber(model.CardNumber))
		{
			_logger.LogInformation("Card num invalid");
			model.ErrorMessage = "Must provide a valid card number with 8-19 digits. Remove any spaces.";
			return View(model);
		}
		if (!PaymentService.ValidateCCV(model.CardCCV))
		{
			_logger.LogInformation("CCV invalid");
			model.ErrorMessage = "Invalid CCV";
			return View(model);
		}
		if (!PaymentService.ValidateExpiry(model.CardExpiry))
		{
			_logger.LogInformation("Expiry invalid");
			model.ErrorMessage = "Expiry must be in the form MM/YY, and be in the future";
			return View(model);
		}
		
		if (!PaymentService.TakePayment(
				model.CardName,
				model.CardNumber,
				model.CardCCV,
				model.CardExpiry
			))
		{
			model.ErrorMessage = "Payment failed";
			_logger.LogError("Payment failed");
			return View(model);
		}
		
		_logger.LogInformation("Card payment taken successfully");
		return RedirectToAction("Index", "Home");
	}
	
	
	[HttpPost]
	public async Task<IActionResult> FileUpload(IFormFile file)
	{
		IdentityUser? user = await _userManager.GetUserAsync(HttpContext.User);
		if (user == null || file == null || file.Length == 0)
		{
			_logger.LogWarning("File upload failed");
			_logger.LogWarning("User is null: " + (user == null));
			ModelState.AddModelError("File", "Please add an image");
			return RedirectToAction("Index", "Home");
		}
		Directory.CreateDirectory("wwwroot/images/profilepictures/" + user.UserName);
		var filePath = "wwwroot/images/profilepictures/" + user.UserName + "/" + "picture.png";
		using (var stream = new FileStream(filePath, FileMode.Create))
		{
			await file.CopyToAsync(stream);
		}
		return RedirectToAction("Index", "Home");
	}
}






