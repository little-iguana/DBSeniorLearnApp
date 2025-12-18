using System.Linq;
using System.IO;
using Microsoft.EntityFrameworkCore;
using JsonHandler = System.Text.Json;
using DataAccess = DBSeniorLearnApp.DataAccess;

namespace DBSeniorLearnApp.Tests;

// This whole class is a hack-y way outta a lotta different issues, but its better than previous bodge solutions
internal static class DatabaseManager
{
	private static string _UiProjectFolder = GetProjectFolderPath(@"..\..\src\DBSeniorLearnApp.UI");
	
	
	internal static void ResetDatabase()
	{
        using (DataAccess::ServiceDbContext context = GenerateAppDbContext())
        {
			try {
				context.Database.ExecuteSql($"EXEC usp_ResetDatabase;");
			} catch (Microsoft.Data.SqlClient.SqlException exception) {
				throw exception;
				// create new usp_ResetDatabase stored procedure
				
				// string usp_ResetDatabase = GetProjectFolderPath("./SqlFiles/usp_ResetDatabase.sql");
				// using (StreamReader sr = new StreamReader(usp_ResetDatabase)) {
					// string storedProcedure = sr.ReadToEnd();
					// context.Database.ExecuteSql(storedProcedure);
				// }
			}
        }
	}
	
	
    internal static DataAccess::ServiceDbContext GenerateAppDbContext(string connectionStringName)
	{
        var optionsBuilder = new DbContextOptionsBuilder<DataAccess::ServiceDbContext>();
		
        optionsBuilder.UseSqlServer(GetConnectionString(connectionStringName)
			?? throw new System.ArgumentException($"{connectionStringName} does not point to a valid connection string"));

        return new DataAccess::ServiceDbContext(optionsBuilder.Options);
    }
	
    internal static DataAccess::ServiceDbContext GenerateAppDbContext()
	{
        return GenerateAppDbContext(DBSeniorLearnApp.UI.Program.GlobalConnectionStringName);
    }
	
	internal static string? GetConnectionString(string stringName)
	{
		string filePath = _UiProjectFolder + @"\appsettings.Development.json";
		string json = "";
		using (StreamReader sr = new StreamReader(filePath))
		{
			json = sr.ReadToEnd();
		}
		
		using (JsonHandler::JsonDocument document = JsonHandler::JsonDocument.Parse(json))
		{
			JsonHandler::JsonElement root = document.RootElement;
			
			try {
				return root.GetProperty("ConnectionStrings").GetProperty(stringName).ToString();
			} catch {}
		}
		
		return null;
	}
	
	
	private static string GetProjectFolderPath(string appendedToBaseDirectory)
	{
		string relativePath = Path.Combine(
			System.AppDomain.CurrentDomain.BaseDirectory,
			"..",
			"..",
			"..",
			appendedToBaseDirectory
		);
		
		return Path.GetFullPath(relativePath);
	}
}






