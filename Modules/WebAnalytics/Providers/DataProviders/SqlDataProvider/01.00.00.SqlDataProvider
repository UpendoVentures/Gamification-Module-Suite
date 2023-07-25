CREATE TABLE dbo.hccm_Visitors
	(
	VisitorId int NOT NULL IDENTITY (1, 1),
    PortalId int NOT NULL,
    UserId int NULL,
	CreatedOnDate datetime NOT NULL
	)  ON [PRIMARY]
GO

ALTER TABLE dbo.hccm_Visitors ADD CONSTRAINT
	PK_hccm_Visitors PRIMARY KEY CLUSTERED 
	(
	VisitorId
	) ON [PRIMARY]

GO

ALTER TABLE dbo.hccm_Visitors ADD CONSTRAINT
	FK_hccm_Visitors_hccm_Portals FOREIGN KEY
	(
	PortalId
	) REFERENCES dbo.Portals
	(
	PortalId
	) ON UPDATE NO ACTION 
	 ON DELETE CASCADE
	
GO

create procedure [dbo].[hccm_AddVisitor]

@PortalId int

as

declare @VisitorId int

insert into dbo.hccm_Visitors (
  PortalId,
  UserId,
  CreatedOnDate
)
values (
  @PortalId,
  null,
  getdate()
)

select @VisitorId = SCOPE_IDENTITY() 

select @VisitorId as VisitorId

GO

create procedure [dbo].[hccm_UpdateVisitor]

@VisitorId int,
@UserId int

as

update dbo.hccm_Visitors
set UserId = @UserId
where VisitorId = @VisitorId

GO

create procedure [dbo].[hccm_GetVisitor]

@VisitorId int

as

select hccm_Visitors.*, Users.Username, Users.DisplayName
from dbo.hccm_Visitors
left outer join dbo.Users on hccm_Visitors.UserId = Users.UserId
where VisitorId = @VisitorId

GO

CREATE TABLE dbo.hccm_Visits
	(
	VisitId bigint NOT NULL IDENTITY (1, 1),
    PortalId int NOT NULL,
    Date datetime NOT NULL,
	VisitorId int NOT NULL,
	TabId int NULL,
	UserId int NULL,
	IP nvarchar(50) NOT NULL,
    Country nvarchar(50) NOT NULL,
	Region nvarchar(50) NOT NULL,
	City nvarchar(50) NOT NULL,
	Latitude nvarchar(50) NOT NULL,
	Longitude nvarchar(50) NOT NULL,
    Language nvarchar(50) NOT NULL,
    Domain nvarchar(255) NOT NULL,
	URL nvarchar(2048) NOT NULL,
	UserAgent nvarchar(512) NOT NULL,
    DeviceType nvarchar(50) NOT NULL,
	Device nvarchar(255) NOT NULL,
	Platform nvarchar(255) NOT NULL,
	Browser nvarchar(255) NOT NULL,
	ReferrerDomain nvarchar(255) NOT NULL,
	ReferrerURL nvarchar(2048) NOT NULL,
    Server nvarchar(50) NOT NULL,
    Campaign nvarchar(50) NOT NULL,
    SessionId uniqueidentifier NOT NULL,
    RequestId uniqueidentifier NOT NULL,
    LastRequestId uniqueidentifier NULL
	)  ON [PRIMARY]
GO

ALTER TABLE dbo.hccm_Visits ADD CONSTRAINT
	PK_hccm_Visits PRIMARY KEY CLUSTERED 
	(
	VisitId
	) ON [PRIMARY]

GO

ALTER TABLE dbo.hccm_Visits ADD CONSTRAINT
	FK_hccm_Visits_hccm_Visitors FOREIGN KEY
	(
	VisitorId
	) REFERENCES dbo.hccm_Visitors
	(
	VisitorId
	) ON UPDATE NO ACTION 
	 ON DELETE CASCADE 
	
GO

create procedure [dbo].[hccm_AddVisit]

@PortalId int,
@Date datetime,
@VisitorId int,
@TabId int,
@UserId int,
@IP nvarchar(50),
@Country nvarchar(50),
@Region nvarchar(50),
@City nvarchar(50),
@Latitude nvarchar(50),
@Longitude nvarchar(50),
@Language nvarchar(50),
@Domain nvarchar(255),
@URL nvarchar(2048),
@UserAgent nvarchar(512),
@DeviceType nvarchar(50),
@Device nvarchar(255),
@Platform nvarchar(255),
@Browser nvarchar(255),
@ReferrerDomain nvarchar(255),
@ReferrerURL nvarchar(2048),
@Server nvarchar(50),
@Campaign nvarchar(50),
@SessionId uniqueidentifier,
@RequestId uniqueidentifier,
@LastRequestId uniqueidentifier

as

insert into dbo.hccm_Visits (
  PortalId,
  Date,
  VisitorId,
  TabId,
  UserId,
  IP,
  Country,
  Region,
  City,
  Latitude,
  Longitude,
  Language,
  Domain,
  URL,
  UserAgent,
  DeviceType,
  Device,
  Platform,
  Browser,
  ReferrerDomain,
  ReferrerURL,
  Server,
  Campaign,
  SessionId,
  RequestId,
  LastRequestId
)
values (
  @PortalId,
  @Date,
  @VisitorId,
  @TabId,
  @UserId,
  @IP,
  @Country,
  @Region,
  @City,
  @Latitude,
  @Longitude,
  @Language,
  @Domain,
  @URL,
  @UserAgent,
  @DeviceType,
  @Device,
  @Platform,
  @Browser,
  @ReferrerDomain,
  @ReferrerURL,
  @Server,
  @Campaign,
  @SessionId,
  @RequestId,
  @LastRequestId
)

GO

create procedure [dbo].[hccm_GetVisit]

@VisitId bigint

as

select *
from dbo.hccm_Visits
left outer join dbo.Tabs on hccm_Visits.TabId = Tabs.TabId
left outer join dbo.Users on hccm_Visits.UserId = Users.UserId
where VisitId = @VisitId

GO

create procedure [dbo].[hccm_GetVisitsDashboard]

@PortalId int,
@StartDate datetime,
@EndDate datetime

as

select count(*) as Views, count(distinct SessionId) As Visits, count(distinct hccm_Visits.VisitorId) As Visitors, count(distinct UserId) As Users
from dbo.hccm_Visits
where PortalId = @PortalId 
and ((hccm_Visits.Date >= @StartDate) or @StartDate is null)
and ((hccm_Visits.Date <= @EndDate) or @EndDate is null)

select convert(varchar,Date,101) AS Date, count(*) As Views
from dbo.hccm_Visits
where PortalId = @PortalId 
and ((hccm_Visits.Date >= @StartDate) or @StartDate is null)
and ((hccm_Visits.Date <= @EndDate) or @EndDate is null)
and TabId is not null
group by convert(varchar,Date,101)
order by convert(varchar,Date,101)

select convert(varchar,Date,101) AS Date, count(distinct SessionId) As Visits
from dbo.hccm_Visits
where PortalId = @PortalId 
and ((hccm_Visits.Date >= @StartDate) or @StartDate is null)
and ((hccm_Visits.Date <= @EndDate) or @EndDate is null)
and TabId is not null
group by convert(varchar,Date,101)
order by convert(varchar,Date,101)

select convert(varchar,Date,101) AS Date, count(distinct VisitorId) As Visitors
from dbo.hccm_Visits
where PortalId = @PortalId 
and ((hccm_Visits.Date >= @StartDate) or @StartDate is null)
and ((hccm_Visits.Date <= @EndDate) or @EndDate is null)
and TabId is not null
group by convert(varchar,Date,101)
order by convert(varchar,Date,101)

select convert(varchar,Date,101) AS Date, count(distinct UserId) As Users
from dbo.hccm_Visits
where PortalId = @PortalId 
and ((hccm_Visits.Date >= @StartDate) or @StartDate is null)
and ((hccm_Visits.Date <= @EndDate) or @EndDate is null)
and TabId is not null
group by convert(varchar,Date,101)
order by convert(varchar,Date,101)

GO

create procedure [dbo].[hccm_GetVisitsReport]

@SQL nvarchar(3000),
@PortalId int,
@StartDate datetime,
@EndDate datetime

as

exec sp_executesql @SQL, N'@PortalId int, @StartDate datetime, @EndDate datetime', @PortalId, @StartDate, @EndDate

GO

create procedure [dbo].[hccm_PurgeVisits]

as

declare @RetentionHistory int
declare @PortalId int

select @PortalId = min(PortalId)
from dbo.Portals

while @PortalId is not null
begin
  select @RetentionHistory = 90

  select @RetentionHistory = convert(int,SettingValue)
  from dbo.PortalSettings
  where PortalId = @PortalId
  and SettingName = 'RetentionHistory'

  delete top (100)
  from dbo.hccm_Visits
  where PortalId = @PortalId
  and Date < DATEADD(day, -(@RetentionHistory), getdate())

  select @PortalId = min(PortalId)
  from dbo.Portals
  where PortalId > @PortalId
end

GO

INSERT INTO dbo.Schedule
	( TypeFullName, [TimeLapse], [TimeLapseMeasurement], [RetryTimeLapse], [RetryTimeLapseMeasurement], [RetainHistoryNum], [AttachToEvent], [CatchUpEnabled], [Enabled], [ObjectDependencies], [Servers], [FriendlyName])
VALUES ( 'HCC.WebAnalytics.VisitorJob, HCC.WebAnalytics', 1, 'm', 1, 'm', 10, '', 0, 1, '', null, 'Visitor Tracking Job' )
GO




