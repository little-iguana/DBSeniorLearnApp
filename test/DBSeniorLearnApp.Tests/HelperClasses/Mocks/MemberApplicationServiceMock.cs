using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Moq;
using DBSeniorLearnApp.Services.Interfaces;

namespace DBSeniorLearnApp.Tests.Mocks;

public class MemberApplicationServiceMock
{
	public Mock<IMemberApplicationService> memberService { get; }
	
	public MemberApplicationServiceMock()
	{
		memberService = new Mock<IMemberApplicationService>();
	}
	
    public IEnumerable<(Type, object)> GetMocks()
    {
        return GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(x =>
            {
                var underlyingType = x.PropertyType.GetGenericArguments()[0];
                var value = x.GetValue(this) as Mock;

                return (underlyingType, value.Object ?? throw new ArgumentNullException());
            })
            .ToArray();
    }
}
