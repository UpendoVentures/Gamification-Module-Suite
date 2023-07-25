using System.Collections.Generic;
using WebMatrix.Data;

namespace HCC.VisitorsOnline
{

    public class VisitorsOnlineController
	{
        public IEnumerable<dynamic> GetVisitorsOnline(int PortalId, int OnlineTime)
        {
            IEnumerable<dynamic> objVisitors;
            using (var db = Database.Open("SiteSqlServer"))
            {
                objVisitors = db.Query("exec hccm_GetVisitorsOnline @0, @1", PortalId, OnlineTime);
            }
            return objVisitors;
        }
     }
}