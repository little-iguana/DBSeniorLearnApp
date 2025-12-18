namespace DBSeniorLearnApp.Services;

public class ValidatorService {
	
	[System.Obsolete("This method for validating a phone number is obselete.", true)]
	public static bool CheckEmail(string email) {
		var trimmedEmail = email.Trim();
		
		if (trimmedEmail.EndsWith(".")) {
			return false;
		}
		try {
			var addr = new System.Net.Mail.MailAddress(email);
			return addr.Address == trimmedEmail;
		}
		catch {
			return false;
		}
	}
	
	[System.Obsolete("This method for validating a phone number is obselete.", true)]
	public static bool CheckPhone(string num) {
		throw new System.NotImplementedException();
	}
}
