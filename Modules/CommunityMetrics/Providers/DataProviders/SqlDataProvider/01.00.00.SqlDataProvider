CREATE TABLE dbo.hccm_Activity
(
    ActivityId int NOT NULL IDENTITY (1, 1),
    ActivityName nvarchar(50) NOT NULL,
    Description nvarchar(255) NULL,
    TypeName nvarchar(255) NULL,
    Factor float NOT NULL,
    IsActive bit NOT NULL,
    LastExecutionDate datetime NOT NULL,
    MetricType int NOT NULL,
    UserFilter nvarchar(500) NOT NULL,
    MinDaily int NOT NULL,
    MaxDaily int NOT NULL,
    CreatedByUserID int NOT NULL,
    CreatedOnDate datetime NOT NULL,
    LastModifiedByUserID int NOT NULL,
    LastModifiedOnDate datetime NOT NULL,
)  ON [PRIMARY]
GO

ALTER TABLE dbo.hccm_Activity ADD CONSTRAINT
	PK_hccm_Activity PRIMARY KEY CLUSTERED 
	(
	ActivityId
	) ON [PRIMARY]

GO

CREATE UNIQUE NONCLUSTERED INDEX IX_hccm_Activity ON dbo.hccm_Activity
	(
	ActivityName
	) ON [PRIMARY]
GO

CREATE TABLE dbo.hccm_ActivitySetting
(
	ActivitySettingId int NOT NULL IDENTITY (1, 1),
	ActivityId int NOT NULL,
	SettingName nvarchar(50) NOT NULL,
	SettingValue nvarchar(2000) NOT NULL,
    CreatedByUserID int NOT NULL,
    CreatedOnDate datetime NOT NULL,
    LastModifiedByUserID int NOT NULL,
    LastModifiedOnDate datetime NOT NULL,
)  ON [PRIMARY]
GO

ALTER TABLE dbo.hccm_ActivitySetting ADD CONSTRAINT
	PK_hccm_ActivitySetting PRIMARY KEY CLUSTERED 
	(
	ActivitySettingId
	) ON [PRIMARY]

GO

CREATE UNIQUE NONCLUSTERED INDEX IX_hccm_ActivitySetting ON dbo.hccm_ActivitySetting
	(
	ActivityId,
	SettingName
	) ON [PRIMARY]
GO

ALTER TABLE dbo.hccm_ActivitySetting ADD CONSTRAINT
	FK_hccm_ActivitySetting_hccm_Activity FOREIGN KEY
	(
	ActivityId
	) REFERENCES dbo.hccm_Activity
	(
	ActivityId
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	 NOT FOR REPLICATION

GO

CREATE TABLE dbo.hccm_UserActivity
(
	UserActivityId bigint NOT NULL IDENTITY (1, 1),
	ActivityId int NOT NULL,
	UserId int NOT NULL,
	[Date] datetime NOT NULL,
	Count int NOT NULL,
	Notes nvarchar(255) NULL,
	CreatedOnDate datetime NOT NULL
)  ON [PRIMARY]
GO

ALTER TABLE dbo.hccm_UserActivity ADD CONSTRAINT
	PK_hccm_UserActivity PRIMARY KEY CLUSTERED 
	(
	UserActivityId
	) ON [PRIMARY]

GO

ALTER TABLE dbo.hccm_UserActivity ADD CONSTRAINT
	FK_hccm_UserActivity_hccm_Activity FOREIGN KEY
	(
	ActivityId
	) REFERENCES dbo.hccm_Activity
	(
	ActivityId
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	 NOT FOR REPLICATION

GO

CREATE UNIQUE NONCLUSTERED INDEX IX_hccm_UserActivity ON dbo.hccm_UserActivity
(
	ActivityId,
	UserId,
	[Date]
) ON [PRIMARY]
GO

CREATE TABLE dbo.hccm_UserReward
(
	UserRewardId int NOT NULL IDENTITY (1, 1),
	UserId int NOT NULL,
	[Date] datetime NOT NULL,
	Description nvarchar(255) NULL,
	Value int NOT NULL,
	CreatedOnDate datetime NOT NULL
)  ON [PRIMARY]
GO

ALTER TABLE dbo.hccm_UserReward ADD CONSTRAINT
	PK_hccm_UserReward PRIMARY KEY CLUSTERED 
	(
	UserRewardId
	) ON [PRIMARY]

GO


create procedure dbo.hccm_GetActivities

as

select *
from dbo.hccm_Activity
order by ActivityName

GO

create procedure dbo.hccm_GetActivity

@ActivityId int

as

select *
from dbo.hccm_Activity
where ActivityId = @ActivityId

GO

create procedure dbo.hccm_UpdateActivity

@ActivityId           int,
@ActivityName         nvarchar(50),
@Description          nvarchar(255),
@TypeName             nvarchar(255),
@Factor               float,
@IsActive             bit,
@LastExecutionDate    datetime,
@MetricType           int,
@UserFilter           nvarchar(500),
@MinDaily             int,
@MaxDaily             int,
@UserID               int

as

if @ActivityId = -1
begin
  insert into dbo.hccm_Activity (
    ActivityName,
    Description,
    TypeName,
    Factor,
    IsActive,
    LastExecutionDate,
    MetricType,
    UserFilter,
	MinDaily,
	MaxDaily,
    CreatedByUserID,
    CreatedOnDate,
    LastModifiedByUserID,
    LastModifiedOnDate
  )
  values (
    @ActivityName,
    @Description,
    @TypeName,
    @Factor,
    @IsActive,
    @LastExecutionDate,
    @MetricType,
    @UserFilter,
	@MinDaily,
	@MaxDaily,
    @UserID,
    getdate(),
    @UserID,
    getdate()
  )

  select @ActivityId = SCOPE_IDENTITY()
end
else
begin
  update dbo.hccm_Activity
  set ActivityName = @ActivityName,
      Description = @Description,
      TypeName = @TypeName,
      Factor = @Factor,
      IsActive = @IsActive,
      LastExecutionDate = @LastExecutionDate,
      MetricType = @MetricType,
      UserFilter = @UserFilter,
	  MinDaily = @MinDaily,
	  MaxDaily = @MaxDaily,
      LastModifiedByUserID = @UserID,
      LastModifiedOnDate = getdate()
  where ActivityId = @ActivityId
end

select @ActivityId as ActivityId

GO

create procedure dbo.hccm_GetActivitySettings

@ActivityId int

as

select *
from dbo.hccm_ActivitySetting
where ActivityId = @ActivityId

GO

create procedure dbo.hccm_UpdateActivitySetting

@ActivityId    int,
@SettingName   nvarchar(50),
@SettingValue  nvarchar(2000),
@UserID        int

as

if not exists (select 1 from dbo.hccm_ActivitySetting where ActivityId = @ActivityId and SettingName = @SettingName)
begin
  insert into dbo.hccm_ActivitySetting (
    ActivityId,
    SettingName,
    SettingValue,
    CreatedByUserID,
    CreatedOnDate,
    LastModifiedByUserID,
    LastModifiedOnDate
  )
  values (
    @ActivityId,
    @SettingName,
    @SettingValue,
    @UserID,
    getdate(),
    @UserID,
    getdate()
  )
end
else
begin
  update dbo.hccm_ActivitySetting
  set SettingValue = @SettingValue,
      LastModifiedByUserID = @UserID,
      LastModifiedOnDate = getdate()
  where ActivityId = @ActivityId
  and SettingName = @SettingName
end

GO

create procedure dbo.hccm_UpdateUserActivity

@ActivityId    int,
@UserId        int,
@Date          datetime,
@Count         int
@Notes         nvarchar(255)

as

if not exists (select 1 from dbo.hccm_UserActivity where ActivityId = @ActivityId and UserId = @UserId and [Date] = @Date)
begin
  insert into dbo.hccm_UserActivity (
    ActivityId,
    UserId,
    [Date],
    Count,
	Notes,
    CreatedOnDate
  )
  values (
    @ActivityId,
    @UserId,
    @Date,
    @Count,
	@Notes,
    getdate()
  )
end
else
begin
  update dbo.hccm_UserActivity
  set Count = @Count,
      Notes = @Notes,
      CreatedOnDate = getdate()
  where ActivityId = @ActivityId
  and UserId = @UserId
  and [Date] = @Date
end
GO

create procedure dbo.hccm_GetUserActivity

@ActivityId    int,
@UserId        int,
@Date          datetime

as

select *
from dbo.hccm_UserActivity
where ActivityId = @ActivityId
and UserId = @UserId
and Date = @Date

GO

create procedure dbo.hccm_GetUserActivityCount

@ActivityId    int,
@UserId        int,
@Date          datetime

as

select isnull(sum(Count),0) as [Count]
from dbo.hccm_UserActivity
where ActivityId = @ActivityId
and UserId = @UserId
and Date < @Date

GO

create procedure dbo.hccm_GetUserActivities

@UserId        int,
@ActivityId    int,
@StartDate     datetime,
@EndDate       datetime,
@Summary       bit,
@Rows          int

as

if @Summary = 1
begin
  select top (@Rows) ua.UserId, u.DisplayName, u.CreatedOnDate, '' as ActivityName, sum(Count * a.Factor) as [Score]
  from dbo.hccm_UserActivity ua
  inner join dbo.hccm_Activity a on ua.ActivityId = a.ActivityId
  inner join dbo.Users u on ua.UserId = u.UserId
  where (ua.UserId = @UserId or @UserId is null)
  and   (ua.ActivityId = @ActivityId or @ActivityId is null)
  and   (ua.Date >= @StartDate or @StartDate is null)
  and   (ua.Date <= @EndDate or @EndDate is null)
  and   a.IsActive = 1
  group by ua.UserId, u.DisplayName, u.CreatedOnDate
  order by [Score] desc
end
else
begin
  select top (@Rows) ua.UserId, u.DisplayName, u.CreatedOnDate, a.ActivityName, sum(Count * a.Factor) as [Score]
  from dbo.hccm_UserActivity ua
  inner join dbo.hccm_Activity a on ua.ActivityId = a.ActivityId
  inner join dbo.Users u on ua.UserId = u.UserId
  where (ua.UserId = @UserId or @UserId is null)
  and   (ua.ActivityId = @ActivityId or @ActivityId is null)
  and   (ua.Date >= @StartDate or @StartDate is null)
  and   (ua.Date <= @EndDate or @EndDate is null)
  and   a.IsActive = 1
  group by ua.UserId, u.DisplayName, u.CreatedOnDate, a.ActivityName
  order by u.DisplayName, a.ActivityName 
end
GO

create procedure [dbo].[hccm_GetDailyActivity]

@ActivityId int,
@StartDate datetime,
@EndDate datetime

as

select ua.ActivityId, a.ActivityName, convert(varchar,ua.Date,101) AS Date, count(*) As Count
from dbo.hccm_UserActivity ua
inner join dbo.hccm_Activity a on ua.ActivityId = a.ActivityId 
where ((ua.ActivityId = @ActivityId) or @ActivityId is null) 
and ((ua.Date >= @StartDate) or @StartDate is null)
and ((ua.Date <= @EndDate) or @EndDate is null)
group by ua.ActivityId, a.ActivityName, convert(varchar,ua.Date,101)
order by ua.ActivityId, a.ActivityName, convert(varchar,ua.Date,101)

GO

INSERT INTO dbo.Schedule
	( TypeFullName, [TimeLapse], [TimeLapseMeasurement], [RetryTimeLapse], [RetryTimeLapseMeasurement], [RetainHistoryNum], [AttachToEvent], [CatchUpEnabled], [Enabled], [ObjectDependencies], [Servers], [FriendlyName])
VALUES ( 'HCC.CommunityMetrics.ActivityJob, HCC.CommunityMetrics', 1, 'd', 0, 's', 10, '', 0, 1, '', null, 'Community Metrics Job' )
GO


