using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using DBSeniorLearnApp.Tests.Mocks;

namespace DBSeniorLearnApp.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<DBSeniorLearnApp.UI.Program>
{
    private MemberApplicationServiceMock _memberApplicationServiceMock;
    private ScheduleCourseServiceMock _scheduleCourseServiceMock;
    private EnrolmentServiceMock _enrolmentServiceMock;

    public CustomWebApplicationFactory(
			MemberApplicationServiceMock memberApplicationServiceMock,
			ScheduleCourseServiceMock scheduleCourseServiceMock,
			EnrolmentServiceMock enrolmentServiceMock
		)
    {
		_memberApplicationServiceMock = memberApplicationServiceMock;
		_scheduleCourseServiceMock = scheduleCourseServiceMock;
		_enrolmentServiceMock = enrolmentServiceMock;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        base.ConfigureWebHost(builder);
		
		
		// idk any other way to do this
        builder.ConfigureServices(services =>
		{
			foreach ((var interfaceType, var serviceMock) in _memberApplicationServiceMock.GetMocks())
			{
				services.Remove(
					services.SingleOrDefault(d => d.ServiceType == interfaceType)
					  ?? throw new ArgumentNullException()
				);

				services.AddSingleton(interfaceType, serviceMock);
			}
			
			foreach ((var interfaceType, var serviceMock) in _scheduleCourseServiceMock.GetMocks())
			{
				services.Remove(
					services.SingleOrDefault(d => d.ServiceType == interfaceType)
					  ?? throw new ArgumentNullException()
				);

				services.AddSingleton(interfaceType, serviceMock);
			}
			
			foreach ((var interfaceType, var serviceMock) in _enrolmentServiceMock.GetMocks())
			{
				services.Remove(
					services.SingleOrDefault(d => d.ServiceType == interfaceType)
					  ?? throw new ArgumentNullException()
				);

				services.AddSingleton(interfaceType, serviceMock);
			}
		});
    }
}




