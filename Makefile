
.DEFAULT_GOAL := buildsilent
PROJECT = src\DBSeniorLearnApp.UI\DBSeniorLearnApp.UI.csproj

build:
	@echo ------------------------------------------------------------
	@echo building...
	dotnet build src\DBSeniorLearnApp
	@echo ------------------------------------------------------------
	dotnet build src\DBSeniorLearnApp.UI
	@echo ------------------------------------------------------------
	dotnet build test\DBSeniorLearnApp.Tests
buildsilent:
	@dotnet build src\DBSeniorLearnApp
	@echo ------------------------------------------------------------
	@dotnet build src\DBSeniorLearnApp.UI
	@echo ------------------------------------------------------------
	@dotnet build test\DBSeniorLearnApp.Tests
clean:
	dotnet clean
restore:
	dotnet restore
start:
	@echo starting $(PROJECT)
	dotnet run --project $(PROJECT) --launch-profile http
run:
	@echo starting $(PROJECT)
	dotnet run --project $(PROJECT)
updatedb:
	@echo updating db...
	dotnet ef database update --project $(PROJECT) -c ServiceDbContext
	@echo ------------------------------------------------------------
	dotnet ef database update --project $(PROJECT) -c IdentificationDbContext
tests:
	@echo testing...
	dotnet test
