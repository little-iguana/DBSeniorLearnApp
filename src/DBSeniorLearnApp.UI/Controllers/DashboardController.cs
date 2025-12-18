using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DBSeniorLearnApp.Services.Interfaces;
using DBSeniorLearnApp.DataAccess;
using DBSeniorLearnApp.DataAccess.Models;
using DBSeniorLearnApp.UI.Models;
using DBSeniorLearnApp.UI.Data;

namespace DBSeniorLearnApp.UI.Controllers;

public class DashboardController : Controller
{
    public List<String> memberTypes = ["Standard", "Professional (Active)", "Professional (Deactive)"];

    private ServiceDbContext _context;
    private IMemberApplicationService _memberService;
    private UserManager<IdentityUser> _userManager;
	
	
    public DashboardController(
            ServiceDbContext serviceContext,
            IMemberApplicationService memberService,
            UserManager<IdentityUser> userManager
        )
    {
        _context = serviceContext;
        _memberService = memberService;
        _userManager = userManager;
    }
	
	
    public async Task<IActionResult> Admin()
    {
        MemberListViewModel membersList = new MemberListViewModel(_context.Members.ToList());
        for(int i = 0; i < membersList.Members.Count; i++)
        {
            var user = await _userManager.FindByNameAsync(membersList.Members[i].EmailAddress);
            if (user == null)
            {
                throw new Exception("User not found in users table.");
            }
            ProfessionalMember? proMember = _context.ProfessionalMembers.FirstOrDefault(pm => pm.StandardMemberId == membersList.Members[i].Id);
            IList<String> role = await _userManager.GetRolesAsync(user);
            membersList.MemberTypes.Add(role[0]);
            if (proMember != null)
            {
                membersList.Members[i].ProfessionalMember = proMember;
            }
        }

        return View(membersList);
    }

    [HttpGet]
    public IActionResult UserView(string selectedMemberNumber)
    {
        Member? selectedMember = _memberService.RetrieveMemberByMemberNumber(selectedMemberNumber);
        if (selectedMember == null)
        {
            return View("Admin");
        }
        ProfessionalMember? proMember = _context.ProfessionalMembers.FirstOrDefault(pm => pm.StandardMemberId == selectedMember.Id);
        if (proMember != null)
        {
            selectedMember.ProfessionalMember = proMember;
        }
        //UpdateMemberAdminViewModel model = new UpdateMemberAdminViewModel(selectedMember, proMember);
        UpdateMemberAdminViewModel model = new UpdateMemberAdminViewModel
        {
            Member = selectedMember,
            ProMember = selectedMember.ProfessionalMember,
            MemberNumber = selectedMember.MemberNumber
        };
		
        if (proMember != null && proMember.Deactivated == false)
        {
            model.MemberType = "Professional (Active)";
        }
        else if (proMember != null)
        {
            model.MemberType = "Professional (Deactive)";
        }
        model.MemberTypes = new SelectList(memberTypes);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> UserView(UpdateMemberAdminViewModel model)
    {
        if (model.MemberType == "Professional (Active)")
        {
            
            if (model.ProMember == null)
            {
                Member? member = _context.Members.FirstOrDefault(m => m.MemberNumber == model.MemberNumber);
                if (member == null)
                {
                    throw new InvalidOperationException("User does not exist in users table.");
                }
                ProfessionalMember proMember = new ProfessionalMember
                {
                    StandardMemberId = member.Id,
                    Description = "Default description",
                    Deactivated = false,
                    IsHonoraryMember = false
                };
                IdentityUser? updatedUser = await _userManager.FindByNameAsync(member.EmailAddress);
                if (updatedUser == null)
                {
                    throw new InvalidOperationException("User does not exist in users table.");
                }
                await _userManager.AddToRoleAsync(updatedUser, "Professional");
                await _userManager.RemoveFromRoleAsync(updatedUser, "Standard");
                _context.ProfessionalMembers.Add(proMember);
                _context.SaveChanges();



            }
        }
        return RedirectToAction("Admin");
    }
	
	[HttpGet]
	public IActionResult Statistics()
	{
		int numOfCourses = _context.Courses.Count();
		
		AdminStatisticsViewModel model = new AdminStatisticsViewModel();
		model.NumberOfCourses = numOfCourses;
		
		return View(model);
	}
}





