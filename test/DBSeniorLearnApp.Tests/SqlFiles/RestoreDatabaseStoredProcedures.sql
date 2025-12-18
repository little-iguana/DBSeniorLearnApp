/*--------------------------------------------------

Restore database from backup stored procedure
Backup database stored procedure

--------------------------------------------------*/

USE DBSeniorLearnDB;
GO

ALTER DATABASE DBSeniorLearnDB SET RECOVERY FULL;

GO

-- Execute every week
CREATE PROCEDURE usp_FullBackup
AS
BEGIN
	BACKUP DATABASE [DBSeniorLearnDB]
	TO DISK = N'C:\DatabaseBackups\DBSeniorLearnDB.bak' WITH FORMAT;
END
GO

-- Execute every day
CREATE PROCEDURE usp_DifferentialBackup
AS
BEGIN
	BACKUP DATABASE [DBSeniorLearnDB]
	TO DISK = N'C:\DatabaseBackups\DBSeniorLearnDB.bak' WITH DIFFERENTIAL;
END
GO

-- Execute every 15 minutes
CREATE PROCEDURE usp_TransactionLogBackup
AS
BEGIN
	BACKUP LOG [DBSeniorLearnDB]
	TO DISK = N'C:\DatabaseBackups\DBSeniorLearnDB.bak';

	-- With full backup model, transaction log is automatically truncated on log backup
END
GO

-- Execute to restore to datetime point in time
CREATE PROCEDURE usp_RestoreToPoint
(
	@restoreTime datetime2,
	@file int
)
AS
BEGIN
	RESTORE DATABASE [DBSeniorLearnDB]
	FROM DISK = N'C:\DatabaseBackups\DBSeniorLearnDB.bak';

	RESTORE LOG [BackupExample]
	FROM DISK = N'C:\DatabaseBackups\DBSeniorLearnDB.bak'
	WITH FILE=@file, NORECOVERY, STOPAT = @restoreTime;
END
GO


RESTORE HEADERONLY
FROM DISK = N'C:\DatabaseBackups\DBSeniorLearnDB.bak';



EXEC usp_FullBackup;
EXEC usp_DifferentialBackup;
EXEC usp_TransactionLogBackup;
EXEC usp_RestoreToPoint GETDATE(), 1;




/* -- Backup requirements --

Disaster Recovery Plan
The DRP should include:
. A detailed & prioritised list of critical databases & their dependencies
. Clearly assigned roles and responsibilities
. A step-by-step process of how backups will be accessed, tested, and restored
. A plan for communicating the breach/failure to steakholders, employees, and customers
. Consistent updates and tests to ensure that the plan will succeed in the event of a failure

Define the RPO (Recovery Point Objective)
The RPO is about how much data is acceptable to lose in the event of a failure
Since SeniorLearn is an Australia wide company which handles a lot of customer data,
backups should be fairly frequent, about once per day for full backups, once per 4 or 6
hours for differential backups, and every 15 to 30 minutes for the transaction log.

Define the RTO (Recovery Time Objective)
The RTO is how long is acceptable before the data needs to be restored before the business
faces significant disruptions - SeniorLearn only operates in Australia, and so might not
need data to be restored for hours if it is disrupted in the middle of the night. But if it
happens during the day, the data would have to be restored within the hour.

Backup storage solutions
Backups should follow the 3-2-1 rule, requiring 3 copies of the backed up data:
. 2 onsite copies, and
. 1 offsite copy, ensuring it still complies with the Privacy act 1988

Security of backups
Backups must be securely encrypted and protected, potentially even stored in cold,
offline storage

Regular testing
Backups need to be tested regularly to ensure integrity so that a full restoration can
occur after a failure

-- ------------------ -- */





/*
SELECT [Current LSN],
       [Operation],
       [Transaction Name],
       [Transaction ID],
       [Transaction SID],
       [SPID],
       [Begin Time]
FROM   fn_dblog(null,null)

SELECT file_id, type_desc,
       CAST(FILEPROPERTY(name, 'SpaceUsed') AS decimal(19,4)) * 8 / 1024. AS space_used_mb,
       CAST(size/128.0 - CAST(FILEPROPERTY(name, 'SpaceUsed') AS int)/128.0 AS decimal(19,4)) AS space_unused_mb,
       CAST(size AS decimal(19,4)) * 8 / 1024. AS space_allocated_mb,
       CAST(max_size AS decimal(19,4)) * 8 / 1024. AS max_size_mb
FROM sys.database_files;
*/


