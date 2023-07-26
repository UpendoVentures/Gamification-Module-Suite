create procedure dbo.hccm_GetVisitorsOnline

@PortalId int,
@OnlineTime int

as

with x as (
  select distinct IP
  from hccm_Visits 
  where PortalId = @PortalId
  and Date >= dateadd(minute, -@OnlineTime, getdate())
)
select x.IP, top1.*
from x 
cross apply 
  (select top 1 v.*
   from hccm_Visits v
   where IP = x.IP
   order by v.Date desc) as top1

GO





