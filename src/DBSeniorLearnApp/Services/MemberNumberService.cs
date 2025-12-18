using System.Linq;
using DBSeniorLearnApp.DataAccess;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using DataAccess = DBSeniorLearnApp.DataAccess;
using Models = DBSeniorLearnApp.DataAccess.Models;

namespace DBSeniorLearnApp.Services;

public static class MemberNumberService
{

	// Member numbers are of the form `AA-0000`
	public static bool Validate(string candidate)
	{
		// Check preliminary conditions
		if (candidate.Length != 7)
		{
			return false;
		}

		string comparator = "AA-0000";
		int current = 0;
		while (current < candidate.Length)
		{
			if (System.Char.IsDigit(candidate[current]) ^ System.Char.IsDigit(comparator[current]))
			{
				return false;
			}
			if (System.Char.IsLetter(candidate[current]) ^ System.Char.IsLetter(comparator[current]))
			{
				return false;
			}
			if (candidate[current] == '-' ^ comparator[current] == '-')
			{
				return false;
			}
			current++;
		}
		return true;
	}

	// Generate random member number, check if already in use, then return if not
	public static string NewNumber(DataAccess::ServiceDbContext _context)
	{
		System.Random rnd = new System.Random();
		while (true)
		{
			string output = "";
			output += (char)rnd.Next(65, 91);
			output += (char)rnd.Next(65, 91);
			output += "-"; // O and 0 aren't super easy to distinguish
			for (byte i = 0; i < 4; i++)
			{
				output += rnd.Next(0, 10); // includes lower bound, does not include upper bound
			}

			// check if number in db
			Models::Member? member = _context.Members.FirstOrDefault(n => n.MemberNumber == output);
			if (member == null)
			{
				return output;
			}
		}
	}
}






