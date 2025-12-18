using System;
using DBSeniorLearnApp.DataAccess.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DBSeniorLearnApp.UI.Models;

public class UpdateMemberAdminViewModel
{
    public Member Member { get; set; }
    public ProfessionalMember? ProMember { get; set; }
    public string MemberType { get; set; } = "Standard";
    public SelectList? MemberTypes { get; set; }
    public string MemberNumber { get; set; }


    /*
    public UpdateMemberAdminViewModel(Member member, ProfessionalMember? proMember)
    {
        Member = member;
        ProMember = proMember;
        MemberType = "Standard";
    }
    */
}


