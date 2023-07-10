CREATE TABLE dbo.hccm_Groups
(
    GroupId int NOT NULL IDENTITY (1, 1),
	PortalId int NOT NULL,
    GroupName nvarchar(50) NOT NULL,
    URL nvarchar(255) NOT NULL,
    City nvarchar(50) NOT NULL,
    Region nvarchar(50) NOT NULL,
    Latitude nvarchar(50) NOT NULL,
    Longitude nvarchar(50) NOT NULL,
    IsActive bit NOT NULL,
    CreatedByUserID int NOT NULL,
    CreatedOnDate datetime NOT NULL,
    LastModifiedByUserID int NOT NULL,
    LastModifiedOnDate datetime NOT NULL,
)  ON [PRIMARY]
GO

ALTER TABLE dbo.hccm_Groups ADD CONSTRAINT
	PK_hccm_Groups PRIMARY KEY CLUSTERED 
	(
	    GroupId
	) ON [PRIMARY]

GO

CREATE UNIQUE NONCLUSTERED INDEX IX_hccm_Groups ON dbo.hccm_Groups
	(
	    GroupName
	) ON [PRIMARY]
GO

ALTER TABLE dbo.hccm_Groups ADD CONSTRAINT
	FK_hccm_Groups_Portals FOREIGN KEY
	(
	PortalId
	) REFERENCES dbo.Portals
	(
	PortalId
	) ON UPDATE NO ACTION 
	 ON DELETE CASCADE
	
GO

create procedure dbo.hccm_GetGroups

@PortalId int

as

select *
from dbo.hccm_Groups
where PortalId = @PortalId
and IsActive = 1

GO

create procedure dbo.hccm_GetGroupByUserId

@PortalId int,
@UserId int

as

select *
from dbo.hccm_Groups
where PortalId = @PortalId
and CreatedByUserId = @UserId

GO

create procedure dbo.hccm_UpdateGroup

@PortalId            int,
@GroupName           nvarchar(50),
@URL                 nvarchar(255),
@City                nvarchar(50),
@Region              nvarchar(50),
@Latitude            nvarchar(50),
@Longitude           nvarchar(50),
@IsActive            bit,
@UserId              int

as

declare @GroupId int

select @GroupId = GroupId
from dbo.hccm_Groups 
where PortalId = @PortalId
and CreatedByUserID = @UserId

if @GroupId is null
begin
  insert into dbo.hccm_Groups (
    PortalId,
    GroupName,
    URL,
    City,
    Region,
    Latitude,
    Longitude,
    IsActive,
    CreatedByUserID,
    CreatedOnDate,
    LastModifiedByUserID,
    LastModifiedOnDate
  )
  values (
    @PortalId,
    @GroupName,
    @URL,
    @City,
    @Region,
    @Latitude,
    @Longitude,
    @IsActive,
    @UserId,
    getdate(),
    @UserId,
    getdate()
  )

  select @GroupId = SCOPE_IDENTITY()
end
else
begin
  update dbo.hccm_Groups
  set PortalId = @PortalId,
      GroupName = @GroupName,
      URL = @URL,
      City = @City,
      Region = @Region,
      Latitude = @Latitude,
      Longitude = @Longitude,
      IsActive = @IsActive,
      LastModifiedByUserID = @UserId,
      LastModifiedOnDate = getdate()
  where CreatedByUserID = @UserId
end

select @GroupId as GroupId

GO



