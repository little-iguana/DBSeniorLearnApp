using System;

namespace DBSeniorLearnApp.Services;

public static class PaymentService
{
	public static bool ValidateCardNumber(string number)
	{
		if (number.Length < 8 ||
			number.Length > 19 ||
			!long.TryParse(number, out long ccno)
			)
		{
			return false;
		}
		
		long ccno2 = long.Parse(number);
		long total = 0;
		while (ccno > 10)
		{
			long digitcount = ccno % 100 / 10 * 2;
			if (digitcount > 9)
			{
				digitcount = (digitcount / 10) + (digitcount % 10);
			}
			ccno = ccno / 100;
			total += digitcount;
		}
		total += ccno2 % 10;
		ccno2 /= 10;
		while (ccno2 > 0)
		{
			total += ccno2 % 100 / 10;
			ccno2 /= 100;
		}
		return total % 10 == 0;
	}
	
	public static bool ValidateCCV(string number) {
		if (!int.TryParse(number, out int ccv)) {
			return false;
		}
		if (Math.Floor(Math.Log10(ccv) + 1) != 3) {
			return false;
		}
		return true;
	}
	
	public static bool ValidateExpiry(string date) {
		var tokens = new System.Collections.Generic.List<string>();
		int current = 0;
		string currentToken = "";
		while (current < date.Length) {
			if (Char.IsDigit(date[current])) {
				while (current < date.Length && Char.IsDigit(date[current])) {
					currentToken += date[current];
					current++;
				}
				tokens.Add(currentToken);
				currentToken = "";
				current--;
			} else if (date[current] == '/') {
				tokens.Add(date[current].ToString());
			} else {
				return false;
			}
			current++;
		}
		
		if (tokens.Count != 3) {
			return false;
		}
		
		// check tokens follow `number, "/", number`
		if (!int.TryParse(tokens[0], out int mm) || tokens[1] != "/" || !int.TryParse(tokens[2], out int yy)) {
			return false;
		}
		
		if (mm <= 0 || yy <= 0) {
			return false;
		}
		
		if ((int)Math.Floor(Math.Log10(yy) + 1) == 2) {
			yy += 2000; // 10/26 becomes 10/2026
		}
		
		// check first and last tokens are numbers past the current
		if (yy < DateTime.Now.Year || (yy == DateTime.Now.Year && mm <= DateTime.Now.Month)) {
			return false;
		}
		
		return true;
	}
	
	public static bool TakePayment(string cardName, string cardNo, string ccv, string expiry)
	{
		Random rnd = new Random();
		return ValidateCardNumber(cardNo) && ValidateCCV(ccv) && ValidateExpiry(expiry) && !(rnd.Next(0, 100) < 5);
	}
	
	public static bool TakePayment(string cardName, string cardNo, string ccv, string expiry, Random rnd)
	{
		return ValidateCardNumber(cardNo) && ValidateCCV(ccv) && ValidateExpiry(expiry) && !(rnd.Next(0, 100) < 5);
	}
}





