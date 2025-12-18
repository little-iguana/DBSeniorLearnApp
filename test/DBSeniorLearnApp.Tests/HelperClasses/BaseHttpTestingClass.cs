using System.Net.Http;
using DBSeniorLearnApp.Tests;
using DBSeniorLearnApp.Tests.Mocks;

namespace DBSeniorLearnApp.UI.Controllers.Tests;

public class BaseHttpTestingClass
{
    private readonly CustomWebApplicationFactory _webApplicationFactory;
	
    public MemberApplicationServiceMock memberApplicationServiceMock { get; }
    public ScheduleCourseServiceMock scheduleCourseServiceMock { get; }
    public EnrolmentServiceMock enrolmentServiceMock { get; }

    public BaseHttpTestingClass()
    {
		this.memberApplicationServiceMock = new MemberApplicationServiceMock();
		this.scheduleCourseServiceMock = new ScheduleCourseServiceMock();
		this.enrolmentServiceMock = new EnrolmentServiceMock();
		
        this._webApplicationFactory = new CustomWebApplicationFactory(
			memberApplicationServiceMock,
			scheduleCourseServiceMock,
			enrolmentServiceMock
		);
    }

    public HttpClient GetClient() => _webApplicationFactory.CreateClient();
}
