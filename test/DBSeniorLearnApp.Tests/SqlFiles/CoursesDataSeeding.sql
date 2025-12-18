

USE DBSeniorLearnDB;
GO

--TRUNCATE TABLE Courses;

/*
INSERT INTO Members (
	Firstname,
	Lastname,
	PhoneNumber,
	EmailAddress,
	PaidStatus,
	MemberNumber,
	DateRegistered,
	DateUpdatedPaidStatus
) VALUES (
	'Millie',
	'Billie',
	'0404123456',
	'millie.billie@hillie.com',
	1,
	'RR-2762',
	'2025-10-28 3:00:00',
	'2025-10-28 3:00:00'
);
GO


INSERT INTO ProfessionalMembers (
	StandardMemberId,
	[Description],
	Deactivated,
	IsHonoraryMember
) VALUES (
	(SELECT Id FROM Members WHERE MemberNumber = 'RR-2762'),
	'',
	0,
	0
);
GO
*/


INSERT INTO Courses (
	InstructorId,
	Title,
	[Description],
	StartTime,
	EndTime,
	Prerequisites,
	[Status]
) VALUES (
	(SELECT Id FROM Members WHERE EmailAddress = 'millie.billie@hillie.com'),
	'How to bake a cake',
	'Cake baking 101',
	DATEADD(DAY, 1, GETDATE()) + ' 4:00:00',
	DATEADD(DAY, 1, GETDATE()) + ' 5:00:00',
	'',
	'Scheduled'
);

INSERT INTO Courses (
	InstructorId,
	Title,
	[Description],
	StartTime,
	EndTime,
	Prerequisites,
	[Status]
) VALUES (
	(SELECT Id FROM Members WHERE EmailAddress = 'millie.billie@hillie.com'),
	'How to cook a steak',
	'The biggest beef steak-',
	DATEADD(DAY, 1, GETDATE()) + ' 5:00:00',
	DATEADD(DAY, 1, GETDATE()) + ' 6:00:00',
	'',
	'Scheduled'
);

INSERT INTO Courses (
	InstructorId,
	Title,
	[Description],
	StartTime,
	EndTime,
	Prerequisites,
	[Status]
) VALUES (
	(SELECT Id FROM Members WHERE EmailAddress = 'millie.billie@hillie.com'),
	'How to make a shake',
	'Milk shakes for the shakers',
	DATEADD(DAY, 1, GETDATE()) + ' 6:00:00',
	DATEADD(DAY, 1, GETDATE()) + ' 7:00:00',
	'',
	'Scheduled'
);
GO


-- DELETE FROM Courses WHERE InstructorId = 3;
-- UPDATE Courses SET InstructorId = 3 WHERE InstructorId = 1;

-- UPDATE Courses SET StartTime = '2025-10-31 6:00:00'

-- TRUNCATE TABLE Courses;

SELECT * FROM Courses;
SELECT * FROM CourseEnrolments;

DELETE FROM Courses WHERE Status = 'Draft';
