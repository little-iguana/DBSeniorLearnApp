using System.Threading.Tasks;
using System.Net.Http;

namespace DBSeniorLearnApp.UI.Controllers.Tests;

public class HomeControllerTests : BaseHttpTestingClass
{
	[Test]
	public async Task TestHomeIndex_ShouldReturnSuccessAndCorrectContentTypeAsync()
	{
		HttpClient client = GetClient();
		HttpResponseMessage response = await client.GetAsync("/");
		
		Assert.True(response.IsSuccessStatusCode);
		Assert.That(response.Content.Headers.ContentType.ToString() ?? "", Is.EqualTo("text/html; charset=utf-8"));
	}
}
