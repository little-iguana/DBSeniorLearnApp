using System;
using System.Collections.Generic;
using DBSeniorLearnApp.DataAccess.Models;

namespace DBSeniorLearnApp.UI.Models;

public class MemberListViewModel
{
    public List<Member> Members;

    public List<String> MemberTypes;
    public MemberListViewModel(List<Member> memList)
    {
        Members = memList;
        MemberTypes = new List<String>();
    }
}