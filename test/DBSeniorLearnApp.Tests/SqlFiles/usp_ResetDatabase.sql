/*--------------------------------------------------

Reset database stored procedure
Edit to remove and re-add foreign keys to allow tables to be truncated

--------------------------------------------------*/

USE DBSeniorLearnDB;
GO

CREATE PROCEDURE usp_ResetDatabase
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


	-- Remove all foreign keys before truncating
	ALTER TABLE [CourseEnrolments]
	DROP CONSTRAINT [FK_CourseEnrolments_Courses_CourseId];

	ALTER TABLE [CourseEnrolments]
	DROP CONSTRAINT [FK_CourseEnrolments_Members_MemberId];

	ALTER TABLE [Courses]
	DROP CONSTRAINT [FK_Courses_ProfessionalMembers_InstructorId];

	ALTER TABLE [ProfessionalMembers]
	DROP CONSTRAINT [FK_ProfessionalMembers_Members_StandardMemberId];


	ALTER TABLE [AspNetRoleClaims]
	DROP CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId];

	ALTER TABLE [AspNetUserClaims]
	DROP CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId];

	ALTER TABLE [AspNetUserLogins]
	DROP CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId];

	ALTER TABLE [AspNetUserRoles]
	DROP CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId];

	ALTER TABLE [AspNetUserRoles]
	DROP CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId];

	ALTER TABLE [AspNetUserTokens]
	DROP CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId];



    -- Truncate all tables
	TRUNCATE TABLE [CourseEnrolments];
	TRUNCATE TABLE [Courses];
	TRUNCATE TABLE [ProfessionalMembers];
	TRUNCATE TABLE [Members];

	TRUNCATE TABLE [AspNetRoleClaims];
	TRUNCATE TABLE [AspNetRoles];
	TRUNCATE TABLE [AspNetUserClaims];
	TRUNCATE TABLE [AspNetUserLogins];
	TRUNCATE TABLE [AspNetUserRoles];
	TRUNCATE TABLE [AspNetUsers];
	TRUNCATE TABLE [AspNetUserTokens];
	


	-- Add the foreign keys again after truncating
	ALTER TABLE [ProfessionalMembers]
	ADD CONSTRAINT [FK_ProfessionalMembers_Members_StandardMemberId] 
	FOREIGN KEY ([StandardMemberId]) REFERENCES [Members] ([Id]);

	ALTER TABLE [Courses]
	ADD CONSTRAINT [FK_Courses_ProfessionalMembers_InstructorId]
	FOREIGN KEY ([InstructorId]) REFERENCES [ProfessionalMembers] ([StandardMemberId]);

	ALTER TABLE [CourseEnrolments]
	ADD CONSTRAINT [FK_CourseEnrolments_Courses_CourseId]
	FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]);

	ALTER TABLE [CourseEnrolments]
	ADD CONSTRAINT [FK_CourseEnrolments_Members_MemberId]
	FOREIGN KEY ([MemberId]) REFERENCES [Members] ([Id]);


	ALTER TABLE [AspNetRoleClaims]
	ADD CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId]
	FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]);

	ALTER TABLE [AspNetUserClaims]
	ADD CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId]
	FOREIGN KEY ([UserId]) REFERENCES [AspNetRoles] ([Id]);

	ALTER TABLE [AspNetUserRoles]
	ADD CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId]
	FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] (Id);

	ALTER TABLE [AspNetUserRoles]
	ADD CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId]
	FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]);

	ALTER TABLE [AspNetUserLogins]
	ADD CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId]
	FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]);

	ALTER TABLE [AspNetUserTokens]
	ADD CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId]
	FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]);

	-- Populate database with seed data
	
END
GO


DROP PROCEDURE usp_ResetDatabase;
EXEC usp_ResetDatabase;

