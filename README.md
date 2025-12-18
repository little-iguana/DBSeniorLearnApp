# DBSeniorLearnApp
Project 1 from the Diploma of Programming at Tafe Petersham

This project has been pushed here as it was at the end of the course.
The related assignments required this project to be constructed in 10 weeks in teams of 2.
The project was created and managed through azure dev ops throughout the course.


# Running the app
Requires T-SQL (MS SqlServer) and C# .NET core to start serverside.
Ensure the connection strings in the following (in src/DBSeniorLearnApp.UI/) are correct for your usecase:
- appsettings.json
- appsettings.Development.json
- appsettings.Testing.json
Access client side by entering `localhost:5118` into a browser.

Use to start the application
`> make start`

Use to execute the tests
`> make tests`

Executing the tests will reset the database by default, but one of each account type is seeded in the database on startup.
See the Makefile for all options
