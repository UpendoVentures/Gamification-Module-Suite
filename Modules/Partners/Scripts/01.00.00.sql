CREATE TABLE dbo.hccm_Partner
(
    PartnerId int NOT NULL IDENTITY (1, 1),
	PortalId int NOT NULL,
    PartnerName nvarchar(50) NOT NULL,
    Logo nvarchar(50) NOT NULL,
    Summary nvarchar(1000) NOT NULL,
    Description nvarchar(max) NOT NULL,
    City nvarchar(50) NOT NULL,
    Region nvarchar(50) NOT NULL,
    Telephone nvarchar(50) NOT NULL,
    URL nvarchar(255) NOT NULL,
    Email nvarchar(50) NOT NULL,
	Contact nvarchar(50) NULL,
	Services nvarchar(500) NULL,
    IsApproved bit NOT NULL,
    CreatedByUserID int NOT NULL,
    CreatedOnDate datetime NOT NULL,
    LastModifiedByUserID int NOT NULL,
    LastModifiedOnDate datetime NOT NULL,
)  ON [PRIMARY]
GO

ALTER TABLE dbo.hccm_Partner ADD CONSTRAINT
	PK_hccm_Partner PRIMARY KEY CLUSTERED 
	(
	    PartnerId
	) ON [PRIMARY]

GO

CREATE UNIQUE NONCLUSTERED INDEX IX_hccm_Partner ON dbo.hccm_Partner
	(
	    PartnerName
	) ON [PRIMARY]
GO

ALTER TABLE dbo.hccm_Partner ADD CONSTRAINT
	FK_hccm_Partner_Portals FOREIGN KEY
	(
	    PortalId
	) REFERENCES dbo.Portals
	(
	    PortalId
	) ON UPDATE NO ACTION 
	 ON DELETE CASCADE 

GO

CREATE TABLE dbo.hccm_PartnerUser
(
    PartnerUserId int NOT NULL IDENTITY (1, 1),
    PartnerId int NOT NULL,
    UserId int NOT NULL,
    CreatedOnDate datetime NOT NULL,
)  ON [PRIMARY]
GO

ALTER TABLE dbo.hccm_PartnerUser ADD CONSTRAINT
	PK_hccm_PartnerUser PRIMARY KEY CLUSTERED 
	(
	    PartnerUserId
	) ON [PRIMARY]

GO

CREATE UNIQUE NONCLUSTERED INDEX IX_hccm_PartnerUser ON dbo.hccm_PartnerUser
	(
	    PartnerId,
	    UserId
	) ON [PRIMARY]
GO

ALTER TABLE dbo.hccm_PartnerUser ADD CONSTRAINT
	FK_hccm_PartnerUser_hccm_Partner FOREIGN KEY
	(
	    PartnerId
	) REFERENCES dbo.hccm_Partner
	(
	    PartnerId
	) ON UPDATE NO ACTION 
	 ON DELETE CASCADE 
	 NOT FOR REPLICATION

GO

ALTER TABLE dbo.hccm_PartnerUser ADD CONSTRAINT
	FK_hccm_PartnerUser_hccm_User FOREIGN KEY
	(
	    UserId
	) REFERENCES dbo.Users
	(
	    UserId
	) ON UPDATE NO ACTION 
	 ON DELETE NO ACTION  
	 NOT FOR REPLICATION

GO

create procedure dbo.hccm_GetPartnerByUserId

@PortalId int,
@UserId int

as

select *
from dbo.hccm_Partner
where PortalId = @PortalId 
and CreatedByUserId = @UserId

GO

create procedure dbo.hccm_GetPartner

@PartnerId int

as

select *
from dbo.hccm_Partner
where PartnerId = @PartnerId

GO

create procedure dbo.hccm_UpdatePartner

@PortalId            int,
@PartnerName         nvarchar(50),
@Logo                nvarchar(50),
@Summary             nvarchar(1000),
@Description         nvarchar(max),
@City                nvarchar(50),
@Region              nvarchar(50),
@Telephone           nvarchar(50),
@URL                 nvarchar(255),
@Email               nvarchar(50),
@Contact             nvarchar(50),
@Services            nvarchar(500),
@IsApproved          bit,
@UserId              int

as

declare @PartnerId int

select @PartnerId = PartnerId
from dbo.hccm_Partner 
where PortalId = @PortalId
and CreatedByUserID = @UserId

if @PartnerId is null
begin
  insert into dbo.hccm_Partner (
    PortalId,
    PartnerName,
    Logo,
    Summary,
    Description,
    City,
    Region,
    Telephone,
    URL,
    Email,
	Contact,
	Services,
    IsApproved,
    CreatedByUserID,
    CreatedOnDate,
    LastModifiedByUserID,
    LastModifiedOnDate
  )
  values (
    @PortalId,
    @PartnerName,
    @Logo,
    @Summary,
    @Description,
    @City,
    @Region,
    @Telephone,
    @URL,
    @Email,
	@Contact,
	@Services,
    @IsApproved,
    @UserId,
    getdate(),
    @UserId,
    getdate()
  )

  select @PartnerId = SCOPE_IDENTITY()
end
else
begin
  update dbo.hccm_Partner
  set PortalId = @PortalId,
      PartnerName = @PartnerName,
      Logo = @Logo,
      Summary = @Summary,
      Description = @Description,
      City = @City,
      Region = @Region,
      Telephone = @Telephone,
      URL = @URL,
      Email = @Email,
	  Contact = @Contact,
	  Services = @Services,
      IsApproved = @IsApproved,
      LastModifiedByUserID = @UserId,
      LastModifiedOnDate = getdate()
  where CreatedByUserID = @UserId
end

select @PartnerId as PartnerId

GO

create procedure dbo.hccm_GetPartnerUsers

@PartnerId int

as

select *
from dbo.hccm_PartnerUser pu
inner join dbo.Users u on pu.UserId = u.UserId 
where PartnerId = @PartnerId

GO

create procedure dbo.hccm_GetUserPartners

@UserId int

as

select *
from dbo.hccm_PartnerUser pu
inner join dbo.hccm_Partner p on pu.PartnerId = p.PartnerId 
where UserId = @UserId

GO

create procedure dbo.hccm_AddPartnerUser

@PartnerId     int,
@UserId        int

as

if not exists (select 1 from dbo.hccm_PartnerUser where PartnerId = @PartnerId and UserId = @UserId)
begin
  insert into dbo.hccm_PartnerUser (
    PartnerId,
    UserId,
    CreatedOnDate
  )
  values (
    @PartnerId,
    @UserId,
    getdate()
  )
end

GO

create procedure dbo.hccm_DeletePartnerUser

@PartnerId     int,
@UserId        int

as

delete
from dbo.hccm_PartnerUser
where PartnerId = @PartnerId
and UserId = @UserId

GO

create procedure [dbo].[hccm_GetPartnerOwners]

@PortalId int

as

select distinct UserId, DisplayName
from   dbo.hccm_Partner
inner join dbo.Users on dbo.hccm_Partner.CreatedByUserId = dbo.Users.UserId
where PortalId = @PortalId
order by DisplayName

GO

create procedure dbo.hccm_GetPartnerActivity

@PortalId      int,
@PartnerId     int,
@Service       nvarchar(50),
@ActivityId    int,
@StartDate     datetime,
@EndDate       datetime,
@Rows          int,
@Page          int,
@Summary       bit

as

if @Summary = 1 
begin 
  declare @Total int

  select @Total = count(distinct p.PartnerId)
  from dbo.hccm_Partner p
  where p.PortalId = @PortalId
  and IsApproved = 1
  and (@Service is null or p.Services LIKE '%,' + @Service + ',%')

  select p.*, p.CreatedByUserID as UserId, Score, @Total as Rows
  from (
    select p.PartnerId, isnull(sum(Count * a.Factor),0) as [Score], row_number() over(order by isnull(sum(Count * a.Factor),0) desc) as Row 
    from dbo.hccm_Partner p
    left outer join dbo.hccm_PartnerUser pu on p.PartnerId = pu.PartnerId
    left outer join dbo.Users u on pu.UserId = u.UserId
    left outer join dbo.hccm_UserActivity ua on u.UserID = ua.UserId
    left outer join dbo.hccm_Activity a on ua.ActivityId = a.ActivityId  
    where p.PortalId = @PortalId
	and   (p.PartnerId = @PartnerId or @PartnerId is null)
    and   (ua.ActivityId = @ActivityId or @ActivityId is null or ua.ActivityId is null)
    and   (ua.Date >= @StartDate or @StartDate is null or ua.Date is null) 
    and   (ua.Date <= @EndDate or @EndDate is null or ua.Date is null)
    and   (a.IsActive = 1 or a.IsActive is null)
    and   p.IsApproved = 1
  group by p.PartnerId
  ) as tbl
  inner join hccm_Partner p on tbl.PartnerId = p.PartnerId
  where Row between ((@Page - 1) * @Rows + 1) and (@Page * @Rows)
  and (@Service is null or p.Services LIKE '%,' + @Service + ',%')
  order by Row
end
else
begin
  select p.PartnerId, p.PartnerName, u.UserId, u.DisplayName, a.ActivityName, sum(Count * a.Factor) as [Score]
  from dbo.hccm_UserActivity ua
  inner join dbo.hccm_Activity a on ua.ActivityId = a.ActivityId  
  inner join dbo.Users u on ua.UserId = u.UserId
  inner join dbo.hccm_PartnerUser pu on u.UserId = pu.UserId
  inner join dbo.hccm_Partner p on pu.PartnerId = p.PartnerId
  where p.PortalId = @PortalId
  and   (p.PartnerId = @PartnerId or @PartnerId is null)
  and   (ua.ActivityId = @ActivityId or @ActivityId is null)
  and   (ua.Date >= @StartDate or @StartDate is null) 
  and   (ua.Date <= @EndDate or @EndDate is null)
  and   a.IsActive = 1
  and   p.IsApproved = 1
  group by p.PartnerId, p.PartnerName, u.UserId, u.DisplayName, a.ActivityName
  order by p.PartnerName, u.DisplayName, a.ActivityName
end

GO

