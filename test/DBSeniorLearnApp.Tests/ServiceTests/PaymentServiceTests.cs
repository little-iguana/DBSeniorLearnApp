namespace DBSeniorLearnApp.Services.Tests;

// [Ignore("Test isolation")]
public class PaymentServiceTests {
	
	[TestCase("4111111111111111")]
	[TestCase("378282246310005")]
	[TestCase("30569309025904")]
	[TestCase("6011000990139424")]
	public void TestValidateCardNumber_ShouldReturnTrue(string number)
	{
		Assert.True(PaymentService.ValidateCardNumber(number));
	}
	
	[TestCase("1234567")] // length out of bounds
	[TestCase("12345678912345678900")]
	[TestCase("5555555655554444")] // checksum should fail
	[TestCase("76009244561")]
	[TestCase("40128888A8881881")] // includes non-digit characters
	[TestCase("40128888+8881881")]
	public void TestInvalidCardNumber_ShouldReturnFalse(string number)
	{
		Assert.False(PaymentService.ValidateCardNumber(number));
	}
	
	[TestCase("123")]
	[TestCase("686")]
	public void TestValidCCV_ShouldReturnTrue(string number)
	{
		Assert.True(PaymentService.ValidateCCV(number));
	}
	
	[TestCase("12")] // invalid length
	[TestCase("1234")]
	[TestCase("12a")] // includes non-digit characters
	[TestCase("12-")]
	[TestCase("-12")] // catches original validate method as only checking string length and valid int
	public void TestInvalidCCV_ShouldReturnFalse(string number)
	{
		Assert.False(PaymentService.ValidateCCV(number));
	}
	
	[TestCase("12/34")]
	[TestCase("1/26")] // originally parsed as 1/01/0001 12:00:00 AM
	[TestCase("2/34")]
	[TestCase("10/23126")] // date set 21,100 years in the future
	public void TestValidExpiry_ShouldReturnTrue(string date)
	{
		Assert.True(PaymentService.ValidateExpiry(date));
	}
	
	[TestCase("10/10/26")] // does not follow `mm/yy` syntax
	[TestCase("10/")]
	[TestCase("10")]
	[TestCase("10026")]
	[TestCase("-5/-35")]
	[TestCase("1-26")] // invalid characters
	[TestCase("mm/yy")]
	[TestCase("0/26")] // invalid date
	[TestCase("10/0")]
	[TestCase("8/25")] // date in the past
	public void TestInvalidExpiry_ShouldReturnFalse(string date)
	{
		Assert.False(PaymentService.ValidateExpiry(date));
	}
	
	[TestCase("4111111111111111", "123", "10/26")]
	public void TestTakePayment_ShouldReturnTrue(string cardNo, string ccv, string expiry)
	{
		System.Random randomObj = new System.Random(0);
		Assert.True(PaymentService.TakePayment("Unimportant Name", cardNo, ccv, expiry, randomObj));
	}
}







