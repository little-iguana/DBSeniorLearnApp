using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.ServiceLookup;
using Microsoft.Extensions.Hosting;
using DBSeniorLearnApp;
using DBSeniorLearnApp.DataAccess;
using DBSeniorLearnApp.DataAccess.Models;
using DBSeniorLearnApp.Services;
using DBSeniorLearnApp.Services.Interfaces;
using DBSeniorLearnApp.UI.Data;

namespace DBSeniorLearnApp.UI;

public class Program
{
	public static string GlobalConnectionStringName { get; } = "HomeConnection";
	// public static string GlobalConnectionStringName { get; } = "TafeConnection";
	private static string connectionString = "";

	public async static Task Main(string[] args)
	{
		WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

	   // builder.Environment is of type Microsoft.AspNetCore.Hosting.HostingEnvironment
        if (builder.Environment.IsDevelopment() || builder.Environment.EnvironmentName == "Testing")
        {
           connectionString = builder.Configuration.GetConnectionString(GlobalConnectionStringName);
        }
        else
        {
           connectionString = builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING");
        }
		
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException($"Connection string '{GlobalConnectionStringName}' not found.");
        }
		
		// Add services to the container.
		builder.Services.AddDbContext<IdentificationDbContext>(options =>
			options.UseSqlServer(connectionString));
		
		// what does this do
		builder.Services.AddDatabaseDeveloperPageExceptionFilter();

		builder.Services.AddDbContext<ServiceDbContext>(options =>
			options.UseSqlServer(connectionString, b => b.MigrationsAssembly("DBSeniorLearnApp.UI"))
		);

		// register instantiated services with the DI container
		builder.Services.AddScoped<IMemberApplicationService, MemberApplicationService>();
		builder.Services.AddScoped<IScheduleCourseService, ScheduleCourseService>();
		builder.Services.AddScoped<IEnrolmentService, EnrolmentService>();

		builder.Services.AddDefaultIdentity<IdentityUser>(options =>
		{
			options.SignIn.RequireConfirmedAccount = false;
		}).AddRoles<IdentityRole>()
		  .AddEntityFrameworkStores<IdentificationDbContext>();
		
		// if non-ascii characters are required for username
		// https://stackoverflow.com/questions/68463201/using-non-ascii-characters-in-microsoft-aspnetcore-identity-identity
		
		builder.Services.Configure<IdentityOptions>(options =>
		{
			// options.Password.RequireDigit = true;
			// options.Password.RequireLowercase = true;
			options.Password.RequireNonAlphanumeric = false; // default == true
			// options.Password.RequireUppercase = true;
			// options.Password.RequiredLength = 6;
			options.Password.RequiredUniqueChars = 2; // default == 1
		});

		builder.Services.AddControllersWithViews();

		WebApplication app = builder.Build();

		// Seed user data
		using (var scope = app.Services.CreateScope())
		{
			// scope and services are of type ServiceProviderEngineScope
			// in the Microsoft.Extentions.DependencyInjection.ServiceLookup namespace
			var services = scope.ServiceProvider;
			await SeedRolesAndAdminAsync(services);
		}

		// Configure the HTTP request pipeline.
		if (app.Environment.IsDevelopment())
		{
			app.UseMigrationsEndPoint();
		}
		else
		{
			app.UseExceptionHandler("/Home/Error");
			// The default HSTS value is 30 days.
			// You may want to change this for production scenarios
			// See https://aka.ms/aspnetcore-hsts.
			app.UseHsts();
		}

		app.UseHttpsRedirection();
		app.UseStaticFiles();

		app.UseRouting();

		app.UseAuthorization();

		app.MapControllerRoute(
			name: "default",
			pattern: "{controller=Home}/{action=Index}/{id?}");
		app.MapRazorPages();

		app.Run();
	}
	
	
	/// <summary>
	/// Method for adding roles to the database if they don't already exist
	/// </summary>
	/// <param>IServiceProvider serviceProvider</param>
	private static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
    {
		UserManager<IdentityUser> userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
		RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

		// Define and seed all roles
		string[] roles = ["Admin", "Professional", "Standard"];
		foreach (string role in roles)
		{
			if (!await roleManager.RoleExistsAsync(role))
			{
				await roleManager.CreateAsync(new IdentityRole(role));
			}
		}
		
		var accounts = new [] {
			new {
				Role = "Admin",
				
				Firstname = "Admin",
				Lastname = "Admin",
				PhoneNumber = "0404000000",
				EmailAddress = "admin@seniorlearn.com",
				
				Password = "Password123!",
				IsProfessionalMember = true,
				
				PaidStatus = true,
			},
			new {
				Role = "Professional",
				
				Firstname = "Millie",
				Lastname = "Billie",
				PhoneNumber = "0404654321",
				EmailAddress = "millie.billie@hillie.com",
				
				Password = "Abcdef1!",
				IsProfessionalMember = true,
				
				PaidStatus = true,
			},
			new {
				Role = "Standard",
				
				Firstname = "Bob",
				Lastname = "Jobs",
				PhoneNumber = "0404123456",
				EmailAddress = "bob.jobs@hobbs.com",
				
				Password = "Bobjobshobbs1!",
				IsProfessionalMember = false,
				
				PaidStatus = true,
			}
		};
		
		// Seed accounts
		foreach (var acc in accounts)
		{
			IdentityUser existingUser = await userManager.FindByEmailAsync(
				acc.EmailAddress ?? throw new Exception("Email is null")
			);
			
			if (existingUser != null)
			{
				continue;
			}
			
			IdentityUser user = new IdentityUser(acc.EmailAddress!);
			user.Email = acc.EmailAddress!;
			
			IdentityResult result = await userManager.CreateAsync(user, acc.Password ?? "Password1!");
			if (!result.Succeeded)
			{
				System.Console.WriteLine("result: " + result);
				throw new Exception("Failed to create an account: " + acc.Role);
			}
			
			// member needs to be added to all roles "below" it
			await userManager.AddToRoleAsync(user, acc.Role ?? "Standard");
			await userManager.AddToRoleAsync(user, "Standard");
			
			// ik manually instantiating the services is bad but idk any other way
			ServiceDbContext context = new ServiceDbContext(
				new DbContextOptionsBuilder<ServiceDbContext>().UseSqlServer(connectionString).Options
			);
			
			MemberApplicationService memberService = new MemberApplicationService(context);
			
			Member? newMember = memberService.CreateMember(
				acc.Firstname ?? "",
				acc.Lastname ?? "",
				acc.PhoneNumber ?? "",
				acc.EmailAddress ?? "nulluser@null.com", // Member.EmailAddress doesn't have AK
				acc.Password ?? "Password1!",
				acc.PaidStatus,
				SaveToDb: true
			);
			
			if (acc.IsProfessionalMember)
			{
				ProfessionalMember professionalMember = new ProfessionalMember()
				{
					Description = "",
					StandardMember = newMember ?? throw new Exception("Member is null")
				};
				context.ProfessionalMembers.Add(professionalMember);
				context.SaveChanges();
			}
		}
    }
}








