CREATE TABLE [dbo].[hccm_Sites]
(
	[SiteId] [int] IDENTITY(1,1) NOT NULL,
    [ModuleId] [int] NOT NULL,
	[URL] [nvarchar](250) NOT NULL,
	[Title] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](2000) NULL,
	[Categories] [nvarchar](500) NULL,
    [IsActive] [bit] NOT NULL,
	[Thumbnail] [nvarchar](250) NULL,
	[CreatedByUserId] [int] NOT NULL,
	[CreatedOnDate] [datetime] NOT NULL,
	[LastModifiedByUserId] [int] NOT NULL,
	[LastModifiedOnDate] [datetime] NOT NULL
        CONSTRAINT [PK_hccm_Sites] PRIMARY KEY CLUSTERED ( [SiteId] ASC ) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE dbo.hccm_Sites ADD CONSTRAINT
	FK_hccm_Sites_Modules FOREIGN KEY
	(
	ModuleId
	) REFERENCES dbo.Modules
	(
	ModuleId
	) ON UPDATE NO ACTION 
	 ON DELETE CASCADE
	
GO

create procedure [dbo].[hccm_GetSites]

@ModuleId int,
@Rows     int,
@Page     int,
@Category nvarchar(50)

as

declare @Total int

select @Total = count(*)
from dbo.hccm_Sites
where (@ModuleId is null or ModuleId = @ModuleId)
and (@Category is null or Categories like '%,' + @Category + ',%')
and IsActive = 1

select tbl.*, m.PortalId, u.UserID, u.Username, u.Email, u.DisplayName, @Total As Rows from (
    select row_number() over(order by CreatedOnDate desc) as Row, *
    from dbo.hccm_Sites
    where (@ModuleId is null or ModuleId = @ModuleId)
    and (@Category is null or Categories like '%,' + @Category + ',%')
    and IsActive = 1
) as tbl
inner join Modules m on  tbl.ModuleID = m.ModuleID
inner join Users u on tbl.CreatedByUserId = u.UserID
where Row between ((@Page - 1) * @Rows + 1) and (@Page * @Rows)
order by tbl.CreatedOnDate desc

GO


create procedure [dbo].[hccm_GetSitesByUserId]

@ModuleId int,
@UserId int

as

select s.*, u.UserID, u.Username, u.Email, u.DisplayName
from   dbo.hccm_Sites s 
inner join Users u on s.CreatedByUserId = u.UserID
where  ModuleId = @ModuleId
and s.CreatedByUserId = @UserId
order by Title

GO

create procedure [dbo].[hccm_GetSitesByURL]

@ModuleId int,
@URL nvarchar(250)

as

select *
from   dbo.hccm_Sites
where  ModuleId = @ModuleId
and URL like ('%' + @URL + '%')

GO

create procedure [dbo].[hccm_GetSite]

@SiteId int

as

select *
from   dbo.hccm_Sites
where  SiteId = @SiteId

GO

create procedure [dbo].[hccm_UpdateSite]

@SiteId                int,
@ModuleId              int,
@URL                   nvarchar(250),
@Title                 nvarchar(100),
@Description           nvarchar(2000),
@Categories            nvarchar(500),
@IsActive              bit,
@Thumbnail             nvarchar(250),
@UserId                int

as

if not exists ( select 1 from dbo.hccm_Sites where SiteId = @SiteId )
begin
  insert into dbo.hccm_Sites (
    ModuleId,
    URL,
    Title,
    Description,
	Categories,
    IsActive,
    Thumbnail,
    CreatedByUserId,
    CreatedOnDate,
    LastModifiedByUserId,
    LastModifiedOnDate
  )
  values (
    @ModuleId,
    @URL,
    @Title,
    @Description,
	@Categories,
    @IsActive,
    @Thumbnail,
    @UserId,
    getdate(),
    @UserId,
    getdate()
  )

  select @SiteId = SCOPE_IDENTITY()  
end
else
begin
  update dbo.hccm_Sites
  set URL = @URL,
      Title = @Title,
      Description = @Description,
      Categories = @Categories,
      IsActive = @IsActive,
      Thumbnail = @Thumbnail,
      LastModifiedByUserId = @UserId,
      LastModifiedOnDate = getdate()
  where SiteId = @SiteId
end

select @SiteId as SiteId

GO

create procedure [dbo].[hccm_GetSiteOwners]

@ModuleId int

as

select distinct UserId, DisplayName
from   dbo.hccm_Sites
inner join dbo.Users on dbo.hccm_Sites.CreatedByUserId = dbo.Users.UserId
where ModuleId = @ModuleId
order by DisplayName

GO

INSERT into dbo.Schedule ( TypeFullName, TimeLapse, TimeLapseMeasurement, RetryTimeLapse, RetryTimeLapseMeasurement, RetainHistoryNum, AttachToEvent, CatchUpEnabled, Enabled, ObjectDependencies, Servers, CreatedByUserID, CreatedOnDate, LastModifiedByUserID, LastModifiedOnDate, FriendlyName )
values ( 'HCC.Showcase.SiteJob, HCC.Showcase', 1, 'd', 0, 's', 10, '', 0, 1, '', NULL, NULL, getdate(), NULL, getdate(), 'Showcase Site Job' )
GO
   